using System.Text;

using FChatApi.Objects;
using FChatApi.Enums;
using Plugins.Tokenizer;
using Plugins.Core;

using CardGame.Enums;
using CardGame.Commands;
using CardGame.PersistentEntities;
using FChatApi.Core;
using System.Text.RegularExpressions;

namespace CardGame;

public partial class FChatTournamentOrganiser
{
	private bool IssueChallenge(CommandTokens commandTokens,FChatMessageBuilder commandResponse,FChatMessageBuilder targetAlertResponse)
	{
		StringBuilder responseBuilder   = new();
		StringBuilder alertBuilder      = new();

		//////////
		
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
		
		if (!commandTokens.Parameters.TryGetAs("CardName",out User challengeTarget))
		{
			commandResponse
				.WithMessage($"You have to specify a user to challenge.");
			return false;
		}
		else if (challengeTarget.PrivilegeLevel < Privilege.RegisteredUser)
		{
			commandResponse
				.WithMessage($"You cannot challenge a user that is not registered.");
			return false;
		}
		AdoptOrCreateCharacter(challengeTarget);

		var errMessage = ValidateIssueChallenge(commandTokens.Source.Author,challengeTarget);

		if (!string.IsNullOrWhiteSpace(errMessage))
		{
			commandResponse
				.WithMessage(errMessage);
			return false;
		}
		
		//////////

		responseBuilder
			.Append("You have challenged ")
			.Append(challengeTarget.Name)
			.Append(" to a [b]Duel[/b]! [sup]Awaiting response.[/sup]");

		alertBuilder
			.Append(commandTokens.Source.Author.Mention)
			.Append(" has challenged you to a [b]Duel[/b]! ")
			.Append("[i]Hint:[/i] To accept this challenge, use the command \"tom!xcg accept stat1 stat2\"");
		
		//////////

		commandResponse
			.WithMessage(responseBuilder.ToString());

		targetAlertResponse
			.WithRecipient(challengeTarget.Name)
			.WithMessage(alertBuilder.ToString());
		
		//////////

		IncomingChallenges.Add(
			challengeTarget,
			new MatchChallenge(
				commandTokens.Source.Author,
				PlayerCharacters[commandTokens.Source.Author]
					.CreateMatchPlayer(stat1,stat2),
				challengeTarget
			)
		);
		IncomingChallenges[challengeTarget].AdvanceState(MatchChallenge.Event.Initiate);
		return true;
	}

	private string ValidateIssueChallenge(User challenger,User challengeTarget)
	{
		if (!challengeTarget.IsRegistered)
		{
			return "You can't challenge a user that is not registered.";
		}

		if (!PlayerCharacters.ContainsKey(challenger))
			PlayerCharacters.Add(challenger,new PlayerCharacter(challenger));

		if (!PlayerCharacters.ContainsKey(challengeTarget))
			PlayerCharacters.Add(challengeTarget,new PlayerCharacter(challengeTarget));

		if (challengeTarget == challenger)
		{
			return "You can't challenge yourself.";
		}
		else if (OutgoingChallenges.ContainsKey(challenger))
		{
			return "You can't have more than one pending challenge.";
		}
		else if (!PlayerCharacters.ContainsKey(challengeTarget))
		{
			return "That player is already being challenged by you.";
		}
		else if (IncomingChallenges.TryGetValue(challengeTarget, out MatchChallenge? value))
		{
			return $"That player is already being challenged by {(value.Challenger == challenger ? "you" : "someone")}.";
		}
		return string.Empty;
	}
}