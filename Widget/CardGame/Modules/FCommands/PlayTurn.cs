using System.Text;
using FChatApi.Objects;
using FChatApi.Enums;
using Widget.CardGame.Enums;
using Widget.CardGame.Commands;
using Engine.ModuleHost.Plugins;
using Engine.ModuleHost.CommandHandling;

namespace Widget.CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
	private bool PlayTurn(BotCommand command,FChatMessageBuilder commandResponse,FChatMessageBuilder challengerAlertResponse)
	{

		return true;
	}
}