using System.ComponentModel;
using FChatApi.Attributes;
using LevelGame.Enums;

namespace LevelGame.SheetComponents;

public class CharacterClass
{
	#region Properties
	public ClassName Name { get; }
	public Dictionary<Resource,Dictionary<ResourceModifier,float>> ResourceModifiers { get; }
	public Dictionary<Resource,Dictionary<Ability,float>> ResourceAbilityScales { get; }
	public Dictionary<Ability,float> AbilityGrowth { get; }
    #endregion

	public CharacterClass(
		ClassName name,
		Dictionary<Resource,Dictionary<ResourceModifier,float>> resourceModifiers,
		Dictionary<Resource,Dictionary<Ability,float>> resourceAbilityScales,
		Dictionary<Ability,float> abilityGrowth
	)
	{
		Name					= name;
		AbilityGrowth			= abilityGrowth;
		ResourceAbilityScales	= resourceAbilityScales;
		ResourceModifiers		= resourceModifiers;
	}
	
    public override string ToString()
    {
        return Name.ToString();
	}
	
    public string ToStringDescription()
    {
        return string.Format(
			"[b][u]{0}[/u][/b]\n{1}\n[sub]Combined Growth: {2}[/sub]",
			Name.ToString(),
			Name.GetEnumAttribute<ClassName,DescriptionAttribute>().Description,
			0.0f);
	}
}