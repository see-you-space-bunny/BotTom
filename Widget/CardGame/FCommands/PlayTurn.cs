using System.Text;
using System.Text.RegularExpressions;
using CardGame.Enums;
using CardGame.MatchEntities;
using FChatApi.Enums;
using FChatApi.Objects;
using FChatApi.Tokenizer;

using ModularPlugins;
using ModularPlugins.Interfaces;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin, IFChatPlugin
{
	private bool SummonAction(CommandTokens command,FChatMessageBuilder messageBuilder,BoardState boardState,out string lastGameAction)
	{
		var activePlayer	= boardState.GetActivePlayer();
		lastGameAction		= string.Empty;
		//////////
		
		if (!Enum.TryParse(command.Parameters[0],true,out CharacterStat stat1))
		{
			messageBuilder
				.WithMessage($"{command.Parameters[0].ToUpper()} is not a recognised stat.");
			return false;
		}

		string cardName = string.Empty;
		int slot = !activePlayer.IsCardInSlot1 ? 0 : (!activePlayer.IsCardInSlot2 ? 1 : (!activePlayer.IsCardInSlot3 ? 2 : -1));
		if (command.Parameters.Length > 1 && Int32.TryParse(command.Parameters[1],out slot))
		{
			--slot;
			if (slot < 0 || slot > 2)
			{
				messageBuilder
					.WithMessage($"You can't summon into slot {slot+1}. Choose a slot bettween 1 and 3.");
				return false;
			}
			else if (activePlayer.PlaySlots[slot].Card != null)
			{
				messageBuilder
					.WithMessage($"Can't summon into slot {slot+1}. That slot already contains a card.");
				return false;
			}
		}
		
		if (boardState.CardsDrawnThisTurn > 0)
		{
			messageBuilder
				.WithMessage($"You've already summoned this turn.");
			return false;
		}

		if (slot < 0)
		{
			messageBuilder
				.WithMessage($"You don't have any available slots to summon into.");
			return false;
		}

		//////////

		string result = boardState.SummonAction(slot,stat1,cardName);
		
		//////////

		lastGameAction = $"{result} [b]→[/b] {activePlayer.User.Mention} summoned a {activePlayer.PlaySlots[slot]} into slot {slot+1}.";
		return true;
	}

	private bool AttackAction(CommandTokens command,FChatMessageBuilder messageBuilder,BoardState boardState,out string lastGameAction)
	{
		var activePlayer	= boardState.GetActivePlayer();
		var nonActivePlayer	= boardState.GetNonActivePlayer();
		lastGameAction		= string.Empty;
		StringBuilder sb 	= new StringBuilder();

		//////////

		int slotNonActivePlayer;
		if (command.Parameters.Length > 0 && Int32.TryParse(command.Parameters[0],out int slotActivePlayer))
		{
			--slotActivePlayer;
			if (slotActivePlayer < 0 || slotActivePlayer > 2)
			{
				messageBuilder
					.WithMessage($"Slot {slotActivePlayer+1} is not a valid selection. Choose a slot bettween 1 and 3.");
				return false;
			}
			else if (activePlayer.PlaySlots[slotActivePlayer].Card == null)
			{
				messageBuilder
					.WithMessage($"Slot {slotActivePlayer+1} doesn't contain a summon.");
				return false;
			}

			if (command.Parameters.Length > 1 && Int32.TryParse(command.Parameters[1],out slotNonActivePlayer))
			{
				--slotNonActivePlayer;
				if (slotNonActivePlayer < -1 || slotNonActivePlayer > 2)
				{
					messageBuilder
						.WithMessage($"Slot {slotNonActivePlayer+1} is not a valid attack target. Choose a slot bettween 0 and 3.");
					return false;
				}
				else if (slotNonActivePlayer >= 0 && nonActivePlayer.PlaySlots[slotNonActivePlayer].Card == null)
				{
					messageBuilder
						.WithMessage($"Slot {slotNonActivePlayer+1} doesn't contain a summon for you to attack.");
					return false;
				}
			}
			else
			{
				messageBuilder
					.WithMessage($"You need to specify which of your opponent's summon slots you're attacking.");
				return false;
			}
		}
		else
		{
			messageBuilder
				.WithMessage($"You need to specify which summon slot your're attacking from.");
			return false;
		}
		
		

		//////////
		
		string activePlayerInitialState		= activePlayer.PlaySlots[slotActivePlayer].ToString();

		//////////
		
		if (activePlayer.PlaySlots[slotActivePlayer].Card!.CardArchetype != CharacterStat.DEX &&
			activePlayer.PlaySlots[slotActivePlayer].Card!.TurnSummoned == boardState.MatchTurn)
		{
			messageBuilder
				.WithMessage("You need to wait a turn before you can attack with this summon.");
			return false;
		}
		else if (activePlayer.PlaySlots[slotActivePlayer].Card!.TurnAttacked == boardState.MatchTurn)
		{
			messageBuilder
				.WithMessage("You've already attacked with this summon this turn.");
			return false;
		}
		
		if (slotNonActivePlayer == -1)
		{
			if (!activePlayer.PlaySlots[slotActivePlayer].CanAttackPlayer(boardState.MatchTurn))
			{
				messageBuilder
					.WithMessage("You can't attack your opponent with this summon yet.");
				return false;
			}
			else if (nonActivePlayer.HasDefender)
			{
				messageBuilder
					.WithMessage("You can't attack your opponent while they have a defender (VIT summon).");
				return false;
			}
		
			sb.Append($"{activePlayer.User.Mention}'s {activePlayerInitialState} attacked {nonActivePlayer.User.Mention}'s life points directly, dealing {activePlayer.PlaySlots[slotActivePlayer].Card!.Power} damage,");
		
			boardState.DirectAttack(slotActivePlayer,boardState.MatchTurn);
			sb.Append($" reducing their life points to {nonActivePlayer.Health}!");

			if (nonActivePlayer.Health <= 0)
				sb.Append($" {nonActivePlayer.User.Mention} has been defeated!");
		}
		else
		{
			string nonActivePlayerInitialState	=  nonActivePlayer	.PlaySlots[slotNonActivePlayer	].ToString();
			sb.Append($"{activePlayer.User.Mention}'s {activePlayerInitialState} attacked {nonActivePlayer.User.Mention}'s {nonActivePlayerInitialState}!");
		
			boardState.ExchangeAttacks(slotActivePlayer,slotNonActivePlayer,boardState.MatchTurn);	
	
			string activePlayerFinalState		= activePlayer.PlaySlots[slotActivePlayer].ToString();
			bool activePlayerSummonAlive		= activePlayer.PlaySlots[slotActivePlayer].Card!.Health > activePlayer.PlaySlots[slotActivePlayer]._damage;

			string nonActivePlayerFinalState	= nonActivePlayer.PlaySlots[slotNonActivePlayer].ToString();
			bool nonActivePlayerSummonAlive		= nonActivePlayer.PlaySlots[slotNonActivePlayer].Card!.Health > nonActivePlayer.PlaySlots[slotNonActivePlayer]._damage;

			if (!activePlayerSummonAlive || !nonActivePlayerSummonAlive)
			{
				sb.Append($" In the aftermath, ");
				if (!activePlayerSummonAlive)
					sb.Append($" {activePlayer.User.Mention}'s {activePlayerFinalState}");

				if (!activePlayerSummonAlive && !nonActivePlayerSummonAlive)
					sb.Append($" and");

				if (!nonActivePlayerSummonAlive)
					sb.Append($" {nonActivePlayer.User.Mention}'s {nonActivePlayerFinalState}");

				if (!activePlayerSummonAlive && !nonActivePlayerSummonAlive)
					sb.Append($" are");
				else
					sb.Append($" is");

				sb.Append($" destroyed!");
			}
		}
		
		//////////
		
		lastGameAction = sb.ToString();

		return true;
	}

	private void TakeGameAction(CommandTokens command,FChatMessageBuilder messageBuilder,CardGameCommand gameAction)
	{
		var match = OngoingMatches.FirstOrDefault(m => m.Channel == command.Message.Channel);
		messageBuilder
			.WithChannel(command.Message.Channel)
			.WithMessageType(FChatMessageType.Basic);
		
		if (match == null)
		{
			messageBuilder
				.WithMessage($"{command.Message.Author.Mention}, you're not part of any ongoing games!");
			return; // no valid game 
		}
		else if (!match.IsGameChannelValid())
		{
			messageBuilder
				.WithMessage($"{command.Message.Author.Mention}, both players need to be present for game actions to be made!");
			return; // all players need to be present
		}
		else if (match.GetActivePlayer().User != command.Message.Author)
		{
			messageBuilder
				.WithMessage($"{command.Message.Author.Mention}, it's not your turn!");
			return; // not your turn
		}
		
		//////////

		switch (gameAction)
		{
			case CardGameCommand.Summon:
				if (!SummonAction(command,messageBuilder,match,out match.LastGameAction))
					return;
				break;

			case CardGameCommand.Attack:
				if (!AttackAction(command,messageBuilder,match,out match.LastGameAction))
					return;
				break;

			case CardGameCommand.Special:
			case CardGameCommand.Target:
			case CardGameCommand.EndTurn:
				match.PassTurn();
				break;
		}
		
		//////////
		match.ValidateBoardState();
		//////////
		messageBuilder
			.WithMessage(match.ToString());
	}
}