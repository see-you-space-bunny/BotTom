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
using RoleplayingGame.Effects;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame;

public partial class FRoleplayMC
{
#region Attack
	private bool ValidateAttackCommand(CommandTokens commandTokens,FChatMessageBuilder messageBuilder)
	{
//////////////// Attack /////
		if (!commandTokens.Parameters.TryAdd("Attack","Basic"))
			if (!Enum.TryParse<AttackType>(commandTokens.Parameters["Attack"],out _))
			{
				messageBuilder.WithMessage($"{commandTokens.Parameters["Attack"]} is not a valid type of attack.");
				return false;
			}
//////////////// Target /////
		commandTokens.Parameters.TryGetValue("Target",out string ?targetName);
		if (string.IsNullOrWhiteSpace(targetName))
		{
			throw new NullReferenceException();
		}
///////////////////////////// Target is registered?
		else if (!ApiConnection.Users.RegisteredUsers.ContainsKey(targetName))
		{
			messageBuilder.WithMessage("The character you are trying to attack is not registered.");
			return false;
		}
///////////////////////////// Target has CharacterSheet?
		else if (!Characters.ContainsKey(targetName))
		{
			messageBuilder.WithMessage("The user you are trying to attack does not have a character sheet.");
			return false;
		}
///////////////////////////// Is there a pending outgoing attack?
		else if (Interactions.TryGetPendingEventsByResponder(commandTokens.Source.Author,out AttackEvent[] attackOut))
		{
			messageBuilder.WithMessage($"You need to respond to the incoming attack from {attackOut[0].Source.CharacterName} first.");
			return false;
		}
///////////////////////////// Is there a pending incoming attack?
		else if (Interactions.TryGetPendingEventsByInitiator(commandTokens.Source.Author,out AttackEvent[] attackIn))
		{
			messageBuilder.WithMessage($"{attackIn[0].Source.CharacterName} needs to respond to your attack first.");
			return false;
		}
///////////////////////////// Is the target already being attacked?
		else if (Interactions.TryGetPendingEventsByResponder<AttackEvent>(ApiConnection.Users.SingleByName(targetName),out _))
		{
			messageBuilder.WithMessage($"{targetName} is already being attacked by someone.");
			return false;
		}
/////////////////////////////
		return true;
	}
#endregion


#region ClassChange
	private bool ValidateClassChangeCommand(CommandTokens commandTokens,FChatMessageBuilder messageBuilder)
	{
//////////////// Class //////
		if (!Enum.TryParse<ClassName>(commandTokens.Parameters["Class"],out _))
		{
			messageBuilder.WithMessage($"\"{commandTokens.Parameters["Class"]}\" is not a valid class.");
			return false;
		}
///////////////////////////// Is cooldown ready?
		CharacterSheet characterSheet	=	Characters.SingleByUser(commandTokens.Source.Author);
		if (!characterSheet.CanChangeClass)
		{
			messageBuilder.WithMessage($"You need to wait [color=yellow]{characterSheet.RemainingClassChangeCooldown}m[/color] before changing your class.");
			return false;
		}
/////////////////////////////
		return true;
	}
#endregion


#region Defend
	private bool ValidateDefendCommand(CommandTokens commandTokens,FChatMessageBuilder messageBuilder)
	{
///////////////////////////// Is there a pending incoming attack?
		if (!Interactions.TryGetPendingEventsByResponder<AttackEvent>(commandTokens.Source.Author,out _))
		{
			messageBuilder.WithMessage("There is nothing for you to defend against.");
			return false;
		}
/////////////////////////////
		return true;
	}
#endregion
}