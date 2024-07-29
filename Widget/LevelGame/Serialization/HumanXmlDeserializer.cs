using System.Xml;
using System.Globalization;

using RoleplayingGame.Enums;
using RoleplayingGame.SheetComponents;
using FChatApi.Attributes;
using RoleplayingGame.Attributes;

namespace RoleplayingGame.Serialization;

/**
public static class HumanXmlDeserializer
{
	#region Paths
	private const string XPathCharacterClass = "/RootElement/CharacterClass";
	private const string CurrentCharacterClassXPath = "/RootElement/CharacterClass[Name=\"{0}\"]";
	private const string NameAttribute			= "Name";
	private const string AbilityScales			= "descendant::AbilityScales";
	private const string SkillActions			= "descendant::SkillActions";
	private const string SkillAction			= "descendant::SkillAction";
	
	/////
	private const string XPathResources			= "descendant::Resources";
	private const string XPathResource			= "descendant::Resources/Resource[@Name='{0}']";
	
	/////
	private const string XPathAbilities			= "descendant::Abilities";
	private const string XPathAbility			= "descendant::Abilities/Ability[@Name='{0}']";
	private const string XPathAbilityGroup		= "descendant::Abilities/Group[@Name='{0}']";
	private const string XPathAbilityAll		= "descendant::Abilities/All";
	
	/////
	private const string BaseValueAttribute		= "BaseValue";
	private const string MinimumValueAttribute	= "MinimumValue";

	/////
	private const string Limits					= "descendant::Limits";
	private static readonly ResourceModifier[] LimitAttributes	= [	ResourceModifier.BaseValue,
																	ResourceModifier.BaseGrowth,
																	ResourceModifier.MinimumValue,
																	ResourceModifier.SoftLimit,
																	ResourceModifier.HardLimit
																];
	private const string SoftLimitAttribute		= "SoftLimit";
	private const string HardLimitAttribute		= "HardLimit";
	#endregion

	private static readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

	public static void /** ItemSettings / GetItemSettings(string filePath) { }

	public static void /** FeatSettings / GetFeatSettings(string filePath) { }

	public static IEnumerable<CharacterClass> GetClasses(string filePath)
	{
		XmlDocument xmlDocument = new();
		
		xmlDocument.Load(filePath);
				
		foreach(XmlNode characterClass in xmlDocument.SelectNodes(XPathCharacterClass)!)
		{
			if (characterClass.Attributes is not null && characterClass.Attributes[NameAttribute] is not null)
			{
				CharacterClassBuilder ccb = new();
				
				ccb.WithName(characterClass.Attributes[NameAttribute]!.Value);

				if (characterClass is not null)
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
		if (!TrySelectSingleNode(characterClass,string.Format(XPathResource,resource.ToString()),out XmlNode resourceInfo))
			return;
			
		foreach (ResourceModifier modifier in LimitAttributes)
		{
			if (resourceInfo.Attributes![modifier.ToString()] is not null)
			{
				ccb.WithResourceModifier(resource,modifier,float.Parse(resourceInfo.Attributes[modifier.ToString()]!.Value,culture));
			}
		}

		if (TrySelectSingleNode(resourceInfo,Limits,out XmlNode resourceLimits))
		{
			GetResourceLimits(resource, resourceLimits, ccb);
		}

		if (TrySelectSingleNode(resourceInfo,AbilityScales,out XmlNode resourceAbilities))
		{
			GetResourceAbilityScales(resource, resourceAbilities, ccb);
		}
	}

	private static void GetResourceAbilityScales(Resource resource,XmlNode resourceAbilities,CharacterClassBuilder ccb)
	{
		if (resourceAbilities.Attributes is null)
			return;
		
		foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>().Where(a=>a != Ability.None && a != Ability.Level))
			if (resourceAbilities.Attributes[ability.ToString()] is not null)
				ccb.WithResourceAbilityScales(resource,ability,float.Parse(resourceAbilities.Attributes[ability.ToString()]!.Value,culture));
	}

	private static void GetResourceLimits(Resource resource, XmlNode resourceLimits, CharacterClassBuilder ccb)
	{
		if (TrySelectSingleNode(resourceLimits,XPathAbilities,out XmlNode resourceAbilityLimits))
		{
			GetResourceAbilityLimits(resource, resourceAbilityLimits, ccb);
		}

		if (resourceLimits.Attributes is null)
			return;

		foreach (ResourceModifier modifier in LimitAttributes)
		{
			if (resourceLimits.Attributes[modifier.ToString()] is not null)
			{
				ccb.WithResourceModifier(resource,modifier,float.Parse(resourceLimits.Attributes[modifier.ToString()]!.Value,culture));
			}
		}
	}

	private static void GetResourceAbilityLimits(Resource resource, XmlNode resourceAbilityLimits, CharacterClassBuilder ccb)
	{
		if (resourceAbilityLimits.Attributes is null)
			return;
		/**
		 TODO: Add "health ability limits" to CharacterClass / CharacterClassBuilder
		foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
			if (healthAbilities.Attributes[ability.ToString()] != null)
				ccb.WithHealthAbilityLimit(ability,float.Parse(healthAbilities.Attributes[ability.ToString()]!.Value,culture));
		/
	}

	private static bool TrySelectSingleNode(XmlNode node,string xpath,out XmlNode xmlNode)
	{
		xmlNode = node.SelectSingleNode(xpath)!;
		if (xmlNode is null)
			return false;
		return true;
	}
}
*/