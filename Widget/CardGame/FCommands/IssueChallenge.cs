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
	private bool IssueChallenge(BotCommand command,FChatMessageBuilder commandResponse,FChatMessageBuilder targetAlertResponse)
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

		var (exitEarly, errMessage, player) = ValidateIssueChallenge(command.User.Key,target);

		if (exitEarly)
		{
			commandResponse
				.WithMessage(errMessage);
			return false;
		}
		
		//////////

		responseBuilder
			.Append(command.User.Mention)
			.Append(" has challenged ")
			.Append(player.Name)
			.Append(" to a [b]Duel[/b]! ")
			.Append("[i]Hint:[/i] To accept this challenge, use the command \"tom!xcg accept [i]stat1[/i] [i]stat2[/i]\"");

		alertBuilder
			.Append(command.User.Mention)
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
			player.Name.ToLower(),
			new MatchChallenge(
				command.User,
				PlayerCharacters[command.User.Key].CreateMatchPlayer(stat1,stat2),
				PlayerCharacters[player.Key]
			)
		);
		IncomingChallenges[player.Name.ToLower()].AdvanceState(MatchChallenge.Event.Initiate);
		return true;
	}

	private (bool exitEarly,string message,User target) ValidateIssueChallenge(string challenger,string target)
	{
		if (!ApiConnection.TryGetUserByName(target,out User challengeTarget))
		{
			return (true,"You can't challenge a user that is not registered.",new User());
		}
		if (!PlayerCharacters.ContainsKey(challenger.ToLowerInvariant()))
			PlayerCharacters.Add(challenger.ToLowerInvariant(),new PlayerCharacter(ApiConnection.GetUserByName(challenger)));

		if (!PlayerCharacters.ContainsKey(challengeTarget.Key))
			PlayerCharacters.Add(challengeTarget.Key,new PlayerCharacter(challengeTarget));

		if (target == challenger)
		{
			return (true,"You can't challenge yourself.",challengeTarget);
		}
		else if (OutgoingChallenges.ContainsKey(challenger.ToLower()))
		{
			return (true,"You can't have more than one pending challenge.",challengeTarget);
		}
		else if (!PlayerCharacters.ContainsKey(challengeTarget.Name.ToLower()))
		{
			return (true,"That player is already being challenged by you.",challengeTarget);
		}
		else if (IncomingChallenges.ContainsKey(challengeTarget.Name.ToLower()))
		{
			if (IncomingChallenges[challengeTarget.Name.ToLower()].Challenger.Name == challenger)
			{
				return (true,"That player is already being challenged by you.",challengeTarget);
			}
			else
			{
				return (true,"That player is already being challenged by someone.",challengeTarget);
			}
		}
		return (false,string.Empty,challengeTarget);
	}
}