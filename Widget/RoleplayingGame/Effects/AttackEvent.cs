using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Objects;
using RoleplayingGame.Enums;
using RoleplayingGame.Factories;
using RoleplayingGame.Interfaces;
using RoleplayingGame.Objects;
using RoleplayingGame.SheetComponents;
using RoleplayingGame.Systems;

namespace RoleplayingGame.Effects;

internal class AttackEvent : IPendingEvent
{
#region (-) Constants
	private const float MinimumStaminaRatio = 0.15f;
	private const float RemainingHealthMultiplier = 2.5f;
	/// <summary>
	/// {0} = attacker<br/>
	/// {1} = target<br/>
	/// {2} = attack flavor text<br/>
	/// {3} = die roll (attacker)<br/>
	/// {4} = die roll (target)<br/>
	/// {5} = attack outcome
	/// </summary>
	private const string DefaultMessageFormat = "{0} attacked {1} {2} ({3}🎲 vs {4}🎲)\n{5}";
#endregion


#region (-) Fields
	internal required StatusEffectFactory StatusEffectFactory { private get; set; }
	internal required DieRoller DieRoller { private get; set; }
#endregion


#region (+) Fields
	internal readonly Actor Source;
	internal readonly EnvironmentSource EnvironmentSource;
	internal readonly Actor Target;
	internal readonly float Accuracy;
	internal readonly float Impact;
	internal readonly float Harm;
#endregion


#region (+) Fields
	private ActiveStatusEffect[] CarriedEffects;
#endregion


#region (#) Fields
	protected float	_remainingAccuracy;
	protected float	_remainingImpact;
	protected float	_remainingHarm;
	protected float	_appliedAccuracy;
	protected float	_appliedImpact;
	protected float	_appliedHarm;
	protected float	_appliedOverkill;
	protected bool	_hit;
	protected bool	_protBreak;
	protected bool	_kill;
	protected float	_impactRatio;
#endregion


#region (#) Properties
	protected float	_accuracyRatio	=> Accuracy != 0 ? Math.Max(_remainingAccuracy / Accuracy,0) : -1;
#endregion


#region Try To X
/// <summary>
/// 
/// </summary>
/// <param name="evasion"></param>
/// <returns>true if the attack hit its target</returns>
	public bool TryToHit(CharacterResource evasion,CharacterResource health)
	{
		RefreshRemainingDamage();
		int initialEvasion	= evasion.Current;
		_appliedAccuracy	= (float)Math.Min(
			_remainingAccuracy
				/ (
					health.Current
						/ health.SoftLimit
							* RemainingHealthMultiplier
				),
			_remainingAccuracy
				- health.Current
					* MinimumStaminaRatio
		);
		evasion.BaseValue   -= _appliedAccuracy;
		_remainingAccuracy	-= initialEvasion;
		_hit = Accuracy >= initialEvasion;
		return _hit;
	}

/// <summary>
/// 
/// </summary>
/// <param name="protection"></param>
/// <returns>true if the target has 0 or less protection remaining</returns>
	public bool TryToImpact(CharacterResource protection)
	{
		if (!_hit)
			return false;
		if (Accuracy > 0)
		{
			_remainingImpact *= _accuracyRatio;
		}
		if (_remainingImpact > 0)
		{
			float overflowImpact    = Math.Max(_remainingImpact - protection.Current,0);
			_appliedImpact			= _remainingImpact - overflowImpact;
			protection.BaseValue	-= _remainingImpact;
			_impactRatio             = overflowImpact / _remainingImpact;
		}
		_protBreak = protection.Current <= 0;
		return _protBreak;
	}

/// <summary>
/// 
/// </summary>
/// <param name="health"></param>
/// <returns>true if the target has 0 or less health remaining</returns>
	internal bool TryToHarm(CharacterResource health)
	{
		if (!_protBreak)
			return false;
		if (Accuracy > 0)
		{
			_remainingHarm   *= _accuracyRatio;
		}
		_remainingHarm		*= _impactRatio;
		health.BaseValue    -= _remainingHarm;
		_kill = health.BaseValue <= 0;
		if (health.BaseValue < 0)
			_appliedOverkill -= health.BaseValue;
		_appliedHarm = _remainingHarm - _appliedOverkill;
		return _kill;
	}
#endregion


#region Refresh/Reset
	private void RefreshRemainingDamage()
	{
		_remainingHarm		= Harm;
		_remainingImpact	= Impact;
		_remainingAccuracy	= Accuracy;
		_appliedAccuracy	= 0;
		_appliedImpact		= 0;
		_appliedHarm		= 0;
		_appliedOverkill	= 0;
		_hit				= false;
		_protBreak			= false;
		_kill				= false;
		_impactRatio		= 0;
	}
#endregion


#region With
	internal AttackEvent WithInitiator(User value)
	{
		_initiator = value;
		return this;
	}

