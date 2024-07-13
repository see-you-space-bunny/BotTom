using System.Text;

using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Tokenizer;

using ModularPlugins;

using CardGame.Enums;
using CardGame.Commands;
using CardGame.MatchEntities;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
	private bool AcceptChallenge(BotCommand command,FChatMessageBuilder commandResponse,FChatMessageBuilder challengerAlertResponse)
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
			
		var responseBuilder = new StringBuilder()
			.Append(command.Message.Author.Mention)
			.Append(" has accepted ")
			.Append(IncomingChallenges[command.Message.Author].Challenger.Mention)
			.Append("'s challenge!");

		var alertBuilder    = new StringBuilder()
			.Append(command.Message.Author.Mention)
			.Append(" has accepted your challenge!");
		
		//////////

		commandResponse
			.WithMessage(responseBuilder.ToString())
			.WithMessageType(FChatMessageType.Whisper);

		challengerAlertResponse
			.WithMessage(alertBuilder.ToString())
			.WithRecipient(IncomingChallenges[command.Message.Author].Challenger.Name)
			.WithMessageType(FChatMessageType.Whisper);
		
		//////////

		IncomingChallenges[command.Message.Author].AdvanceState(MatchChallenge.Event.Confirm);

		OngoingMatches.Add(
			new BoardState(
				IncomingChallenges[command.Message.Author].Player1,
				PlayerCharacters[command.Message.Author].CreateMatchPlayer(stat1,stat2)
		));
		return true;
	}
}