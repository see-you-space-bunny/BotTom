using System.Text;

using FChatApi.Objects;
using FChatApi.Enums;
using Plugins.Tokenizer;

using Plugins;

using CardGame.Enums;
using CardGame.Commands;

namespace CardGame;

public partial class FChatTournamentOrganiser
{
	private bool RejectChallenge(CommandTokens commandTokens,FChatMessageBuilder commandResponse,FChatMessageBuilder challengerAlertResponse)
	{
		//////////
			
		var responseBuilder = new StringBuilder()
			.Append("You have rejected ")
			.Append(IncomingChallenges[commandTokens.Source.Author].Challenger.Mention)
			.Append("'s challenge!");

		var alertBuilder    = new StringBuilder()
			.Append(commandTokens.Source.Author.Mention)
			.Append(" has rejected your challenge!");
		
		//////////

		commandResponse
			.WithMessage(responseBuilder.ToString())
			.WithMessageType(FChatMessageType.Whisper);

		challengerAlertResponse
			.WithMessage(alertBuilder.ToString())
			.WithRecipient(IncomingChallenges[commandTokens.Source.Author].Challenger.Name)
			.WithMessageType(FChatMessageType.Whisper);
		
		//////////

		IncomingChallenges[commandTokens.Source.Author].AdvanceState(MatchChallenge.Event.Cancel);
		return true;
	}
}