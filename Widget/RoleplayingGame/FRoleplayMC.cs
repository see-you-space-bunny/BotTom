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
#region (-) Serialization
    private static readonly string _csvDirectory;
	private static string _cacheURL;
	private static string _cardGameURI;
#endregion


#region (+) Serialization
	public string CacheRoot { get; set; }	= Environment.CurrentDirectory;
	public string CacheURL { get=>Path.Combine(CacheRoot,_cacheURL); set=>_cacheURL=value; }
	public string RoleplayingGameURI { get => Path.Combine(CacheRoot,_cacheURL,_cardGameURI); set=>_cardGameURI=value; }
#endregion


#region (~) Fields
    internal static readonly ConcurrentDictionary<ClassName,CharacterClass> CharacterClasses;
	internal static readonly ConcurrentDictionary<AttackType,AttackChassis> AttackPool;
#endregion


#region (~) Fields
	internal readonly StatusEffectFactory StatusEffectFactory;
	internal readonly List<IPendingEvent> ActionQueue;
#endregion


#region (~) Properties
    internal CharacterTracker Characters { get; private set; }
    internal InteractionTracker Interactions { get; private set; }
    internal DieRoller DieRoller { get; private set; }
#endregion


#region LoadAttacks
	public void LoadAttacks(string filePath) => LoadAttacks(_csvDirectory,filePath);

	public void LoadAttacks(string directory,string filePath)
	{
		foreach (AttackChassis chassis in DeserializeKommaVaues.GetAttacks(Path.Combine(directory,filePath)))
		{
			AttackPool.AddOrUpdate(chassis.AttackType,(k)=>chassis,(k,v)=>chassis);
		}
	}
#endregion
	

#region LoadClasses
	public void LoadClasses(string filePath) => LoadClasses(_csvDirectory,filePath);

	public void LoadClasses(string directory,string filePath)
	{
		foreach (CharacterClass @class in DeserializeKommaVaues.GetClasses(Path.Combine(directory,filePath)))
		{
			CharacterClasses.AddOrUpdate(@class.Name,(k)=>@class,(k,v)=>@class);
		}
	}
#endregion


#region Apply Effect
	public void ApplyStatusEffect(StatusEffect statusEffect,User target,float intensity,User? source = null) =>
		ApplyStatusEffect(statusEffect,Characters.SingleByUser(target),intensity,source is null ? null : Characters.SingleByUser(source));

	public void ApplyStatusEffect(StatusEffect statusEffect,Actor target,float intensity,Actor? source = null)
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


#region (-) Sanity Checks
	private static void DirectorySanityCheck()
	{
		if (!Directory.Exists(_csvDirectory))
			Directory.CreateDirectory(_csvDirectory);
	}
#endregion


#region (+) IFChatPlugin
    void IFChatPlugin.HandleRecievedMessage(CommandTokens command)	=> HandleRecievedMessage(command);

    void IFChatPlugin.HandleJoinedChannel(ChannelEventArgs @event)	=> HandleJoinedChannel(@event);

    void IFChatPlugin.HandleCreatedChannel(ChannelEventArgs @event)	=> HandleCreatedChannel(@event);

    void IFChatPlugin.Update()		=> Update();
	
    void IFChatPlugin.Shutdown()	=> Shutdown();
#endregion


#region Constructor
    public FRoleplayMC(ApiConnection api, TimeSpan updateInterval) : base(api, updateInterval)
    {
		ActionQueue			= [];
		Characters			= new CharacterTracker();
		Interactions		= new InteractionTracker();
		DieRoller			= new DieRoller();
		StatusEffectFactory	= new StatusEffectFactory();
		LoadClasses("CharacterClasses - Export.csv");
		LoadAttacks("CharacterClasses - Attacks.csv");
	}

    static FRoleplayMC()
    {
		AttackPool			= [];
		CharacterClasses	= [];
		_cacheURL		= "sessioncache";
		_cardGameURI	= "cardgame";
		_csvDirectory	= Path.Combine(Environment.CurrentDirectory,"csv");
		DirectorySanityCheck();
		PreProcessEnumAttributes();
	}
#endregion
}