using System.ComponentModel;
using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Enums;
using CardGame.Enums;
using CardGame.MatchEntities;
using CardGame.Commands;
using CardGame.PersistentEntities;
using CardGame.DataStructures;
using CardGame.Exceptions;
using CardGame.Attributes;
using FChatApi.Attributes;
using FChatApi.Tokenizer;
using FChatApi.EventArguments;
using FChatApi;
using ModularPlugins;

namespace CardGame;

public partial class FChatTournamentOrganiser<TModuleType> : FChatPlugin<TModuleType>, IFChatPlugin
{
	internal Dictionary<string,(User FChatUser,PlayerCharacter ModuleCharacter)> RegisteredCharacters { get; }

	internal Dictionary<string,MatchChallenge> IncomingChallenges { get; }
	internal Dictionary<string,MatchChallenge> OutgoingChallenges => IncomingChallenges.ToOutgoing();

	internal List<BoardState> OngoingMatches { get; }

#if DEBUG
	internal FChatMessageBuilder MostRecentMessage = null!; 
#endif

	public FChatTournamentOrganiser(ApiConnection api,TModuleType moduleType,TimeSpan updateInterval) : base(api,moduleType,updateInterval)
	{
		ModuleType              = default!;
		RegisteredCharacters    = [];
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
	}

	public override void HandleRecievedMessage(BotCommand command)
	{
		FChatMessageBuilder messageBuilder = new FChatMessageBuilder()
			.WithAuthor(ApiConnection.CharacterName)
			.WithRecipient(command.User.Name)
			.WithMention(command.User.Mention)
			.WithChannel(command.Channel);
		
		string message = ValidateCommandUse(command);

		if (!string.IsNullOrWhiteSpace(message))
			FChatApi.EnqueueMessage(messageBuilder.WithMessage(message));

		////// DO STUFF HERE
		
		HandleValidatedCommand(messageBuilder,command);

		////////////////////

#if DEBUG
		MostRecentMessage = messageBuilder;
#endif
		FChatApi.EnqueueMessage(messageBuilder);
	}


	private string ValidateCommandUse(BotCommand command)
	{
		try
		{
			// User may not be null
			if (command.User == null)
				throw new ArgumentNullException(nameof(command),$"The (BotCommand) User property may not be null when handling a {command.ModuleCommand} command.");

			return string.Empty;
		}
		catch (DisallowedCommandException disallowedCommand)
		{
			return disallowedCommand.Reason switch
			{
				CommandState.InsufficientPermission    => "You don't have permission to do that!",
				CommandState.ResponseRequired          => $"You need to respond to {IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention}'s challenge first!",
				_ => throw new ArgumentOutOfRangeException(command.ToString(), $"Unexpected {typeof(CommandState)} value: {disallowedCommand.Reason}")
			};
		}
	}

	public void HandleValidatedCommand(FChatMessageBuilder messageBuilder,BotCommand command)
	{
		if (command.TryParseCommand(out CardGameCommand moduleCommand))
		try
		{
			var alertTargetMessage = new FChatMessageBuilder()
				.WithAuthor(ApiConnection.CharacterName)
				.WithoutMention()
            	.WithMessageType(command.Channel is not null ? FChatMessageType.Basic : FChatMessageType.Whisper);

			switch (moduleCommand)
			{
				case CardGameCommand.Challenge:
					if (IssueChallenge(command,messageBuilder.WithoutMention(),alertTargetMessage))
						FChatApi.EnqueueMessage(alertTargetMessage);
					break;

				case CardGameCommand.Accept:
					if (AcceptChallenge(command,messageBuilder.WithoutMention(),alertTargetMessage))
						FChatApi.EnqueueMessage(alertTargetMessage);
					break;
				
				default:
					messageBuilder
						.WithRecipient(command.User!.Name)
						.WithMessage("That is not a valid command!")
						.WithMention(RegisteredCharacters[command.User.Name.ToLower()].FChatUser.Mention);
					break;
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
					CommandState.InsufficientPermission    => $"{RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention}, you don't have permission to do that!",
					CommandState.AwaitingResponse          => $"{RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention}, you still have a challenge!",
					CommandState.ResponseRequired          => $"{RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention}, you need to respond to {IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention}'s challenge first!",
					_ => throw new ArgumentOutOfRangeException(nameof(disallowedCommand.Reason), $"Unexpected {typeof(CommandState)} value: {disallowedCommand.Reason}")
				}
			);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}

	public override void Update()
	{
		foreach(string key in OutgoingChallenges.Where((kvp)=>kvp.Value.AtTerminalStage).Select((kvp)=>kvp.Key))
			OutgoingChallenges.Remove(key);
		base.Update();
	}

	public override void HandleJoinedChannel(ChannelEventArgs @event)
	{
		ActiveChannels.TryAdd(@event.Channel.Code,@event.Channel);
	}


    void IFChatPlugin.HandleRecievedMessage(BotCommand command)
    {
        HandleRecievedMessage(command);
    }

    void IFChatPlugin.HandleJoinedChannel(ChannelEventArgs @event)
    {
        HandleJoinedChannel(@event);
    }

	static FChatTournamentOrganiser()
	{
		PreProcessEnumAttributes();
	}
}