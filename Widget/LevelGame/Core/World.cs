using System.Collections.Concurrent;
using System.ComponentModel;
using FChatApi.Attributes;
using FChatApi.Objects;
using LevelGame.Attributes;
using LevelGame.Enums;
using LevelGame.Objects;
using LevelGame.Serialization;
using LevelGame.SheetComponents;

namespace LevelGame.Core;

public static class World
{
    private static readonly string CsvDirectory;
	internal static readonly Random Rng;
    internal static readonly ConcurrentDictionary<ClassName,CharacterClass> CharacterClasses;
    internal static readonly ConcurrentDictionary<string,CharacterSheet> CharacterSheets;


	public static void LoadClasses(string filePath) => LoadClasses(CsvDirectory,filePath);

	public static void LoadClasses(string directory,string filePath)
	{
		foreach (CharacterClass @class in DeserializeKommaVaues.GetClasses(Path.Combine(directory,filePath)))
		{
			CharacterClasses.AddOrUpdate(@class.Name,(k)=>@class,(k,v)=>@class);
		}
	}

	private static void DirectorySanityCheck()
	{
		if (!Directory.Exists(CsvDirectory))
			Directory.CreateDirectory(CsvDirectory);
	}

	public static void ApplyStatusEffect(User target,StatusEffect statusEffect,float intensity,User? source)
	{
		CharacterSheets[target.Name].ApplyStatusEffect(statusEffect,intensity,source is null ? null : CharacterSheets[source.Name]);
	}

	static World()
	{
		AttributeExtensions.ProcessEnumForAttribute<AbilityInfoAttribute		>(typeof(Ability));
		AttributeExtensions.ProcessEnumForAttribute<DescriptionAttribute		>(typeof(Ability));
		AttributeExtensions.ProcessEnumForAttribute<ShortFormAttribute			>(typeof(Ability));

		AttributeExtensions.ProcessEnumForAttribute<DescriptionAttribute		>(typeof(Archetype));

		AttributeExtensions.ProcessEnumForAttribute<DerivedAbilityInfoAttribute	>(typeof(DerivedAbility));
		AttributeExtensions.ProcessEnumForAttribute<DescriptionAttribute		>(typeof(DerivedAbility));
		AttributeExtensions.ProcessEnumForAttribute<ShortFormAttribute			>(typeof(DerivedAbility));

		AttributeExtensions.ProcessEnumForAttribute<DescriptionAttribute		>(typeof(Echelon));
		AttributeExtensions.ProcessEnumForAttribute<EchelonPropertiesAttribute	>(typeof(Echelon));

		AttributeExtensions.ProcessEnumForAttribute<ActionPropertiesAttribute	>(typeof(GameAction));
		AttributeExtensions.ProcessEnumForAttribute<ActionDefaultValuesAttribute>(typeof(GameAction));

		AttributeExtensions.ProcessEnumForAttribute<XmlKeyAttribute				>(typeof(Resource));
		AttributeExtensions.ProcessEnumForAttribute<GameFlagsAttribute			>(typeof(Resource));

		AttributeExtensions.ProcessEnumForAttribute<DefaultModifierAttribute	>(typeof(ResourceModifier));

		AttributeExtensions.ProcessEnumForAttribute<DescriptionAttribute		>(typeof(StatusEffect));

		CharacterClasses 	= [];
		CharacterSheets		= new ConcurrentDictionary<string, CharacterSheet>(StringComparer.InvariantCultureIgnoreCase);
		CsvDirectory = Path.Combine(Environment.CurrentDirectory,"csv");
		Rng = new Random();
		DirectorySanityCheck();
	}
}
