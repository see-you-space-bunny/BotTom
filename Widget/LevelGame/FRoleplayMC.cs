using System.Collections.Concurrent;
using System.ComponentModel;
using FChatApi.Attributes;
using RoleplayingGame.Attributes;
using RoleplayingGame.Enums;
using RoleplayingGame.Factories;
using RoleplayingGame.Interfaces;
using RoleplayingGame.Objects;
using RoleplayingGame.SheetComponents;
using FChatApi.Core;
using Plugins;
using Plugins.Interfaces;
using RoleplayingGame.Serialization;
using FChatApi.Objects;
using RoleplayingGame.Effects;
using Plugins.Tokenizer;
using Plugins.Core;
using RoleplayingGame.Systems;

namespace RoleplayingGame;

public partial class FRoleplayMC : FChatPlugin<RoleplayingGameCommand>, IFChatPlugin
{
    private static readonly string CsvDirectory;
	private static string _cacheURL					= "sessioncache";
	private static string _cardGameURI				= "cardgame";
	public static string CacheRoot { get; set; }	= Environment.CurrentDirectory;
	public static string CacheURL { get=>Path.Combine(CacheRoot,_cacheURL); set=>_cacheURL=value; }
	public static string RoleplayingGameURI { get => Path.Combine(CacheRoot,_cacheURL,_cardGameURI); set=>_cardGameURI=value; }

	internal static readonly Random Rng;
    internal static readonly ConcurrentDictionary<ClassName,CharacterClass> CharacterClasses;
	public static readonly ConcurrentDictionary<AttackType,AttackChassis> AttackPool;
    public static CharacterTracker Characters;
	internal static StatusEffectFactory StatusEffectFactory;
	internal static List<IPendingEvent> ActionQueue;


#region LoadAttacks
	public static void LoadAttacks(string filePath) => LoadAttacks(CsvDirectory,filePath);

	public static void LoadAttacks(string directory,string filePath)
	{
		foreach (AttackChassis chassis in DeserializeKommaVaues.GetAttacks(Path.Combine(directory,filePath)))
		{
			AttackPool.AddOrUpdate(chassis.AttackType,(k)=>chassis,(k,v)=>chassis);
		}
	}
#endregion
	

#region LoadClasses
	public static void LoadClasses(string filePath) => LoadClasses(CsvDirectory,filePath);

	public static void LoadClasses(string directory,string filePath)
	{
		foreach (CharacterClass @class in DeserializeKommaVaues.GetClasses(Path.Combine(directory,filePath)))
		{
			CharacterClasses.AddOrUpdate(@class.Name,(k)=>@class,(k,v)=>@class);
		}
	}
#endregion


#region Apply Effect
	public static void ApplyStatusEffect(StatusEffect statusEffect,User target,float intensity,User? source = null) =>
		ApplyStatusEffect(statusEffect,Characters.SingleByUser(target),intensity,source is null ? null : Characters.SingleByUser(source));

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
#endregion


#region Attributes
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
#endregion

#region Sanity Checks
	private static void DirectorySanityCheck()
	{
		if (!Directory.Exists(CsvDirectory))
			Directory.CreateDirectory(CsvDirectory);
	}
#endregion


#region IFChatPlugin
    void IFChatPlugin.HandleRecievedMessage(CommandTokens command)	=> HandleRecievedMessage(command);

    void IFChatPlugin.HandleJoinedChannel(ChannelEventArgs @event)	=> HandleJoinedChannel(@event);

    void IFChatPlugin.HandleCreatedChannel(ChannelEventArgs @event)	=> HandleCreatedChannel(@event);

    void IFChatPlugin.Update()		=> Update();
	
    void IFChatPlugin.Shutdown()	=> Shutdown();
#endregion


#region Constructor
    public FRoleplayMC(ApiConnection api, TimeSpan updateInterval) : base(api, updateInterval)
    { }

    static FRoleplayMC()
    {
		CharacterClasses	= [];
		AttackPool			= [];
		Characters			= null!;

		CsvDirectory		= Path.Combine(Environment.CurrentDirectory,"csv");
		Rng					= new Random();
		StatusEffectFactory	= new StatusEffectFactory();
		ActionQueue			= [];
		DirectorySanityCheck();
		PreProcessEnumAttributes();
		LoadClasses("CharacterClasses - Export.csv");
		LoadAttacks("CharacterClasses - Attacks.csv");
	}
#endregion
}