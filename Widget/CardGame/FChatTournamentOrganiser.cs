using System.ComponentModel;

using FChatApi;
using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Attributes;
using Plugins.Tokenizer;

using Plugins;

using CardGame.Enums;
using CardGame.Commands;
using CardGame.Exceptions;
using CardGame.Attributes;
using CardGame.MatchEntities;
using CardGame.DataStructures;
using CardGame.PersistentEntities;

using Plugins.Interfaces;
using Plugins.Attributes;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin<CardGameCommand>, IFChatPlugin
{
	private const string GameChannelName = "ครtгคl ςђค๓קเ๏ภร";
	private static string _cacheURL					= "sessioncache";
	private static string _cardGameURI				= "cardgame";
	public static string CacheRoot { get; set; }	= Environment.CurrentDirectory;
	public static string CacheURL { get=>Path.Combine(CacheRoot,_cacheURL); set=>_cacheURL=value; }
	public static string CacheCardGameURI { get => Path.Combine(CacheRoot,_cacheURL,_cardGameURI); set=>_cardGameURI=value; }

	public Dictionary<User,PlayerCharacter	> PlayerCharacters { get; }
	public Dictionary<string,PlayerCharacter> OrphanCharacters { get; }

	public Dictionary<User,MatchChallenge	> IncomingChallenges { get; }
	public Dictionary<User,MatchChallenge	> OutgoingChallenges => IncomingChallenges.ToOutgoing();

	public List<BoardState> OngoingMatches { get; }

#if DEBUG
	public FChatMessageBuilder MostRecentMessage = null!; 
#endif

	public FChatTournamentOrganiser(ApiConnection api,TimeSpan updateInterval) : base(api,updateInterval)
	{
		PlayerCharacters		= [];
		OrphanCharacters		= [];
		IncomingChallenges      = [];
		OngoingMatches          = [];
	}

	private static void PreProcessEnumAttributes()
	{
		AttributeExtensions.ProcessEnumForAttribute<DescriptionAttribute	>(typeof(CharacterStat));
		AttributeExtensions.ProcessEnumForAttribute<DescriptionAttribute	>(typeof(CharacterStatGroup));
		AttributeExtensions.ProcessEnumForAttribute<DescriptionAttribute	>(typeof(CardGameCommand));

		AttributeExtensions.ProcessEnumForAttribute<StatAliasAttribute  	>(typeof(CharacterStat));
			
		AttributeExtensions.ProcessEnumForAttribute<StatGroupAttribute  	>(typeof(CharacterStat));

		AttributeExtensions.ProcessEnumForAttribute<StatDecorationAttribute	>(typeof(CharacterStat));
	}

	public override void HandleRecievedMessage(CommandTokens commandTokens)
	{
		if (HandleCommand(commandTokens,out var messageBuilder))
		{
#if DEBUG
			MostRecentMessage = messageBuilder;
#endif
			FChatApi.EnqueueMessage(messageBuilder);
		}
	}

	public bool HandleCommand(CommandTokens commandTokens,out FChatMessageBuilder messageBuilder)
	{
		messageBuilder = new FChatMessageBuilder();
		//////////////
		
		if (!commandTokens.TryGetParameters(out CardGameCommand command))
			return false;
		
		if (command.HasEnumAttribute<CardGameCommand,MinimumPrivilegeAttribute>() &&
			commandTokens.Source.Author.PrivilegeLevel < command.GetEnumAttribute<CardGameCommand,MinimumPrivilegeAttribute>().Privilege)
			return false;
		
		AdoptOrCreateCharacter(commandTokens.Source.Author);
			
		//////////////
		try
		{
			var alertTargetMessage = new FChatMessageBuilder()
				.WithAuthor(ApiConnection.ApiUser)
				.WithRecipient(commandTokens.Source.Author)
				.WithChannel(commandTokens.Source.Channel)
            	.WithMessageType(commandTokens.Source.Channel is not null ? FChatMessageType.Basic : FChatMessageType.Whisper);

			switch (command)
			{
				case CardGameCommand.Summon:
				case CardGameCommand.Attack:
				case CardGameCommand.Special:
				case CardGameCommand.Target:
				case CardGameCommand.EndTurn:
					TakeGameAction(commandTokens,messageBuilder,command);
					return true;

				case CardGameCommand.Challenge:
					if (IssueChallenge(commandTokens,messageBuilder,alertTargetMessage))
							FChatApi.EnqueueMessage(alertTargetMessage);
					return true;

				case CardGameCommand.Accept:
					if (AcceptChallenge(commandTokens,messageBuilder))
						return false;
					return true;

				case CardGameCommand.Reject:
					if (RejectChallenge(commandTokens,messageBuilder,alertTargetMessage))
							FChatApi.EnqueueMessage(alertTargetMessage);
					return true;

				case CardGameCommand.CgImportStats:
					ImportStats(commandTokens,messageBuilder);
					return false;
				
				default:
					messageBuilder
						.WithRecipient(commandTokens.Source.Author.Name)
						.WithMention(commandTokens.Source.Author.Mention)
						.WithMessage("--> That is not a valid command!");
					return true;
			}
		}
		catch (InvalidOperationException e)
		{
			Console.WriteLine(e.InnerException);
			Console.WriteLine(e.StackTrace);
			Console.WriteLine();
			Console.WriteLine(e.Message);
		}
		catch (InvalidCommandSyntaxException e)
		{
			messageBuilder.WithMessage(e.Message);
		}
		catch (DisallowedCommandException disallowedCommand)
		{
			messageBuilder.WithMessage(
				disallowedCommand.Reason switch
				{
					CommandState.InsufficientPermission    => $"{commandTokens.Source.Author.Mention}, you don't have permission to do that!",
					CommandState.AwaitingResponse          => $"{commandTokens.Source.Author.Mention}, you still have a challenge!",
					CommandState.ResponseRequired          => $"{commandTokens.Source.Author.Mention}, you need to respond to {IncomingChallenges[commandTokens.Source.Author].Challenger.Mention}'s challenge first!",
					_ => throw new ArgumentOutOfRangeException(nameof(disallowedCommand.Reason), $"Unexpected {typeof(CommandState)} value: {disallowedCommand.Reason}")
				}
			);
		}
		return false;
	}

	private void AdoptOrCreateCharacter(User user)
	{
		if (!PlayerCharacters.ContainsKey(user))
		{
			if (OrphanCharacters.ContainsKey(user.Name))
			{
				PlayerCharacters.Add(user,OrphanCharacters[user.Name]);
			}
			else
			{
				PlayerCharacters.Add(user,new PlayerCharacter(user));
			}
		}
	}

	public override void Initialize()
	{
		if (!File.Exists(CacheCardGameURI))
			return;

		using (var reader = new BinaryReader(File.OpenRead(CacheCardGameURI)))
		for (int n=0;n<2;n++)
			for (int i=0;i<reader.ReadUInt32();i++)
			{
				var playerCharacter = PlayerCharacter.Deserialize(reader);
				if (ApiConnection.Users.TrySingleByName(playerCharacter.Key,out User user))
				{
					PlayerCharacters.Add(user,playerCharacter);
				}
				else
				{
					OrphanCharacters.Add(playerCharacter.Key,playerCharacter);
				}
			}
		base.Shutdown();
	}

	public override void Update()
	{
		foreach(User key in OutgoingChallenges.Where((kvp)=>kvp.Value.AtTerminalStage).Select((kvp)=>kvp.Key))
			OutgoingChallenges.Remove(key);

		base.Update();
	}

	public override void Shutdown()
	{
		if (!Directory.Exists(CacheURL))
			Directory.CreateDirectory(CacheURL);

		if (PlayerCharacters.Count == 0)
		{
			File.Delete(CacheCardGameURI);
			return;
		}

		using (var writer = new BinaryWriter(File.OpenWrite(CacheCardGameURI)))
		{
			writer.Write((uint)	PlayerCharacters.Count);
			foreach (PlayerCharacter playerCharacter in PlayerCharacters.Values)
			{
				playerCharacter.Serialize(writer);
			}
			writer.Write((uint)	OrphanCharacters.Count);
			foreach (PlayerCharacter orphanCharacter in OrphanCharacters.Values)
			{
				orphanCharacter.Serialize(writer);
			}
		}
		base.Shutdown();
	}

	public override void HandleJoinedChannel(ChannelEventArgs @event)
	{
		if (@event.Channel.Name != GameChannelName)
			return;

		ActiveChannels.TryAdd(@event.Channel.Code,@event.Channel);

		var match = OngoingMatches.FirstOrDefault(m => m.Channel == @event.Channel);
		if (match != default)
		{
			if (@event.User == match.Player1.User || @event.User == match.Player2.User)
				ApiConnection.Mod_SetChannelUserStatus(@event.Channel,@event.User,UserRoomStatus.Moderator);
		
			if (match.IsGameChannelValid() && !match.WelcomeMessageSent)
				match.SendWelcomeMessage(FChatApi);
		}
	}

	public override void HandleCreatedChannel(ChannelEventArgs @event)
	{
		if (@event.Channel.Name != GameChannelName)
			return;

		if (OngoingMatches.Any(m=>m.AwaitingChannel))
		{
			FinalizeMatchCreation(@event.Channel);
		}
	}


#region IFChatPlugin
    void IFChatPlugin.HandleRecievedMessage(CommandTokens command) => HandleRecievedMessage(command);

    void IFChatPlugin.HandleJoinedChannel(ChannelEventArgs @event) => HandleJoinedChannel(@event);

    void IFChatPlugin.HandleCreatedChannel(ChannelEventArgs @event) => HandleCreatedChannel(@event);

    void IFChatPlugin.Initialize() => Initialize();

    void IFChatPlugin.Update() => Update();
	
    void IFChatPlugin.Shutdown() => Shutdown();
#endregion

	static FChatTournamentOrganiser()
	{
		PreProcessEnumAttributes();
	}
}