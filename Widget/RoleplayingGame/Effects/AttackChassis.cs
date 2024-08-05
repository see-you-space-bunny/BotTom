using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Enums;
using RoleplayingGame.Interfaces;
using RoleplayingGame.Objects;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Effects;

public class AttackChassis
{
	private const float OverallDamageScaling		= 5.5f;
	private const float OverallDamageExponentBase	= 1.3851f;
	private const float LevelScaleDivisor			= 1000.0f;

	public AttackType AttackType;
	private string _textName;
	protected readonly float _accuracy;
	protected readonly float _impact;
	protected readonly float _harm;
	protected readonly Dictionary<Ability,float> _accuracyScales;
	protected readonly Dictionary<Ability,float> _impactScales;
	protected readonly Dictionary<Ability,float> _harmScales;
	protected readonly StatusEffect[] _carriedEffects;

	public AttackEvent BuildAttack(Actor target,Actor source,EnvironmentSource environmentSource = EnvironmentSource.None)
	{
		return new AttackEvent(
			source,
			environmentSource,
			target,
			OverallDamageScaling
				* _harm
				* (_harmScales[Ability.Level]
					* source.Level
					/ LevelScaleDivisor
				+ (float)Math.Pow(
					OverallDamageExponentBase,
					(double)(_harmScales.Keys.Sum(k =>(k == Ability.Level ? 0 : source.Abilities[k].GetActualValue()) * _harmScales[k])
						/ source.Level)
				)),
			OverallDamageScaling
				* _impact
				* (_harmScales[Ability.Level]
					* source.Level
					/ LevelScaleDivisor
				+ (float)Math.Pow(
					OverallDamageExponentBase,
					(double)(_impactScales.Keys.Sum(k =>(k == Ability.Level ? 0 : source.Abilities[k].GetActualValue()) * _impactScales[k])
						/ source.Level)
				)),
			OverallDamageScaling
				* _accuracy
				* (_accuracyScales[Ability.Level]
					* source.Level
					/ LevelScaleDivisor
				+ (float)Math.Pow(
					OverallDamageExponentBase,
					(double)(_accuracyScales.Keys.Sum(k =>(k == Ability.Level ? 0 : source.Abilities[k].GetActualValue()) * _accuracyScales[k])
						/ source.Level)
				))
		);
	}

	public AttackChassis(
		AttackType attackType,string textName,
		float harm,float impact,float accuracy,
		Dictionary<Ability,float> harmScales,Dictionary<Ability,float> impactScales,Dictionary<Ability,float> accuracyScales,
		StatusEffect[]? carriedEffects = null
	)
	{
		AttackType			= attackType;
		_textName			= textName;
		_harm            	= harm;
		_impact          	= impact;
		_accuracy        	= accuracy;
		_accuracyScales		= accuracyScales;
		_impactScales		= impactScales;
		_harmScales			= harmScales;
		_carriedEffects  	= carriedEffects ?? [];
	}
}