	internal AttackEvent WithResponder(User value)
	{
		_responder = value;
		return this;
	}

	internal AttackEvent WithChannel(Channel value)
	{
		_channel = value;
		return this;
	}
#endregion


#region ExecuteEffect
	internal AttackEvent ExecuteEffect(RoleplayingGameCommand command)
	{
		if (!_expectedResponses.Contains(command))
			throw new ArgumentException($"{command} is an invalid response to an {RoleplayingGameCommand.Attack}",nameof(command));

		switch (command)
		{
			default:
				DieRoll	attack	=	DieRoller.StandardRoll([Source.Abilities[Ability.Power]]);
				DieRoll	defense	=	DieRoller.StandardRoll([Target.Abilities[Ability.Dexterity]]);
				break;
		}

		if (TryToHit(Target.Evasion,Target.Health))
		{ }

		if (TryToImpact(Target.Protection))
		{ }

		if (TryToHarm(Target.Health))
		{
            //	StatusEffect.Defeated
		}
		return this;
	}
#endregion


#region EnqueueMessage
	internal AttackEvent EnqueueMessage(ApiConnection api)
	{
		var message = new FChatMessageBuilder();

		if (_channel is not null)
		{
			message
				.WithChannel(_channel)
				.WithMessageType(FChatMessageType.Basic);
		}
		else
		{
			message
				.WithRecipient(_responder ?? _initiator)
				.WithMessageType(FChatMessageType.Whisper);
		}
		
		message.WithMessage(
			string.Format(
				DefaultMessageFormat,
				Source.CharacterName,
				Target.CharacterName,
				"lorem ipsum!",
				"[color=red]1[/color]",
				"47",
				"Lorem ipsum dolor sit amet.."
			)
		);

		api.EnqueueMessage(message);
		return this;
	}
#endregion


#region IPendingAction
	readonly RoleplayingGameCommand[] _expectedResponses;
	User _initiator;
	User _responder;
	Channel _channel;
	User IPendingEvent.Initiator									=>	_initiator;
	User IPendingEvent.Responder									=>	_responder;
	Channel? IPendingEvent.Channel									=>	_channel;
	RoleplayingGameCommand[] IPendingEvent.ExpectedResponses		=> _expectedResponses;
	IPendingEvent IPendingEvent.WithInitiator(User value)			=>	WithInitiator(value);
	IPendingEvent IPendingEvent.WithResponder(User value)			=>	WithResponder(value);
	IPendingEvent IPendingEvent.WithChannel(Channel value)			=>	WithChannel(value);
	IPendingEvent IPendingEvent.EnqueueMessage(ApiConnection api)	=>	EnqueueMessage(api);
#endregion


#region Constructor
	internal AttackEvent(Actor source,Actor target,float harm,float impact,float accuracy,ActiveStatusEffect[]? carriedEffects = null)
		: this(source,EnvironmentSource.None,target,harm,impact,accuracy,carriedEffects)
	{ }

	internal AttackEvent(Actor source,EnvironmentSource environmentSource,Actor target,float harm,float impact,float accuracy,ActiveStatusEffect[]? carriedEffects = null)
	{
		_expectedResponses	=	[RoleplayingGameCommand.Defend];
		_initiator			=	null!;
		_responder			=	null!;
		_channel			=	null!;
		Source				=	source;
		EnvironmentSource	=	environmentSource;
		Target				=	target;
		Harm				=	harm;
		Impact				=	impact;
		Accuracy			=	accuracy;
		CarriedEffects		=	carriedEffects ?? [];
	}
#endregion
}