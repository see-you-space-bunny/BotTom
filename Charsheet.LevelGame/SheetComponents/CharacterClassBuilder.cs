using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charsheet.LevelGame.Enums;

namespace Charsheet.LevelGame.SheetComponents;

public class CharacterClassBuilder
{
  #region Properties
  private ClassName _name;
  readonly Dictionary<Ability,float> _healthPointAbilityScales;
  readonly Dictionary<Ability,float> _damageAbilityScales;
  readonly Dictionary<AbilityGroup,float> _damageAbilityGroupScales;
  readonly Dictionary<Resource,Dictionary<ResourceModifier,float>> _resourceModifiers;
  readonly Dictionary<Resource,Dictionary<Ability,float>> _resourceAbilityScales;
  #endregion

  public CharacterClassBuilder()
  {
    _healthPointAbilityScales = [];
    foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
      _healthPointAbilityScales.Add(ability,0.00f);

    _damageAbilityScales = [];
    foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
      _damageAbilityScales.Add(ability,1.00f);
    

    _damageAbilityGroupScales = [];
    foreach(AbilityGroup abilityGroup in Enum.GetValues(typeof(AbilityGroup)).Cast<AbilityGroup>())
      _damageAbilityGroupScales.Add(abilityGroup,1.00f);

    _resourceModifiers = [];
    foreach(Resource resource in Enum.GetValues(typeof(Resource)).Cast<Resource>())
    {
      Dictionary<ResourceModifier,float> _resourceEntry = [];
      foreach(ResourceModifier resourceMultiplier in Enum.GetValues(typeof(ResourceModifier)).Cast<ResourceModifier>())
      {
        _resourceEntry.Add(resourceMultiplier,1.00f);
      }
      _resourceModifiers.Add(resource,_resourceEntry);
    }

    _resourceAbilityScales = [];
    foreach(Resource resource in Enum.GetValues(typeof(Resource)).Cast<Resource>())
    {
      Dictionary<Ability,float> _resourceEntry = [];
      foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
      {
        _resourceEntry.Add(ability,1.00f);
      }
      _resourceAbilityScales.Add(resource,_resourceEntry);
    }

    #region Specific Defaults
    _healthPointAbilityScales[Ability.Power     ] = 0.15f;
    _healthPointAbilityScales[Ability.Body      ] = 0.80f;
    _healthPointAbilityScales[Ability.Reflex    ] = 0.05f;
    _healthPointAbilityScales[Ability.Focus     ] = 0.30f;
    _healthPointAbilityScales[Ability.Will      ] = 0.50f;
    _healthPointAbilityScales[Ability.Presence  ] = 0.20f;
    #endregion
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
