using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LevelGame.Objects;
using LevelGame.SheetComponents;

namespace LevelGame.Effects;

public class AttackEffect
{
	public Actor? Source;

	public readonly float Accuracy;
	public readonly float Impact;
	public readonly float Harm;

	private float _remainingAccuracy;
	private float _remainingImpact;
	private float _remainingHarm;

	private float _appliedHarm;
	private float _appliedImpact;
	private float _appliedAccuracy;

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
	public bool TryToHit(CharacterResource evasion)
	{
		RefreshRemainingDamage();
		int initialEvasion	= evasion.Current;
		evasion.BaseValue   -= _remainingAccuracy; // reduce actual evasion loss by a 'stamina' value
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
		return _kill;
	}

	public (float,float,bool,float,float,bool,float,bool) AttackInfo() =>
		(Accuracy,AccuracyRatio,_hit,Impact,ImpactRatio,_protBreak,Harm,_kill);

	private void RefreshRemainingDamage()
	{
		_remainingHarm		= Harm;
		_remainingImpact	= Impact;
		_remainingAccuracy	= Accuracy;
		_hit				= false;
		_protBreak			= false;
		_kill				= false;
		ImpactRatio			= 0;
	}

	public AttackEffect(float harm,float impact,float accuracy,ActiveStatusEffectBuilder[]? carriedEffects = null,Actor? source = null)
	{
		Source			= source;
		Harm            = harm;
		Impact          = impact;
		Accuracy        = accuracy;
		CarriedEffects  = carriedEffects ?? [];
	}

	AttackEffect()
	{
		CarriedEffects = [];
	}
}