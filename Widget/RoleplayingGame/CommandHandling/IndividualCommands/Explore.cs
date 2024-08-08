using Plugins.Tokenizer;
using RoleplayingGame.Contexts;
using RoleplayingGame.Effects;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame;

public partial class FRoleplayMC
{
	private void ResolveExploreCommand(CommandTokens commandTokens)
	{
///////////// GET KEY VALUES
		CharacterSheet	characterSheet	=	Characters.SingleByUser(commandTokens.Source.Author);
		EncounterZone	encounterZone	=	Enum.Parse<EncounterZone>(commandTokens.Parameters["Location"],true);
////////////////////////////
		CombatContext context			=	Encounters
			.NewCombatEncounter([
				characterSheet,
				FoeFactory.CreateNonPlayerEnemy(encounterZone,characterSheet.Level)
			]);
////////////////////////////
		context.EnqueueMessage(FChatApi);
//////////////
	}
}