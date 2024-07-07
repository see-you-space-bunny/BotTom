using System.Text;
using FChatApi.Objects;
using Widget.CardGame.Enums;
using Widget.CardGame.Commands;
using Widget.CardGame.PersistentEntities;
using Engine.ModuleHost.CardiApi;
using Engine.ModuleHost.Plugins;
using Engine.ModuleHost.CommandHandling;

namespace Widget.CardGame;

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
		else if (!Enum.TryParse(command.Parameters[1],true,out stat1))
		{
			commandResponse
				.WithMessage($"{command.Parameters[1].ToUpper()} is not a recognised stat.");
			return false;
		}

		CharacterStat stat2 = default;
		if (command.Parameters.Length > 2)
		{
			if (!Enum.TryParse(command.Parameters[2],true,out stat2) && command.Parameters.Length > 3)
			{
				commandResponse
					.WithMessage($"{command.Parameters[2].ToUpper()} is not a recognised stat.");
				return false;
			}
		}
		else
		{
			commandResponse
				.WithMessage($"Too few arguments! You need to specify who your are challenging.");
			return false;
		}

		string target;
		if (stat2 == default)
		{
			target = string.Join(' ',command.Parameters[2..]);
		}
		else if (command.Parameters.Length > 3)
		{
			target = string.Join(' ',command.Parameters[3..]);
		}
		else
		{
			commandResponse
				.WithMessage($"You have to specify a user to challenge.");
			return false;
		}

		var (exitEarly, errMessage, player) = ValidateIssueChallenge(command.User!.Name,target);

		if (exitEarly)
		{
			commandResponse
				.WithMessage(errMessage);
			return false;
		}
		
		//////////

		responseBuilder
			.Append(RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns)
			.Append(" has challenged ")
			.Append(player.Name)
			.Append(" to a [b]Duel[/b]! ")
			.Append("[i]Hint:[/i] To accept this challenge, use the command \"tom!xcg accept [i]stat1[/i] [i]stat2[/i]\"");

		alertBuilder
			.Append(RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns)
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
				command.User!,
				RegisteredCharacters[command.User!.Name.ToLower()].ModuleCharacter.CreateMatchPlayer(stat1,stat2),
				RegisteredCharacters[player.Name.ToLower()].ModuleCharacter
			)
		);
		IncomingChallenges[player.Name.ToLower()].AdvanceState(MatchChallenge.Event.Initiate);
		return true;
	}

	private (bool exitEarly,string message,RegisteredUser target) ValidateIssueChallenge(string challenger,string target)
	{
		RegisteredUser player;
		if (!RegisteredCharacters.TryGetValue(target.ToLower(), out (RegisteredUser FChatUser,PlayerCharacter ModuleCharacter) userAndPlayer))
		{
			return (true,"You can't challenge a user that is not registered.",new RegisteredUser());
		}
		else
		{
			player = userAndPlayer.FChatUser;
		}

		if (target == challenger)
		{
			return (true,"You can't challenge yourself.",new RegisteredUser());
		}
		else if (OutgoingChallenges.ContainsKey(challenger.ToLower()))
		{
			return (true,"You can't have more than one pending challenge.",new RegisteredUser());
		}
		else if (!RegisteredCharacters.ContainsKey(player.Name.ToLower()))
		{
			return (true,"That player is already being challenged by you.",new RegisteredUser());
		}
		else if (IncomingChallenges.ContainsKey(player.Name.ToLower()))
		{
			if (IncomingChallenges[player.Name.ToLower()].Challenger.Name == challenger)
			{
				return (true,"That player is already being challenged by you.",new RegisteredUser());
			}
			else
			{
				return (true,"That player is already being challenged by someone.",new RegisteredUser());
			}
		}
		return (false,string.Empty,player);
	}
}