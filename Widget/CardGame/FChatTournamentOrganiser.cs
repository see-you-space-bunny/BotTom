using System.ComponentModel;

using FChatApi;
using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Attributes;
using FChatApi.Tokenizer;
using FChatApi.EventArguments;

using ModularPlugins;

using CardGame.Enums;
using CardGame.Commands;
using CardGame.Exceptions;
using CardGame.Attributes;
using CardGame.MatchEntities;
using CardGame.DataStructures;
using CardGame.PersistentEntities;

using ModularPlugins.Interfaces;
using System.Text.RegularExpressions;
using System.Reflection.PortableExecutable;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin, IFChatPlugin
{
	private const string GameChannelName = "ครtгคl ςђค๓קเ๏ภร";

	public Dictionary<User,PlayerCharacter	> PlayerCharacters { get; }

	public Dictionary<User,MatchChallenge	> IncomingChallenges { get; }
	public Dictionary<User,MatchChallenge	> OutgoingChallenges => IncomingChallenges.ToOutgoing();

	public List<BoardState> OngoingMatches { get; }

#if DEBUG
	public FChatMessageBuilder MostRecentMessage = null!; 
#endif

	public FChatTournamentOrganiser(ApiConnection api,TimeSpan updateInterval) : base(api,updateInterval)
	{
		PlayerCharacters		= [];
		IncomingChallenges      = [];
		OngoingMatches          = [];
	}

	private static void PreProcessEnumAttributes()
	{
		AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(CharacterStat));
		AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(CharacterStatGroup));
		AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(CardGameCommand));

		AttributeEnumExtensions.ProcessEnumForAttribute<StatAliasAttribute  >(typeof(CharacterStat));
		
		AttributeEnumExtensions.ProcessEnumForAttribute<StatGroupAttribute  >(typeof(CharacterStat));

		AttributeEnumExtensions.ProcessEnumForAttribute<StatDecorationAttribute  >(typeof(CharacterStat));
	}

	public override void HandleRecievedMessage(CommandTokens command)
	{
		FChatMessageBuilder messageBuilder = new FChatMessageBuilder()
			.WithAuthor(ApiConnection.CharacterName)
			.WithRecipient(command.Message.Author.Name)
			.WithChannel(command.Message.Channel);
		
		string message = ValidateCommandUse(command);

		if (!string.IsNullOrWhiteSpace(message))
			FChatApi
				.EnqueueMessage(
					messageBuilder
						.WithMessage(message)
				);

		////// DO STUFF HERE
		
		bool respondWithMessage = HandleValidatedCommand(messageBuilder,command);

		////////////////////

		if (respondWithMessage)
		{
#if DEBUG
			MostRecentMessage = messageBuilder;
#endif
			FChatApi.EnqueueMessage(messageBuilder);
		}
	}


	private string ValidateCommandUse(CommandTokens command)
	{
		try
		{
			// User may not be null
			if (command.Message.Author == null)
				throw new ArgumentNullException(nameof(command),$"The (BotCommand) User property may not be null when handling a {command.ModuleCommand} command.");

			return string.Empty;
		}
		catch (DisallowedCommandException disallowedCommand)
		{
			return disallowedCommand.Reason switch
			{
				CommandState.InsufficientPermission    => "You don't have permission to do that!",
				CommandState.ResponseRequired          => $"You need to respond to {IncomingChallenges[command.Message.Author].Challenger.Mention}'s challenge first!",
				_ => throw new ArgumentOutOfRangeException(command.ToString(), $"Unexpected {typeof(CommandState)} value: {disallowedCommand.Reason}")
			};
		}
	}

	public bool HandleValidatedCommand(FChatMessageBuilder messageBuilder,CommandTokens command)
	{
		if (command.TryParseCommand(out CardGameCommand moduleCommand))
		try
		{
			var alertTargetMessage = new FChatMessageBuilder()
				.WithAuthor(ApiConnection.CharacterName)
				.WithoutMention()
            	.WithMessageType(command.Message.Channel is not null ? FChatMessageType.Basic : FChatMessageType.Whisper);

			switch (moduleCommand)
			{
				case CardGameCommand.Summon:
				case CardGameCommand.Attack:
				case CardGameCommand.Special:
				case CardGameCommand.Target:
				case CardGameCommand.EndTurn:
					TakeGameAction(command,messageBuilder,moduleCommand);
					return true;

				case CardGameCommand.Challenge:
					if (IssueChallenge(command,messageBuilder,alertTargetMessage))
						FChatApi.EnqueueMessage(alertTargetMessage);
					return true;

				case CardGameCommand.Accept:
					if (AcceptChallenge(command,messageBuilder))
						return false;
					return true;

				case CardGameCommand.Reject:
					if (RejectChallenge(command,messageBuilder,alertTargetMessage))
						FChatApi.EnqueueMessage(alertTargetMessage);
					return true;

				case CardGameCommand.Set:
					return false;
				
				default:
					messageBuilder
						.WithRecipient(command.Message.Author.Name)
						.WithMention(command.Message.Author.Mention)
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
					CommandState.InsufficientPermission    => $"{command.Message.Author.Mention}, you don't have permission to do that!",
					CommandState.AwaitingResponse          => $"{command.Message.Author.Mention}, you still have a challenge!",
					CommandState.ResponseRequired          => $"{command.Message.Author.Mention}, you need to respond to {IncomingChallenges[command.Message.Author].Challenger.Mention}'s challenge first!",
					_ => throw new ArgumentOutOfRangeException(nameof(disallowedCommand.Reason), $"Unexpected {typeof(CommandState)} value: {disallowedCommand.Reason}")
				}
			);
		}
		return false;
	}

	public override void Update()
	{
		foreach(User key in OutgoingChallenges.Where((kvp)=>kvp.Value.AtTerminalStage).Select((kvp)=>kvp.Key))
			OutgoingChallenges.Remove(key);

		base.Update();
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


    void IFChatPlugin.HandleRecievedMessage(CommandTokens command) => HandleRecievedMessage(command);

    void IFChatPlugin.HandleJoinedChannel(ChannelEventArgs @event) => HandleJoinedChannel(@event);

    void IFChatPlugin.HandleCreatedChannel(ChannelEventArgs @event) => HandleCreatedChannel(@event);

	static FChatTournamentOrganiser()
	{
		PreProcessEnumAttributes();
	}
}