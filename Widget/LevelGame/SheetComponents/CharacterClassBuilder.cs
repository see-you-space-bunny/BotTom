using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Widget.LevelGame.Enums;

namespace Widget.LevelGame.SheetComponents;

public class CharacterClassBuilder
{
	#region Constants
	private const float DefaultDamageAbilityScales = 0.00f;
	private const float DefaultDamageAbilityGroupScales = 1.00f;
	private const float DefaultResourceModifier = 1.00f;
	private const float DefaultResourceAbilityScales = 0.00f;
	#endregion

	#region Properties
	private ClassName _name;
	readonly Dictionary<Ability,float> _damageAbilityScales;
	readonly Dictionary<AbilityGroup,float> _damageAbilityGroupScales;
	readonly Dictionary<Resource,Dictionary<ResourceModifier,float>> _resourceModifiers;
	readonly Dictionary<Resource,Dictionary<Ability,float>> _resourceAbilityScales;
	#endregion

	public CharacterClassBuilder()
	{
		_damageAbilityScales = [];
		foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
			_damageAbilityScales.Add(ability,DefaultDamageAbilityScales);
		
		_damageAbilityGroupScales = [];
		foreach(AbilityGroup abilityGroup in Enum.GetValues(typeof(AbilityGroup)).Cast<AbilityGroup>())
			_damageAbilityGroupScales.Add(abilityGroup,DefaultDamageAbilityGroupScales);

		_resourceModifiers = [];
		foreach(Resource resource in Enum.GetValues(typeof(Resource)).Cast<Resource>())
		{
			Dictionary<ResourceModifier,float> _resourceEntry = [];
			foreach(ResourceModifier resourceModifier in Enum.GetValues(typeof(ResourceModifier)).Cast<ResourceModifier>())
			{
				_resourceEntry.Add(resourceModifier,DefaultResourceModifier);
			}
			_resourceModifiers.Add(resource,_resourceEntry);
		}

		_resourceAbilityScales = [];
		foreach(Resource resource in Enum.GetValues(typeof(Resource)).Cast<Resource>())
		{
			Dictionary<Ability,float> _resourceEntry = [];
			foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
			{
				_resourceEntry.Add(ability,DefaultResourceAbilityScales);
			}
			_resourceAbilityScales.Add(resource,_resourceEntry);
		}
	}

	public CharacterClass Build() => new(
		_name,
		(_damageAbilityGroupScales,_damageAbilityScales),
		_resourceModifiers,
		_resourceAbilityScales
	);

	public CharacterClassBuilder WithName(string name)
	{
		_name = Enum.Parse<ClassName>(name,true);
		return this;
	}

	public CharacterClassBuilder WithName(ClassName name)
	{
		_name = name;
		return this;
	}

	public CharacterClassBuilder WithDamageAbilityScales(float value)
	{
		foreach(Ability ability in _damageAbilityScales.Keys)
		{
			_damageAbilityScales[ability] = value;
		}
		return this;
	}

	public CharacterClassBuilder WithDamageAbilityScales(string ability,float value)
		=> WithDamageAbilityScales(Enum.Parse<Ability>(ability,true),value);

	public CharacterClassBuilder WithDamageAbilityScales(Ability ability,float value)
	{
		_damageAbilityScales[ability] = value;
		return this;
	}

	public CharacterClassBuilder WithDamageAbilityGroupScales(float value)
	{
		foreach(AbilityGroup abilityGroup in _damageAbilityGroupScales.Keys)
		{
			_damageAbilityGroupScales[abilityGroup] = value;
		}
		return this;
	}

	public CharacterClassBuilder WithDamageAbilityGroupScales(string abilityGroup,float value)
		=> WithDamageAbilityGroupScales(Enum.Parse<AbilityGroup>(abilityGroup,true),value);

	public CharacterClassBuilder WithDamageAbilityGroupScales(AbilityGroup abilityGroup,float value)
	{
		_damageAbilityGroupScales[abilityGroup] = value;
		return this;
	}

	public CharacterClassBuilder WithResourceModifier(Resource resource,ResourceModifier resourceModifier,float value)
	{
		_resourceModifiers[resource][resourceModifier]  = value;
		return this;
	}

	public CharacterClassBuilder WithResourceAbilityScales(Resource resource,Ability ability,float value)
	{
		_resourceAbilityScales[resource][ability]  = value;
		return this;
	}
}
