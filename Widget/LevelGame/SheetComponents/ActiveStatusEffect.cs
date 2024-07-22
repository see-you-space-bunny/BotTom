using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LevelGame.Enums;
using LevelGame.Objects;

namespace LevelGame.SheetComponents;

public class ActiveStatusEffect
{
	public required StatusEffect EffectType;
	
	public required List<(Ability Ability,float BaseValue)> AffectsAbilities;

	public required List<(Resource Ability,ResourceModifier Modifier,float BaseValue)> AffectsResources;

	public required List<(DerivedAbility Ability,float BaseValue)> AffectsDerivedAbilities;

	public required List<(BodyAttribute Ability,float BaseValue)> AffectsBodyAttributes;
	
	public float Intensity;
	
	public required Actor Target;
	
	public Actor? Source;
	
	public bool Tagged;

	public StatusEffect CompoundsTo;

	public StatusEffect[] Overrides;

	public StatusEffectTermination Termination;

	public void Apply()
	{

	}

	private ActiveStatusEffect()
	{
		AffectsAbilities		= [];
		AffectsDerivedAbilities	= [];
		AffectsBodyAttributes	= [];
		Intensity				= 1.0f;
		Source					= null;
		Tagged					= false;
		CompoundsTo 			= StatusEffect.None;
		Overrides				= [];
		Termination				= StatusEffectTermination.FullRecovery;
	}
}