using System.ComponentModel;
using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Enums;
using Widget.CardGame.Enums;
using Widget.CardGame.MatchEntities;
using Widget.CardGame.Commands;
using Widget.CardGame.PersistentEntities;
using Widget.CardGame.DataStructures;
using Widget.CardGame.Exceptions;
using Engine.ModuleHost.Plugins;
using Engine.ModuleHost.CommandHandling;
using Widget.CardGame.Attributes;
using FChatApi.Attributes;

namespace Widget.CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
	internal Dictionary<string,(RegisteredUser FChatUser,PlayerCharacter ModuleCharacter)> RegisteredCharacters { get; }

	internal Dictionary<string,MatchChallenge> IncomingChallenges { get; }
	internal Dictionary<string,MatchChallenge> OutgoingChallenges => IncomingChallenges.ToOutgoing();

	internal List<BoardState> OngoingMatches { get; }

#if DEBUG
	internal FChatMessageBuilder MostRecentMessage = null!; 
#endif

	public FChatTournamentOrganiser(TimeSpan? updateInterval = null) : this(null!,updateInterval)
	{ }

	public FChatTournamentOrganiser(ApiConnection api,TimeSpan? updateInterval = null) : base(api,updateInterval)
	{
		ModuleType              = BotModule.XCG;
		RegisteredCharacters    = [];
		IncomingChallenges      = [];
		OngoingMatches          = [];
	}

	private static void PreProcessEnumAttributes()
	{
		AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(CharacterStat));
		AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(CharacterStatGroup));
		AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(Command));

		AttributeEnumExtensions.ProcessEnumForAttribute<StatAliasAttribute  >(typeof(CharacterStat));
		
		AttributeEnumExtensions.ProcessEnumForAttribute<StatGroupAttribute  >(typeof(CharacterStat));
	}

	public override void HandleRecievedMessage(BotCommand command)
	{
		FChatMessageBuilder messageBuilder = new FChatMessageBuilder()
			.WithAuthor(ApiConnection.CharacterName)
			.WithRecipient(command.User!.Name)
			.WithMention(command.User!.Mention.Name.WithPronouns)
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
				throw new ArgumentNullException(nameof(command),$"The (BotCommand) User property may not be null when handling a {command.Command} command.");

			return string.Empty;
		}
		catch (DisallowedCommandException disallowedCommand)
		{
			return disallowedCommand.Reason switch
			{
				CommandPermission.InsufficientPermission    => "You don't have permission to do that!",
				CommandPermission.ResponseRequired          => $"You need to respond to {IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention.Name.Basic}'s challenge first!",
				_ => throw new ArgumentOutOfRangeException(command.ToString(), $"Unexpected {typeof(CommandPermission)} value: {disallowedCommand.Reason}")
			};
		}
	}

	public void HandleValidatedCommand(FChatMessageBuilder messageBuilder,BotCommand command)
	{
		if (Enum.TryParse(value: command.Parameters[0],ignoreCase: true,result: out Command ModuleCommand))
		try
		{
			var alertTargetMessage = new FChatMessageBuilder()
				.WithAuthor(ApiConnection.CharacterName)
				.WithoutMention();
			switch (ModuleCommand)
			{
				case Command.Challenge:
					if (IssueChallenge(command,messageBuilder.WithoutMention(),alertTargetMessage))
						FChatApi.EnqueueMessage(alertTargetMessage);
					break;

				case Command.Accept:
					if (AcceptChallenge(command,messageBuilder.WithoutMention(),alertTargetMessage))
						FChatApi.EnqueueMessage(alertTargetMessage);
					break;
				
				default:
					messageBuilder
						.WithRecipient(command.User!.Name)
						.WithMessage("That is not a valid command!")
						.WithMention(RegisteredCharacters[command.User.Name.ToLower()].FChatUser.Mention.Name.WithPronouns);
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
					CommandPermission.InsufficientPermission    => $"{RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns}, you don't have permission to do that!",
					CommandPermission.AwaitingResponse          => $"{RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns}, you still have a challenge!",
					CommandPermission.ResponseRequired          => $"{RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns}, you need to respond to {IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention.Name.Basic}'s challenge first!",
					_ => throw new ArgumentOutOfRangeException(nameof(disallowedCommand.Reason), $"Unexpected {typeof(CommandPermission)} value: {disallowedCommand.Reason}")
				}
			);
		}
		catch (Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}

	public async override Task Update()
	{
		Task t = base.Update();
		foreach(string key in OutgoingChallenges.Where((kvp)=>kvp.Value.AtTerminalStage).Select((kvp)=>kvp.Key))
			OutgoingChallenges.Remove(key);
		await t;
	}

	public override void HandleJoinedChannel(Channel channel)
	{
		ActiveChannels.TryAdd(channel.Code,channel);
	}

	static FChatTournamentOrganiser()
	{
		PreProcessEnumAttributes();
	}
}