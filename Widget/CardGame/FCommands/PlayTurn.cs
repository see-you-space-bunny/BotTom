using System.Text;
using CardGame.Enums;
using CardGame.MatchEntities;
using FChatApi.Objects;
using FChatApi.Tokenizer;

using ModularPlugins;
using ModularPlugins.Interfaces;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin, IFChatPlugin
{
	private bool PlayTurn(
        BotCommand command,
		FChatMessageBuilder commandResponse)
	{
		StringBuilder responseBuilder   = new();
		
		BoardState thisMatch = OngoingMatches
			.FirstOrDefault(b =>
				b.Player1.User == command.Message.Author ||
				b.Player2.User == command.Message.Author
		)!;
		if (thisMatch is null)
		{
			commandResponse.WithMessage($"{command.Message.Author.Mention}, you're not part of any ongoing games!");
			return false;
		}
		else if ( command.Message.Author != thisMatch.GetActivePlayer().User )
		{
			commandResponse.WithMessage($"{command.Message.Author.Mention}, it's not your turn!");
			return false;
		}

		//////////
		
		if (!Enum.TryParse(command.Parameters[0],true,out CharacterStat stat1))
		{
			commandResponse
				.WithMessage($"{command.Parameters[0].ToUpper()} is not a recognised stat.");
			return false;
		}

		//////////
		
		MatchPlayer activePlayer = thisMatch.GetActivePlayer();

		// if no valid stat is specified, select a random stat
		if (stat1 != activePlayer.DeckArchetype.First && stat1 != activePlayer.DeckArchetype.First)
		{
			stat1 = new Random().Next(1,3) == 1 ?
					activePlayer.DeckArchetype.First :
					activePlayer.DeckArchetype.Second;
		}

		(string result, SummonCard card) = activePlayer.DrawCard(stat1);

		activePlayer.PlayCard(card);

		thisMatch.PassTurn();

		//////////

		responseBuilder
			.Append(command.Message.Author.Mention);
		
		//////////

		return true;
	}
}