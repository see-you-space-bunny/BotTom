using System.Text;

using FChatApi.Objects;
using FChatApi.Enums;
using Plugins.Tokenizer;

using ModularPlugins;

using CardGame.Enums;
using CardGame.Commands;
using CardGame.MatchEntities;
using FChatApi.Core;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
	private bool AcceptChallenge(CommandTokens command,FChatMessageBuilder commandResponse)
	{
		CharacterStat stat1 = default;
		if (command.Parameters.Length < 1)
		{
			commandResponse
				.WithMessage("You need to specify at least one stat with which to build your deck.");
			return false;
		}
		else if (!Enum.TryParse(command.Parameters[0],true,out stat1))
		{
			commandResponse
				.WithMessage($"{command.Parameters[0].ToUpper()} is not a recognised stat.");
			return false;
		}

		CharacterStat stat2 = default;
		if (command.Parameters.Length > 1)
		{
			if (!Enum.TryParse(command.Parameters[1],true,out stat2))
			{
				commandResponse
					.WithMessage($"{command.Parameters[1].ToUpper()} is not a recognised stat.");
				return false;
			}
		}
		
		//////////

		IncomingChallenges[command.Message.Author].AdvanceState(MatchChallenge.Event.Confirm);
		
		//////////

		OngoingMatches.Add(
			new BoardState(
				IncomingChallenges[command.Message.Author].Player1,
				PlayerCharacters[command.Message.Author].CreateMatchPlayer(stat1,stat2)
		));
		
		//////////
		
		ApiConnection.User_CreateChannel(GameChannelName);
		return true;
	}

	private void FinalizeMatchCreation(Channel channel)
	{
		var match = OngoingMatches.FirstOrDefault(m=>m.AwaitingChannel)!; 
		match.Channel = channel;

		FChatApi.EnqueueMessage(
			new FChatMessageBuilder()
				.WithAuthor(ApiConnection.ApiUser)
				.WithRecipient(match.Player1.User)
				.WithMessageType(FChatMessageType.Whisper)
				.WithMessage($"{match.Player2.User.Mention} has accepted your challenge! Go to your match here: [session={match.Channel.Name}]{match.Channel.Code}[/session]")
		);

		FChatApi.EnqueueMessage(
			new FChatMessageBuilder()
				.WithAuthor(ApiConnection.ApiUser)
				.WithRecipient(match.Player2.User)
				.WithMessageType(FChatMessageType.Whisper)
				.WithMessage($"You have accepted {match.Player1.User.Mention}'s challenge! Go to your match here: [session={match.Channel.Name}]{match.Channel.Code}[/session]")
		);

		ApiConnection.Mod_SetChannelUserStatus(match.Channel,match.Player1.User,UserRoomStatus.Invited);
		ApiConnection.Mod_SetChannelUserStatus(match.Channel,match.Player2.User,UserRoomStatus.Invited);
	}
}