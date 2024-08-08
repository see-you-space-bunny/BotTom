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
	public override void HandleRecievedMessage(CommandTokens commandTokens)
	{
//////////////
		if (!commandTokens.TryGetParameters(out RoleplayingGameCommand command))
			return;
		if (commandTokens.Source.Author.PrivilegeLevel < command.GetEnumAttribute<RoleplayingGameCommand, MinimumPrivilegeAttribute>().Privilege)
			return;
/////////////// ERROR MESSAGE
		FChatMessageBuilder errorMessageBuilder = new FChatMessageBuilder()
			.WithAuthor(ApiConnection.ApiUser)
			.WithRecipient(commandTokens.Source.Author)
			.WithChannel(commandTokens.Source.Channel)
			.WithMessageType(commandTokens.Source.Channel is not null ? FChatMessageType.Basic : FChatMessageType.Whisper);
/////////////// DO STUFF HERE
		switch (command)
		{
#region Attack
			case RoleplayingGameCommand.Attack:
				if (ValidateAttackCommand(commandTokens,errorMessageBuilder))
					InitiateAttackCommand(commandTokens);
				break;
#endregion


#region ClassChange
			case RoleplayingGameCommand.ClassChange:
				if (ValidateClassChangeCommand(commandTokens,errorMessageBuilder))
					ResolveClassChangeCommand(commandTokens);
				break;
#endregion


#region Defend
			case RoleplayingGameCommand.Defend:
				if (ValidateDefendCommand(commandTokens,errorMessageBuilder))
					ResolveAttackCommand(commandTokens,command);
				break;
#endregion


#region Explore
			case RoleplayingGameCommand.Explore:
				if (ValidateExploreCommand(commandTokens,errorMessageBuilder))
					ResolveExploreCommand(commandTokens);
				break;
#endregion


#region default
			default:
				break;
#endregion
		}
/////////////////////////////
		if (errorMessageBuilder.HasMessage)
		{
			FChatApi.EnqueueMessage(errorMessageBuilder);
		}
//////////////
	}
}