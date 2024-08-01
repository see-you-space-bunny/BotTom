using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Attributes;
using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Objects;
using Plugins.Attributes;
using Plugins.Tokenizer;
using RoleplayingGame.Enums;

namespace RoleplayingGame;

public partial class FRoleplayMC
{
	private static bool ValidateAttackCommand(CommandTokens commandTokens,FChatMessageBuilder messageBuilder)
	{
//////////////// Attack /////
		commandTokens.Parameters.TryAdd("Attack","Basic");
//////////////// Target /////
		commandTokens.Parameters.TryGetValue("Target",out string ?value);
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new NullReferenceException();
		}
///////////////////////////// Target is registered?
		else if (!ApiConnection.Users.RegisteredUsers.ContainsKey(value))
		{
			messageBuilder.WithMessage("The character you are trying to attack is not registered.");
			return false;
		}
///////////////////////////// Target has CharacterSheet?
		else if (!Characters.ContainsKey(value))
		{
			messageBuilder.WithMessage("The user you are trying to attack does not have a character sheet.");
			return false;
		}
/////////////////////////////
		return true;
	}

	private static bool ValidateDefendCommand(CommandTokens commandTokens,FChatMessageBuilder messageBuilder)
	{
///////////////////////////// Is there a pending attack?
		if (true)
		{

		}
/////////////////////////////
		return true;
	}
}