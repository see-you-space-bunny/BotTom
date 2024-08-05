using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Effects;

public class ActiveStatusEffectBuilder
{
	private StatusEffect _effectType;

	private readonly List<(Ability Ability,float BaseValue)> _affectsAbilities;

	private readonly List<(Resource Resource,ResourceModifier Modifier,float BaseValue)> _affectsResources;

	private readonly List<(DerivedAbility Ability,float BaseValue)> _affectsDerivedAbilities;

	private readonly List<(BodyAttribute BodyAttribute,float BaseValue)> _affectsBodyAttributes;

	private float _intensity;

	private Actor _target;

	private Actor? _source;

	private bool _tagged;

	private StatusEffect _compoundsTo;

	private readonly List<StatusEffect> _overrides;

	private StatusEffectTermination _termination;

	private float _procChance;

    public ActiveStatusEffectBuilder WithEffectType(StatusEffect value)
    {
        _effectType = value;
        return this;
    }

    public ActiveStatusEffectBuilder AffectsAbility(Ability ability,float value = 0.0f)
    {
		if (ability == Ability.None)
			_affectsAbilities.Clear();
        else
			_affectsAbilities.Add((ability,value));
        return this;
    }

    public ActiveStatusEffectBuilder AffectsResources(Resource? resource,ResourceModifier modifier,float value = 0.0f)
    {
		if (resource == Resource.None)
			_affectsResources.Clear();
        else if (modifier == ResourceModifier.None)
			_affectsResources.Remove(_affectsResources.SingleOrDefault(li=>li.Resource == resource && li.Modifier == modifier));
		else
			_affectsResources.Add(((Resource)resource!,modifier,value));
        return this;
    }

    public ActiveStatusEffectBuilder AffectsDerivedAbility(DerivedAbility derivedAbility,float value = 0.0f)
    {
		if (derivedAbility == DerivedAbility.None)
			_affectsDerivedAbilities.Clear();
        else
			_affectsDerivedAbilities.Add((derivedAbility,value));
        return this;
    }

    public ActiveStatusEffectBuilder AffectsBodyAttribute(BodyAttribute bodyAttribute,float value = 0.0f)
    {
		if (bodyAttribute == BodyAttribute.None)
			_affectsBodyAttributes.Clear();
        else
			_affectsBodyAttributes.Add((bodyAttribute,value));
        return this;
    }

    public ActiveStatusEffectBuilder WithIntensity(float value)
    {
        _intensity = value;
        return this;
    }

    public ActiveStatusEffectBuilder WithTarget(Actor value)
    {
        _target = value;
        return this;
    }

    public ActiveStatusEffectBuilder WithSource(Actor? value)
    {
        _source = value;
        return this;
    }

    public ActiveStatusEffectBuilder WithTagging(bool value = true)
    {
        _tagged = value;
        return this;
    }

    public ActiveStatusEffectBuilder CoumpoundsTo(StatusEffect value)
    {
        _compoundsTo = value;
        return this;
    }

    public ActiveStatusEffectBuilder WithOverride(StatusEffect? value)
    {
		if (value is null)
			_overrides.Clear();
        else
			_overrides.Add((StatusEffect)value);
        return this;
    }

    public ActiveStatusEffectBuilder WithTermination(StatusEffectTermination value)
    {
        _termination = value;
        return this;
    }

    public ActiveStatusEffectBuilder WithProcChance(float value)
    {
        _procChance = value;
        return this;
    }

    public bool IsReadyToBuild() =>
		_target is not null && _effectType != StatusEffect.None;

	public ActiveStatusEffect Build()
	{
		return new ActiveStatusEffect( 
			_effectType,
			_target,
			_intensity,
			_termination,
			[.. _affectsAbilities],
			[.. _affectsResources],
			[.. _affectsDerivedAbilities],
			[.. _affectsBodyAttributes],
			_compoundsTo,
			[.. _overrides],
			_source,
			_tagged
		);
	}
	
	internal ActiveStatusEffectBuilder()
	{
		_effectType = StatusEffect.None;
		_target		= default!;
		/////
		_affectsAbilities		= [];
		_affectsResources		= [];
		_affectsDerivedAbilities= [];
		_affectsBodyAttributes	= [];
		_intensity				= 1.0f;
		_source					= null;
		_tagged					= false;
		_compoundsTo 			= StatusEffect.None;
		_overrides				= [];
		_termination			= StatusEffectTermination.FullRecovery;
		_procChance				= 1.0f;
	}
}