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
	private void ResolveAttackCommand(CommandTokens commandTokens,RoleplayingGameCommand defenseCommand)
	{
///////////// GET KEY VALUES
		AttackEvent		attack		=	Interactions.FirstPendingEventByResponder<AttackEvent>(commandTokens.Source.Author);
////////////////////////////
		attack.ExecuteEffect(defenseCommand);
		// Apply status effects here!?
////////////////////////////
		attack.EnqueueMessage(FChatApi);
		Interactions.Remove(attack);
//////////////
	}
}