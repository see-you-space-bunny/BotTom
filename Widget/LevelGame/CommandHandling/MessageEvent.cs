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
/////////////////////////////
		bool respondWithMessage = true;
		FChatMessageBuilder messageBuilder = new FChatMessageBuilder()
			.WithAuthor(ApiConnection.CharacterName)
			.WithRecipient(commandTokens.Source.Author.Name)
			.WithChannel(commandTokens.Source.Channel)
			.WithMessageType(commandTokens.Source.Channel is not null ? FChatMessageType.Basic : FChatMessageType.Whisper);
/////////////// DO STUFF HERE
		switch (command)
		{
#region Attack
			case RoleplayingGameCommand.Attack:
				if (ValidateAttackCommand(commandTokens,messageBuilder))
					ExecuteAttackCommand(commandTokens,messageBuilder);
				break;
#endregion


#region Defend
			case RoleplayingGameCommand.Defend:
				if (ValidateDefendCommand(commandTokens,messageBuilder))
					ExecuteDefendCommand(commandTokens,messageBuilder);
				break;
#endregion


#region default
			default:
				break;
#endregion
		}
/////////////////////////////
		if (respondWithMessage)
		{
			FChatApi.EnqueueMessage(messageBuilder);
		}
//////////////
	}
}