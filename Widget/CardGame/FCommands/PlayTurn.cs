using FChatApi.Objects;
using FChatApi.Tokenizer;

using ModularPlugins;
using ModularPlugins.Interfaces;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin, IFChatPlugin
{
	private bool PlayTurn<TBotModule>(
        BotCommand command, FChatMessageBuilder commandResponse,
        FChatMessageBuilder challengerAlertResponse) where TBotModule : struct
	{

		return true;
	}
}