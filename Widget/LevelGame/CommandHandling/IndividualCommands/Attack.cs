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
using RoleplayingGame.Objects;

namespace RoleplayingGame;

public partial class FRoleplayMC
{
	private void ExecuteAttackCommand(CommandTokens commandTokens,FChatMessageBuilder messageBuilder)
	{
///////////// GET KEY VALUES

		AttackType		attackType		=	Enum.Parse<AttackType>(commandTokens.Parameters["Attack"]);
		User			userTarget		=	ApiConnection.Users.SingleByName(commandTokens.Parameters["Target"]);
		CharacterSheet	characterTarget	=	Characters.SingleByUser(userTarget);

////////////////////////////



////////////////////////////



////////////////////////////
	}
}