using System.Text;

using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Core;

using Plugins.Tokenizer;
using Plugins.Core;

using CardGame.Enums;
using CardGame.Commands;
using CardGame.MatchEntities;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin<CardGameCommand>
{
	private bool AcceptChallenge(CommandTokens commandTokens,FChatMessageBuilder commandResponse)
	{
		if (!commandTokens.Parameters.TryGetValue("Stat1",out string ?stat1Raw) ||
			!Enum.TryParse(stat1Raw,true,out CharacterStat stat1))
		{
			commandResponse
				.WithMessage($"{stat1Raw} is not a recognised stat.");
			return false;
		}
		
		if (!commandTokens.Parameters.TryGetValue("Stat2",out string ?stat2Raw) ||
			!Enum.TryParse(stat2Raw,true,out CharacterStat stat2))
		{
			commandResponse
				.WithMessage($"{stat2Raw} is not a recognised stat.");
			return false;
		}
		
		//////////

		IncomingChallenges[commandTokens.Source.Author].AdvanceState(MatchChallenge.Event.Confirm);
		
		//////////

		OngoingMatches.Add(
			new BoardState(
				IncomingChallenges[commandTokens.Source.Author].Player1,
				PlayerCharacters[commandTokens.Source.Author].CreateMatchPlayer(stat1,stat2)
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