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
	private void InitiateAttackCommand(CommandTokens commandTokens)
	{
///////////// GET KEY VALUES
		AttackType		attackType	=	Enum.Parse<AttackType>(commandTokens.Parameters["Attack"]);
		User			userTarget	=	ApiConnection.Users.SingleByName(commandTokens.Parameters["Target"]);
		CharacterSheet	defender	=	Characters.SingleByUser(userTarget);
		CharacterSheet	attacker	=	Characters.SingleByUser(commandTokens.Source.Author);
////////////////////////////
		AttackEvent		attack		=	AttackPool[attackType]
			.BuildAttack(defender,attacker)
			.WithInitiator(commandTokens.Source.Author)
			.WithResponder(userTarget)
			.WithChannel(commandTokens.Source.Channel);
		Interactions.Add(attack);
//////////////
	}
}