using Plugins.Tokenizer;
using RoleplayingGame.Contexts;
using RoleplayingGame.Effects;
using RoleplayingGame.Objects;

namespace RoleplayingGame;

public partial class FRoleplayMC
{
	private void ResolveExploreCommand(CommandTokens commandTokens)
	{
///////////// GET KEY VALUES
		CharacterSheet characterSheet	= Characters.SingleByUser(commandTokens.Source.Author);
////////////////////////////
		CombatContext context	= Encounters.NewCombatEncounter();
		context.WithParticipant(characterSheet);
////////////////////////////
		context.EnqueueMessage(FChatApi);
//////////////
	}
}