using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LevelGame.Enums;
using LevelGame.Interfaces;
using LevelGame.Objects;
using LevelGame.SheetComponents;

namespace LevelGame.Effects;

public class AttackEffect : IAttackEffect
{
	private const float MinimumStaminaRatio = 0.15f;

	public Actor Source;
	public EnvironmentSource EnvironmentSource;

	public readonly float Accuracy;
	public readonly float Impact;
	public readonly float Harm;

	private float _remainingAccuracy;
	private float _remainingImpact;
	private float _remainingHarm;

	private float _appliedAccuracy;
	private float _appliedImpact;
	private float _appliedHarm;
	private float _appliedOverkill;

	private bool _hit;
	private bool _protBreak;
	private bool _kill;

	private float AccuracyRatio => Accuracy == 0 ? Math.Max(_remainingAccuracy / Accuracy,0) : -1;
	private float ImpactRatio = 0;

	public readonly ActiveStatusEffectBuilder[] CarriedEffects;

/// <summary>
/// 
/// </summary>
/// <param name="evasion"></param>
/// <returns>true if the attack hit its target</returns>
	public bool TryToHit(CharacterResource evasion,CharacterResource health)
	{
		RefreshRemainingDamage();
		int initialEvasion	= evasion.Current;
		_appliedAccuracy	= (float)Math.Min(_remainingAccuracy / Math.Pow(2,health.Current/health.SoftLimit),(health.Current)*MinimumStaminaRatio);
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
			_remainingImpact *= AccuracyRatio;
		}
		if (_remainingImpact > 0)
		{
			float overflowImpact    = Math.Max(_remainingImpact - protection.Current,0);
			_appliedImpact			= _remainingImpact - overflowImpact;
			protection.BaseValue	-= _remainingImpact;
			ImpactRatio             = overflowImpact / _remainingImpact;
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
			_remainingHarm   *= AccuracyRatio;
		}
		_remainingHarm		*= ImpactRatio;
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
		ImpactRatio			= 0;
	}

	public AttackEffect(Actor source,float harm,float impact,float accuracy,ActiveStatusEffectBuilder[]? carriedEffects = null)
	{
		Source				= source;
		EnvironmentSource	= EnvironmentSource.None;
		Harm            	= harm;
		Impact          	= impact;
		Accuracy        	= accuracy;
		CarriedEffects  	= carriedEffects ?? [];
	}

	public AttackEffect(Actor source,EnvironmentSource environmentSource,float harm,float impact,float accuracy,ActiveStatusEffectBuilder[]? carriedEffects = null)
		: this(source,harm,impact,accuracy,carriedEffects)
	{
		Source				= source;
		EnvironmentSource	= environmentSource;
		Harm            	= harm;
		Impact          	= impact;
		Accuracy        	= accuracy;
		CarriedEffects  	= carriedEffects ?? [];
	}
}