using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Objects;
using Plugins.Tokenizer;
using RoleplayingGame.Effects;
using RoleplayingGame.Enums;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame;

public partial class FRoleplayMC
{
#region Attack
	private bool ValidateAttackCommand(CommandTokens commandTokens,FChatMessageBuilder messageBuilder)
	{
//////////////// Attack /////
		if (!commandTokens.Parameters.TryAdd("Attack","Basic"))
			if (!Enum.TryParse<AttackType>(commandTokens.Parameters["Attack"],true,out _))
			{
				if (commandTokens.Source.MessageType == FChatMessageType.Basic)
				{
					if (commandTokens.Parameters.TryGetValue("Target",out string ?target))
					{
						commandTokens.Parameters["Target"] = $"{commandTokens.Parameters["Attack"]} {target}";
					}
					else
					{
						commandTokens.Parameters.Add("Target",commandTokens.Parameters["Attack"]);
					}
					commandTokens.Parameters["Attack"] = "Basic";
				}
				else
				{
					messageBuilder.WithMessage($"{commandTokens.Parameters["Attack"]} is not a valid type of attack.");
					return false;
				}
			}
//////////////// Target /////
		commandTokens.Parameters.TryGetValue("Target",out string ?targetName);
		if (string.IsNullOrWhiteSpace(targetName))
		{
			if (commandTokens.Source.MessageType == FChatMessageType.Whisper)
			{
				if (Encounters
					.GetCombatEncountersByUser(commandTokens.Source.Author)
					.FirstOrDefault(e=>e.HasNpcParticipant()) is null)
				{
					messageBuilder.WithMessage("You're not engaged in combat with any NPCs.");
					return false;
				}
			}
			else
			{
				throw new NullReferenceException();
			}
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
		if (!Enum.TryParse<ClassName>(commandTokens.Parameters["Class"],true,out _))
		{
			messageBuilder.WithMessage($"\"{commandTokens.Parameters["Class"]}\" is not a valid class.");
			return false;
		}
///////////////////////////// Is cooldown ready?
		Cooldown classChangeCooldown	=	Characters.SingleByUser(commandTokens.Source.Author).Cooldowns[CharacterCooldown.ClassChange];
		if (!classChangeCooldown.Ready)
		{
			messageBuilder.WithMessage($"You need to wait [color=yellow]{classChangeCooldown.Remaining}m[/color] before changing your class.");
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


#region Explore
	private bool ValidateExploreCommand(CommandTokens commandTokens,FChatMessageBuilder messageBuilder)
	{
//////////////// Location ///
		if (!Enum.TryParse<EncounterZone>(commandTokens.Parameters["Location"],true,out _))
		{
			messageBuilder.WithMessage($"\"{commandTokens.Parameters["Location"]}\" is not a valid location.");
			return false;
		}
///////////////////////////// Has ExplorationSupplies?
		if (!Characters.SingleByUser(commandTokens.Source.Author).Inventory.ContainsSpecificItem(SpecificItem.ExplorationSupplies))
		{
			messageBuilder.WithMessage($"You don't have any Exploration Supplies.");
			return false;
		}
/////////////////////////////
		return true;
	}
#endregion
}