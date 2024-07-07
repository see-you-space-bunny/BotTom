using System.Text;
using FChatApi.Objects;
using FChatApi.Enums;
using Widget.CardGame.Enums;
using Widget.CardGame.Commands;
using Engine.ModuleHost.Plugins;
using Engine.ModuleHost.CommandHandling;

namespace Widget.CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
	private bool AcceptChallenge(BotCommand command,FChatMessageBuilder commandResponse,FChatMessageBuilder challengerAlertResponse)
	{
		CharacterStat stat1 = default;
		if (command.Parameters.Length < 2)
		{
			commandResponse
				.WithMessage("You need to specify at least one stat with which to build your deck.");
			return false;
		}
		else if (!Enum.TryParse(command.Parameters[1],true,out stat1))
		{
			commandResponse
				.WithMessage($"{command.Parameters[1].ToUpper()} is not a recognised stat.");
			return false;
		}

		CharacterStat stat2 = default;
		if (command.Parameters.Length > 2)
		{
			if (!Enum.TryParse(command.Parameters[2],true,out stat2))
			{
				commandResponse
					.WithMessage($"{command.Parameters[2].ToUpper()} is not a recognised stat.");
				return false;
			}
		}

		//////////
			
		var responseBuilder = new StringBuilder()
			.Append(command.User!.Mention.Name.Basic)
			.Append(" has accepted ")
			.Append(IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention.Name.Basic)
			.Append("'s challenge!");

		var alertBuilder    = new StringBuilder()
			.Append(command.User!.Mention.Name.Basic)
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