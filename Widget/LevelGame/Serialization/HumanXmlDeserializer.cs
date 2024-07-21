using System.Xml;
using System.Globalization;

using LevelGame.Enums;
using LevelGame.SheetComponents;
using FChatApi.Attributes;
using LevelGame.Attributes;

namespace LevelGame.Serialization;

public static class HumanXmlDeserializer
{
	#region Paths
	private const string CharacterClassXPath = "/RootElement/CharacterClass";
	private const string CurrentCharacterClassXPath = "/RootElement/CharacterClass[Name=\"{0}\"]";
	private const string NameAttribute			= "Name";
	private const string Abilities				= "descendant::Abilities";
	private const string AbilityScales			= "descendant::AbilityScales";
	private const string SkillActions			= "descendant::SkillActions";
	private const string SkillAction			= "descendant::SkillAction";
	private const string Limits					= "descendant::Limits";
	private const string BaseValueAttribute		= "BaseValue";
	private const string MinimumValueAttribute	= "MinimumValue";
	private const string SoftLimitAttribute		= "SoftLimit";
	private const string HardLimitAttribute		= "HardLimit";
	#endregion

	private static readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

	public static void /** ItemSettings */ GetItemSettings(string filePath) { }

	public static void /** FeatSettings */ GetFeatSettings(string filePath) { }

	public static IEnumerable<CharacterClass> GetClasses(string filePath)
	{
		XmlDocument xmlDocument = new();
		
		xmlDocument.Load(filePath);
				
		foreach(XmlNode characterClass in xmlDocument.SelectNodes(CharacterClassXPath)!)
		{
			if (characterClass.Attributes != null && characterClass.Attributes[NameAttribute] != null)
			{
				CharacterClassBuilder ccb = new();
				
				ccb.WithName(characterClass.Attributes[NameAttribute]!.Value);

				if (characterClass != null)
					GetResources(characterClass, ccb);

				yield return ccb.Build();
			}
		}
	}

	private static void GetResources(XmlNode characterClass, CharacterClassBuilder ccb)
	{
		GetResourceInfo(characterClass, ccb, Resource.Health);
		GetResourceInfo(characterClass, ccb, Resource.Protection);
		GetResourceInfo(characterClass, ccb, Resource.Evasion);
	}

	private static void GetResourceInfo(XmlNode characterClass, CharacterClassBuilder ccb, Resource resource)
	{
		XmlNode? resourceInfo = characterClass.SelectSingleNode(resource.GetEnumAttribute<Resource,XmlKeyAttribute>().Value);

		if (resourceInfo is null)
			return;
		
		if (resourceInfo.Attributes is not null && resourceInfo.Attributes[BaseValueAttribute] is not null)
			ccb.WithResourceModifier(resource,ResourceModifier.BaseValue,float.Parse(resourceInfo.Attributes[BaseValueAttribute]!.Value,culture));

		XmlNode? resourceLimits = resourceInfo.SelectSingleNode(Limits);
		if (resourceLimits != null)
			GetResourceLimits(resource, resourceLimits, ccb);

		XmlNode? healthAbilities = resourceInfo.SelectSingleNode(AbilityScales);
		if (healthAbilities != null)
			GetResourceAbilityScales(resource, healthAbilities, ccb);
	}

	private static void GetResourceAbilityScales(Resource resource,XmlNode resourceAbilities,CharacterClassBuilder ccb)
	{
		if (resourceAbilities.Attributes != null)
		{
			foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
				if (resourceAbilities.Attributes[ability.ToString()] != null)
					ccb.WithResourceAbilityScales(resource,ability,float.Parse(resourceAbilities.Attributes[ability.ToString()]!.Value,culture));
		}
	}

	private static void GetResourceLimits(Resource resource, XmlNode resourceLimits, CharacterClassBuilder ccb)
	{
		if (resourceLimits.Attributes != null)
		{
			if (resourceLimits.Attributes[MinimumValueAttribute] != null)
				ccb.WithResourceModifier(resource,ResourceModifier.MinimumValue,float.Parse(resourceLimits.Attributes[MinimumValueAttribute]!.Value,culture));

			if (resourceLimits.Attributes[SoftLimitAttribute] != null)
				ccb.WithResourceModifier(resource,ResourceModifier.SoftLimit,float.Parse(resourceLimits.Attributes[SoftLimitAttribute]!.Value,culture));

			if (resourceLimits.Attributes[HardLimitAttribute] != null)
				ccb.WithResourceModifier(resource,ResourceModifier.HardLimit,float.Parse(resourceLimits.Attributes[HardLimitAttribute]!.Value,culture));
		}

		XmlNode? resourceAbilityLimits = resourceLimits.SelectSingleNode(Abilities);
		if (resourceAbilityLimits != null)
			GetResourceAbilityLimits(resource, resourceAbilityLimits, ccb);
	}

	private static void GetResourceAbilityLimits(Resource resource, XmlNode resourceAbilityLimits, CharacterClassBuilder ccb)
	{
		if (resourceAbilityLimits.Attributes != null)
		{
			/** TODO: Add "health ability limits" to CharacterClass / CharacterClassBuilder
			foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
				if (healthAbilities.Attributes[ability.ToString()] != null)
					ccb.WithHealthAbilityLimit(ability,float.Parse(healthAbilities.Attributes[ability.ToString()]!.Value,culture));
			*/
		}
	}
}