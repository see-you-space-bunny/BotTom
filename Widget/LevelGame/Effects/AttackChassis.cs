using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LevelGame.Enums;
using LevelGame.Interfaces;
using LevelGame.Objects;
using LevelGame.SheetComponents;

namespace LevelGame.Effects;

public class AttackChassis
{
	private const float OverallDamageScaling	= 0.15f;
	private const uint LevelGapToDoubling		= 100;

	public AttackType AttackType;
	private string _textName;
	protected readonly float _accuracy;
	protected readonly float _impact;
	protected readonly float _harm;
	protected readonly Dictionary<Ability,float> _accuracyScales;
	protected readonly Dictionary<Ability,float> _damageScales;
	protected readonly StatusEffect[] _carriedEffects;

	public AttackEffect BuildAttack(Actor target,Actor source,EnvironmentSource environmentSource = EnvironmentSource.None)
	{
		float levelGapMultiplier = (float)Math.Pow(
			Math.Pow(2.0d,
				// Math.Abs works with douple-precision floating point numbers (double), but not single-precision (float)
				Math.Abs((double)(source.Level.Current - target.Level.Current) / LevelGapToDoubling)
			),
			source.Level.Current - target.Level.Current >= 0 ? 1 : -1
		);
		return new AttackEffect(
			source,
			environmentSource,
			target,
			_harm		* _damageScales		.Keys.Sum(k =>(k == Ability.Level ? source.Level.Current : source.Abilities[k].Current) * _damageScales		[k]) * levelGapMultiplier * OverallDamageScaling,
			_impact		* _damageScales		.Keys.Sum(k =>(k == Ability.Level ? source.Level.Current : source.Abilities[k].Current) * _damageScales		[k]) * levelGapMultiplier * OverallDamageScaling,
			_accuracy	* _accuracyScales	.Keys.Sum(k =>(k == Ability.Level ? source.Level.Current : source.Abilities[k].Current) * _accuracyScales	[k]) * levelGapMultiplier * OverallDamageScaling
		);
	}

	public AttackChassis(
		AttackType attackType,string textName,
		float harm,float impact,float accuracy,
		Dictionary<Ability,float> accuracyScales,Dictionary<Ability,float> damageScales,
		StatusEffect[]? carriedEffects = null
	)
	{
		AttackType			= attackType;
		_textName			= textName;
		_harm            	= harm;
		_impact          	= impact;
		_accuracy        	= accuracy;
		_accuracyScales		= accuracyScales;
		_damageScales		= damageScales;
		_carriedEffects  	= carriedEffects ?? [];
	}
}