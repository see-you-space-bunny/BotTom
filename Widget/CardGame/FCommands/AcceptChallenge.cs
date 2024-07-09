using System.Text;
using FChatApi.Objects;
using FChatApi.Enums;
using CardGame.Enums;
using CardGame.Commands;
using ModularPlugins;
using FChatApi.Tokenizer;

namespace CardGame;

public partial class FChatTournamentOrganiser<TModuleType> : FChatPlugin<TModuleType>
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
			.Append(command.User!.Mention)
			.Append(" has accepted ")
			.Append(IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention)
			.Append("'s challenge!");

		var alertBuilder    = new StringBuilder()
			.Append(command.User!.Mention)
			.Append(" has accepted your challenge!");
		
		//////////

		commandResponse
			.WithMessage(responseBuilder.ToString())
			.WithMessageType(FChatMessageType.Whisper);

		challengerAlertResponse
			.WithMessage(alertBuilder.ToString())
			.WithRecipient(IncomingChallenges[command.User!.Name.ToLower()].Challenger.Name)
			.WithMessageType(FChatMessageType.Whisper);
		
		//////////

		IncomingChallenges[command.User!.Name.ToLower()].AdvanceState(MatchChallenge.Event.Confirm);

		OngoingMatches.Add(IncomingChallenges[command.User!.Name.ToLower()].AcceptWithDeckArchetype(stat1,stat2));
		return true;
	}
}