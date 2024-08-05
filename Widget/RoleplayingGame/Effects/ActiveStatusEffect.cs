using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Effects;

public class ActiveStatusEffect
{
	readonly public StatusEffect EffectType;
	readonly public Actor Target;
	readonly public float Intensity;
	readonly public StatusEffectTermination Termination;
	readonly public (Ability Ability,float BaseValue)[] AffectsAbilities;
	readonly public (Resource Ability,ResourceModifier Modifier,float BaseValue)[] AffectsResources;
	readonly public (DerivedAbility Ability,float BaseValue)[] AffectsDerivedAbilities;
	readonly public (BodyAttribute Ability,float BaseValue)[] AffectsBodyAttributes;
	readonly public StatusEffect CompoundsTo;
	readonly public StatusEffect[] Overrides;
	readonly public Actor? Source;
	readonly public bool Tagged;

	internal ActiveStatusEffect(
        StatusEffect effectType,
        Actor target,
        float intensity,
		StatusEffectTermination termination,
        (Ability Ability, float BaseValue)[] affectsAbilities,
		(Resource Ability,ResourceModifier Modifier,float BaseValue)[] affectsResources,
		(DerivedAbility Ability,float BaseValue)[] affectsDerivedAbilities,
		(BodyAttribute Ability,float BaseValue)[] affectsBodyAttributes,
		StatusEffect compoundsTo,
		StatusEffect[] overrides,
		Actor? source = null,
		bool tagged = false
	)
	{
		EffectType				= effectType;
		Target					= target;
		Intensity				= intensity;
		Termination				= termination;
		AffectsAbilities		= affectsAbilities;
		AffectsResources		= affectsResources;
		AffectsDerivedAbilities	= affectsDerivedAbilities;
		AffectsBodyAttributes	= affectsBodyAttributes;
		CompoundsTo 			= compoundsTo;
		Overrides				= overrides;
		Source					= source;
		Tagged					= tagged;
	}
}