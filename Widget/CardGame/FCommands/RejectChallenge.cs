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
			.Append(IncomingChallenges[command.User.Name].Challenger.Mention)
			.Append("'s challenge!");

		var alertBuilder    = new StringBuilder()
			.Append(command.User!.Mention)
			.Append(" has rejected your challenge!");
		
		//////////

		commandResponse
			.WithMessage(responseBuilder.ToString())
			.WithMessageType(FChatMessageType.Whisper);

		challengerAlertResponse
			.WithMessage(alertBuilder.ToString())
			.WithRecipient(IncomingChallenges[command.User.Name].Challenger.Name)
			.WithMessageType(FChatMessageType.Whisper);
		
		//////////

		IncomingChallenges[command.User.Name].AdvanceState(MatchChallenge.Event.Cancel);
		return true;
	}
}