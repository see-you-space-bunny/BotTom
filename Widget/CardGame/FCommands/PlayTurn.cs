using FChatApi.Objects;
using FChatApi.Tokenizer;
using CardGame.Enums;
using ModularPlugins;

namespace CardGame;

public partial class FChatTournamentOrganiser<TModuleType> : FChatPlugin<TModuleType>, IFChatPlugin
{
	private bool PlayTurn<TBotModule>(
        BotCommand command, FChatMessageBuilder commandResponse,
        FChatMessageBuilder challengerAlertResponse) where TBotModule : struct
	{

		return true;
	}
}