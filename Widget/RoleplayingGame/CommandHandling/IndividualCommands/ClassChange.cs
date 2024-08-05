using FChatApi.Objects;
using FChatApi.Enums;
using Plugins.Tokenizer;
using RoleplayingGame.Contexts;
using RoleplayingGame.Effects;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame;

public partial class FRoleplayMC
{
	private void ResolveClassChangeCommand(CommandTokens commandTokens)
	{
///////////// GET KEY VALUES
		ClassName		className		=	Enum.Parse<ClassName>(commandTokens.Parameters["Class"]);
		CharacterSheet	characterSheet	=	Characters.SingleByUser(commandTokens.Source.Author);
///////////// SET UP MESSAGE
		FChatMessageBuilder message		= new FChatMessageBuilder()
			.WithAuthor(commandTokens.Source.Author)
			.WithChannel(commandTokens.Source.Channel)
			.WithMessageType(commandTokens.Source.MessageType);
////////////////////////////
		characterSheet.ChangeClass(CharacterClasses.All[className],triggerCooldown:true);
////////////////////////////
		FChatApi.EnqueueMessage(message);
//////////////
	}
}