using System.Text;

using FChatApi.Objects;
using FChatApi.Tokenizer;

using ModularPlugins;

using CardGame.Enums;
using CardGame.Commands;
using CardGame.PersistentEntities;
using FChatApi.Core;
using System.Text.RegularExpressions;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
	private bool IssueChallenge(CommandTokens command,FChatMessageBuilder commandResponse,FChatMessageBuilder targetAlertResponse)
	{
		StringBuilder responseBuilder   = new();
		StringBuilder alertBuilder      = new();
		
		CharacterStat stat1 = default;
		if (command.Parameters.Length < 2)
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
		if (command.Parameters.Length > 2)
		{
			if (!Enum.TryParse(command.Parameters[1],true,out stat2) && command.Parameters.Length > 3)
			{
				commandResponse
					.WithMessage($"{command.Parameters[1].ToUpper()} is not a recognised stat.");
				return false;
			}
		}
		else
		{
			commandResponse
				.WithMessage($"Too few arguments! You need to specify who your are challenging.");
			return false;
		}
		
		if (command.Parameters.Length < 2)
		{
			commandResponse
				.WithMessage($"You have to specify a user to challenge.");
			return false;
		}

		string target;
		string remainingParameters = string.Join(' ',command.Parameters[(stat2 == default ? 1 : 2)..]);
		var m = Regex.Match(remainingParameters,CommandStringInterpreter.UserPatternComplete);
		if (m.Groups.TryGetValue(CommandStringInterpreter.Player,out Group? group))
			target = group.Value;
		else
			target = remainingParameters;

		var (exitEarly, errMessage, player) = ValidateIssueChallenge(command.Message.Author,target);

		if (exitEarly)
		{
			commandResponse
				.WithMessage(errMessage);
			return false;
		}
		
		//////////

		responseBuilder
			.Append("You have challenged ")
			.Append(player.Name)
			.Append(" to a [b]Duel[/b]! [sup]Awaiting response.[/sup]");

		alertBuilder
			.Append(command.Message.Author.Mention)
			.Append(" has challenged you to a [b]Duel[/b]! ")
			.Append("[i]Hint:[/i] To accept this challenge, use the command \"tom!xcg accept [i]stat1[/i] [i]stat2[/i]\"");
		
		//////////

		commandResponse
			.WithMessage(responseBuilder.ToString());

		targetAlertResponse
			.WithRecipient(player.Name)
			.WithMessage(alertBuilder.ToString());
		
		//////////

		IncomingChallenges.Add(
			player,
			new MatchChallenge(
				command.Message.Author,
				PlayerCharacters[command.Message.Author].CreateMatchPlayer(stat1,stat2),
				player
			)
		);
		IncomingChallenges[player].AdvanceState(MatchChallenge.Event.Initiate);
		return true;
	}

	private (bool exitEarly,string message,User target) ValidateIssueChallenge(User challenger,string target)
	{
		if (!ApiConnection.Users.TrySingleByName(target,out User challengeTarget))
		{
			return (true,"You can't challenge a user that is not registered.",null!);
		}
		if (!PlayerCharacters.ContainsKey(challenger))
			PlayerCharacters.Add(challenger,new PlayerCharacter(challenger));

		if (!PlayerCharacters.ContainsKey(challengeTarget))
			PlayerCharacters.Add(challengeTarget,new PlayerCharacter(challengeTarget));

		if (challengeTarget == challenger)
		{
			return (true,"You can't challenge yourself.",challengeTarget);
		}
		else if (OutgoingChallenges.ContainsKey(challenger))
		{
			return (true,"You can't have more than one pending challenge.",challengeTarget);
		}
		else if (!PlayerCharacters.ContainsKey(challengeTarget))
		{
			return (true,"That player is already being challenged by you.",challengeTarget);
		}
		else if (IncomingChallenges.TryGetValue(challengeTarget, out MatchChallenge? value))
		{
			return (true,$"That player is already being challenged by {(value.Challenger == challenger ? "you" : "someone")}.",challengeTarget);
		}
		return (false,string.Empty,challengeTarget);
	}
}