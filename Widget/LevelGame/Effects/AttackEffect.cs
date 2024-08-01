using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;
using RoleplayingGame.Enums;
using RoleplayingGame.Interfaces;
using RoleplayingGame.Objects;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Effects;

public class AttackEvent : IPendingEvent
{
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

	public readonly Actor Source;
	public readonly EnvironmentSource EnvironmentSource;
	public readonly Actor Target;
	public readonly float Accuracy;
	public readonly float Impact;
	public readonly float Harm;

	private ActiveStatusEffect[] CarriedEffects;
	
	protected float _remainingAccuracy;
	protected float _remainingImpact;
	protected float _remainingHarm;

	protected float _appliedAccuracy;
	protected float _appliedImpact;
	protected float _appliedHarm;
	protected float _appliedOverkill;

	protected bool _hit;
	protected bool _protBreak;
	protected bool _kill;

	protected float _accuracyRatio => Accuracy == 0 ? Math.Max(_remainingAccuracy / Accuracy,0) : -1;

	protected float _impactRatio = 0;

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
	public bool TryToHarm(CharacterResource health)
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

	public (ulong,bool,ulong,bool,ulong,bool,ulong) AttackInfo() =>
		((ulong)_appliedAccuracy,_hit,(ulong)_appliedImpact,_protBreak,(ulong)_appliedHarm,_kill,(ulong)_appliedOverkill);

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

	public AttackEvent(Actor source,Actor target,float harm,float impact,float accuracy,ActiveStatusEffect[]? carriedEffects = null)
		: this(source,EnvironmentSource.None,target,harm,impact,accuracy,carriedEffects)
	{ }

	public AttackEvent(Actor source,EnvironmentSource environmentSource,Actor target,float harm,float impact,float accuracy,ActiveStatusEffect[]? carriedEffects = null)
	{
		_responder			= default!;
		Source				= source;
		EnvironmentSource	= environmentSource;
		Target				= target;
		Harm				= harm;
		Impact				= impact;
		Accuracy			= accuracy;
		CarriedEffects		= carriedEffects ?? [];
	}

#region IPendingAction
	User _responder;
	Channel? _channel;
	User IPendingEvent.Responder => _responder;
	Channel? IPendingEvent.Channel => _channel;

	RoleplayingGameCommand[] IPendingEvent.ExpectedResponses => [RoleplayingGameCommand.Defend];

	IPendingEvent IPendingEvent.WithResponder(User value)
	{
		_responder = value;
		return this;
	}

	IPendingEvent IPendingEvent.WithChannel(Channel value)
	{
		_channel = value;
		return this;
	}

	IPendingEvent IPendingEvent.ExecuteEffect()
	{
		if (!TryToHit(Target.Evasion,Target.Health))
		{ }

		if (!TryToImpact(Target.Protection))
		{ }

		if (!TryToHarm(Target.Health))
		{
            FRoleplayMC.ApplyStatusEffect(StatusEffect.Defeated,Target,1.0f,null);
		}

		if (EnvironmentSource != EnvironmentSource.None)
		{
			Target.Statistics.RecordIncomingAttackResults(Source,(ulong)_appliedAccuracy,_hit,(ulong)_appliedImpact,_protBreak,(ulong)_appliedHarm,_kill,(ulong)_appliedOverkill);
			Source.Statistics.RecordOutgoingAttackResults(Target,(ulong)_appliedAccuracy,_hit,(ulong)_appliedImpact,_protBreak,(ulong)_appliedHarm,_kill,(ulong)_appliedOverkill);
		}
		else
		{
			Target.Statistics.RecordIncomingAttackResults(EnvironmentSource,(ulong)_appliedAccuracy,_hit,(ulong)_appliedImpact,_protBreak,(ulong)_appliedHarm,_kill,(ulong)_appliedOverkill);
			Source.Statistics.RecordOutgoingAttackResults(EnvironmentSource,(ulong)_appliedAccuracy,_hit,(ulong)_appliedImpact,_protBreak,(ulong)_appliedHarm,_kill,(ulong)_appliedOverkill);
		}
		return this;
	}

	IPendingEvent IPendingEvent.EnqueueMessage(ApiConnection api)
	{
		var message = new FChatMessageBuilder();

		if (_channel is not null)
		{
			message
				.WithChannel(_channel)
				.WithMessageType(FChatApi.Enums.FChatMessageType.Basic);
		}
		else
		{
			message
				.WithRecipient(_responder)
				.WithMessageType(FChatApi.Enums.FChatMessageType.Whisper);
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
}