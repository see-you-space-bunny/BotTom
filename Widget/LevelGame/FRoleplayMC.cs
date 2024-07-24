using System.Collections.Concurrent;
using System.ComponentModel;
using FChatApi.Attributes;
using LevelGame.Attributes;
using LevelGame.Enums;
using LevelGame.Factories;
using LevelGame.Interfaces;
using LevelGame.Objects;
using LevelGame.SheetComponents;
using FChatApi.Core;
using ModularPlugins;
using ModularPlugins.Interfaces;
using LevelGame.Serialization;
using FChatApi.Objects;

namespace LevelGame;

public partial class FRoleplayMC : FChatPlugin<LevelGameCommand>, IFChatPlugin
{
    private static readonly string CsvDirectory;
	internal static readonly Random Rng;
    internal static readonly ConcurrentDictionary<ClassName,CharacterClass> CharacterClasses;
    internal static readonly ConcurrentDictionary<string,CharacterSheet> CharacterSheets;
	internal static StatusEffectFactory StatusEffectFactory;
	internal static List<IPendingAction> ActionQueue;


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

	public static void ApplyStatusEffect(StatusEffect statusEffect,User target,float intensity,User? source = null) =>
		ApplyStatusEffect(statusEffect,CharacterSheets[target.Name],intensity,source is null ? null : CharacterSheets[source.Name]);

	public static void ApplyStatusEffect(StatusEffect statusEffect,Actor target,float intensity,Actor? source = null)
	{
		target
			.ApplyStatusEffect(StatusEffectFactory
				.CreateStatusEffect(
					statusEffect,
					target,
					intensity,
					source
				)
			);
	}

    public FRoleplayMC(ApiConnection api, TimeSpan updateInterval) : base(api, updateInterval)
    {
		RegisterCommandRestrictions();
	}

    static FRoleplayMC()
    {
		CharacterClasses	= [];
		CharacterSheets		= new ConcurrentDictionary<string, CharacterSheet>(StringComparer.InvariantCultureIgnoreCase);
		CsvDirectory		= Path.Combine(Environment.CurrentDirectory,"csv");
		Rng					= new Random();
		StatusEffectFactory	= new StatusEffectFactory();
		ActionQueue			= [];
		DirectorySanityCheck();
		PreProcessEnumAttributes();
	}

	private void RegisterCommandRestrictions()
	{
		//ChannelLockedCommands.Add(LevelGameCommand);
		//WhispersLockedCommands.Add(LevelGameCommand);
	}

	private static void PreProcessEnumAttributes()
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
	}
}