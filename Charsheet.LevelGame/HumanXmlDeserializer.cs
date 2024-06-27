using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Charsheet.LevelGame.Enums;
using Charsheet.LevelGame.SheetComponents;

namespace Charsheet.LevelGame;

internal static class HumanXmlDeserializer
{
    #region 
    private const string CharacterClassXPath = "/RootElement/CharacterClass";
    private const string CurrentCharacterClassXPath = "/RootElement/CharacterClass[Name={0}]";
    private const string NameAttribute = "Name";
    private const string Resources = "descendant::Resources";
    private const string HitPoints = "descendant::HitPoints";
    private const string Abilities = "descendant::Abilities";
    private const string AbilityScales = "descendant::AbilityScales";
    private const string SkillActions = "descendant::SkillActions";
    private const string SkillAction = "descendant::SkillAction";
    private const string Limits = "descendant::Limits";
    private const string BaseValueAttribute = "BaseValue";
    private const string SoftLimitAttribute = "SoftLimit";
    private const string HardLimitAttribute = "HardLimit";
    #endregion

    public static void /** ItemSettings */ GetItemSettings(string filePath) { }

    public static void /** FeatSettings */ GetFeatSettings(string filePath) { }

    public static IEnumerable<CharacterClass> GetClasses(string filePath)
    {
        XmlDocument xmlDocument = new();
        xmlDocument.Load(filePath);
                
        foreach(ClassName className in Enum.GetValues(typeof(ClassName)).Cast<ClassName>())
        {
            string currentCharacterClassXPath = string.Format(CurrentCharacterClassXPath,className.ToString());
            CharacterClassBuilder ccb = new();

            XmlNode characterClass = xmlDocument.SelectSingleNode(currentCharacterClassXPath)!;
            
            ccb.WithName(className);
            GetResources(characterClass, ccb);

            yield return ccb.Build();
        }
    }

    private static void GetResources(XmlNode characterClass, CharacterClassBuilder ccb)
    {
        XmlNode? resources = characterClass.SelectSingleNode(Resources);

        if (resources != null)
        {
            GetHealthPoints(resources, ccb);
        }
    }

    private static void GetHealthPoints(XmlNode resources, CharacterClassBuilder ccb)
    {
        XmlNode? healthPoints = resources.SelectSingleNode(HitPoints);

        if (healthPoints != null)
        {
            if (healthPoints.Attributes != null)
            {
                if (healthPoints.Attributes[BaseValueAttribute] != null)
                    ccb.WithResourceModifier(Resource.HealthPoints,ResourceModifier.BaseValue,float.Parse(healthPoints.Attributes[BaseValueAttribute]!.Value));
            }

            XmlNode? healthLimits = healthPoints.SelectSingleNode(Limits);
            if (healthLimits != null)
                GetResourceLimits(Resource.HealthPoints, healthLimits, ccb);

            XmlNode? healthAbilities = healthPoints.SelectSingleNode(AbilityScales);
            if (healthAbilities != null)
                GetResourceAbilityScales(Resource.HealthPoints, healthAbilities, ccb);
        }
    }

    private static void GetResourceAbilityScales(Resource resource,XmlNode resourceAbilities,CharacterClassBuilder ccb)
    {
        if (resourceAbilities.Attributes != null)
        {
            foreach(Ability ability in Enum.GetValues(typeof(Ability)).Cast<Ability>())
                if (resourceAbilities.Attributes[ability.ToString()] != null)
                    ccb.WithResourceAbilityScales(resource,ability,float.Parse(resourceAbilities.Attributes[ability.ToString()]!.Value));
        }
    }

    private static void GetResourceLimits(Resource resource, XmlNode resourceLimits, CharacterClassBuilder ccb)
    {
        if (resourceLimits.Attributes != null)
        {
            if (resourceLimits.Attributes[SoftLimitAttribute] != null)
                ccb.WithResourceModifier(resource,ResourceModifier.SoftLimit,float.Parse(resourceLimits.Attributes[SoftLimitAttribute]!.Value));

            if (resourceLimits.Attributes[HardLimitAttribute] != null)
                ccb.WithResourceModifier(resource,ResourceModifier.HardLimit,float.Parse(resourceLimits.Attributes[HardLimitAttribute]!.Value));
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
                    ccb.WithHealthAbilityLimit(ability,float.Parse(healthAbilities.Attributes[ability.ToString()]!.Value));
            */
        }
    }
}