using System.Text;

using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Tokenizer;

using ModularPlugins;

using CardGame.Enums;
using CardGame.Commands;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
	private bool RejectChallenge(BotCommand command,FChatMessageBuilder commandResponse,FChatMessageBuilder challengerAlertResponse)
	{
		//////////
			
		var responseBuilder = new StringBuilder()
			.Append("You have rejected ")
			.Append(IncomingChallenges[command.Message.Author].Challenger.Mention)
			.Append("'s challenge!");

		var alertBuilder    = new StringBuilder()
			.Append(command.Message.Author.Mention)
			.Append(" has rejected your challenge!");
		
		//////////

		commandResponse
			.WithMessage(responseBuilder.ToString())
			.WithMessageType(FChatMessageType.Whisper);

		challengerAlertResponse
			.WithMessage(alertBuilder.ToString())
			.WithRecipient(IncomingChallenges[command.Message.Author].Challenger.Name)
			.WithMessageType(FChatMessageType.Whisper);
		
		//////////

		IncomingChallenges[command.Message.Author].AdvanceState(MatchChallenge.Event.Cancel);
		return true;
	}
}