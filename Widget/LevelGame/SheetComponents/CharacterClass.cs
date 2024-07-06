using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Widget.LevelGame.Enums;

namespace Widget.LevelGame.SheetComponents;

public class CharacterClass(
		ClassName name,
		(Dictionary<AbilityGroup,float> AbilityGroup, Dictionary<Ability,float> Ability) damageMultipliers,
		Dictionary<Resource,Dictionary<ResourceModifier,float>> resourceModifiers,
		Dictionary<Resource,Dictionary<Ability,float>> resourceAbilityScales
)
{
	#region Properties
	public ClassName Name { get; } = name;
	public string TextName => Name.ToString();
	public (Dictionary<AbilityGroup,float> AbilityGroup, Dictionary<Ability,float> Ability) DamageMultipliers { get; } = damageMultipliers;
	public Dictionary<Resource,Dictionary<ResourceModifier,float>> ResourceModifiers { get; } = resourceModifiers;
	public Dictionary<Resource,Dictionary<Ability,float>> ResourceAbilityScales { get; } = resourceAbilityScales;
	#endregion
}