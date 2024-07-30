using System.Text;
using System.Text.RegularExpressions;
using CardGame.Enums;
using CardGame.MatchEntities;
using FChatApi.Enums;
using FChatApi.Objects;
using Plugins.Tokenizer;

using ModularPlugins;
using ModularPlugins.Interfaces;

namespace CardGame;

public partial class FChatTournamentOrganiser
{
	private static bool SummonAction(IDictionary<string,string> parameters,FChatMessageBuilder messageBuilder,BoardState boardState,out string lastGameAction)
	{
		var activePlayer	= boardState.GetActivePlayer();
		lastGameAction		= string.Empty;

		//////////
		
		if (!parameters.TryGetValue("Stat",out string ?statRaw) || !Enum.TryParse(statRaw,true,out CharacterStat stat))
		{
			messageBuilder
				.WithMessage($"{statRaw} is not a recognised stat.");
			return false;
		}

		if (!parameters.TryGetAs("Slot",out int slot))
		{
			slot = !activePlayer.IsCardInSlot1 ?
				1 : (!activePlayer.IsCardInSlot2 ?
					2 : (!activePlayer.IsCardInSlot3 ?
						3 : 0));
		}
		
		if (!parameters.TryGetValue("CardName",out string ?cardName))
		{
			cardName ??= string.Empty;
		}

		--slot;
		if (slot == -1)
		{
			messageBuilder
				.WithMessage($"You can't summon right now. You have no available slots.");
			return false;
		}
		else if (slot < 0 || slot > 2)
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
		
		if (boardState.CardsDrawnThisTurn > 0)
		{
			messageBuilder
				.WithMessage($"You've already summoned this turn.");
			return false;
		}

		//////////

		string result = boardState.SummonAction(slot,stat,cardName);
		
		//////////

		lastGameAction = $"{result} [b]→[/b] {activePlayer.User.Mention} summoned a {activePlayer.PlaySlots[slot]} into slot {slot+1}.";
		return true;
	}

	private static bool AttackAction(Dictionary<string,string> parameters,FChatMessageBuilder messageBuilder,BoardState boardState,out string gameActionMessage)
	{
		gameActionMessage	= string.Empty;
		if (parameters.Count == 0)
		{
			return false;
		}
		var activePlayer	= boardState.GetActivePlayer();
		var nonActivePlayer	= boardState.GetNonActivePlayer();
		StringBuilder sb 	= new StringBuilder();

		//////////

		if (!parameters.TryGetAs("Slot1",out int slotActivePlayer))
		{
			slotActivePlayer = !activePlayer.IsCardInSlot1 ?
				1 : (!activePlayer.IsCardInSlot2 ?
					2 : (!activePlayer.IsCardInSlot3 ?
						3 : 0));
		}
		--slotActivePlayer;

		if (!parameters.TryGetAs("Slot2",out int slotNonActivePlayer))
		{
			slotNonActivePlayer = !nonActivePlayer.IsCardInSlot1 ?
				1 : (!nonActivePlayer.IsCardInSlot2 ?
					2 : (!nonActivePlayer.IsCardInSlot3 ?
						3 : 0));
		}
		--slotNonActivePlayer;

		if (slotActivePlayer < 0 || slotActivePlayer > 2)
		{
			messageBuilder
				.WithMessage($"Slot {slotActivePlayer+1} is not a valid selection. Choose a slot bettween 1 and 3.");
			return false;
		}
		else if (activePlayer.PlaySlots[slotActivePlayer].Card is null)
		{
			messageBuilder
				.WithMessage($"Slot {slotActivePlayer+1} doesn't contain a summon.");
			return false;
		}

		
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
		
		//////////
		
		string activePlayerInitialState		= activePlayer.PlaySlots[slotActivePlayer].ToString();

		//////////
		
		if (activePlayer.PlaySlots[slotActivePlayer].Card?.CardArchetype != CharacterStat.DEX &&
			activePlayer.PlaySlots[slotActivePlayer].Card?.TurnSummoned == boardState.MatchTurn)
		{
			messageBuilder
				.WithMessage("You need to wait a turn before you can attack with this summon.");
			return false;
		}
		else if (activePlayer.PlaySlots[slotActivePlayer].Card?.TurnAttacked == boardState.MatchTurn ||
			(activePlayer.PlaySlots[slotActivePlayer].Card?.SecondAttackTaken ?? false))
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
			bool activePlayerSummonAlive		= activePlayer.PlaySlots[slotActivePlayer].Card?.Health > activePlayer.PlaySlots[slotActivePlayer]._damage;

			string nonActivePlayerFinalState	= nonActivePlayer.PlaySlots[slotNonActivePlayer].ToString();
			bool nonActivePlayerSummonAlive		= nonActivePlayer.PlaySlots[slotNonActivePlayer].Card?.Health > nonActivePlayer.PlaySlots[slotNonActivePlayer]._damage;

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
		
		gameActionMessage = sb.ToString();

		return true;
	}

	private void TakeGameAction(CommandTokens commandTokens,FChatMessageBuilder messageBuilder,CardGameCommand gameAction)
	{
		messageBuilder
			.WithChannel(commandTokens.Source.Channel)
			.WithMessageType(FChatMessageType.Basic);
		BoardState boardState = OngoingMatches
			.FirstOrDefault(m => m.Channel == commandTokens.Source.Channel)!;
		if (boardState is null)
		{
			messageBuilder
				.WithMessage($"{commandTokens.Source.Author.Mention}, you're not part of any ongoing games!");
			return; // no valid game 
		}
		else if (!boardState.IsGameChannelValid())
		{
			messageBuilder
				.WithMessage($"{commandTokens.Source.Author.Mention}, both players need to be present for game actions to be made!");
			return; // all players need to be present
		}
		else if (boardState.GetActivePlayer().User != commandTokens.Source.Author)
		{
			messageBuilder
				.WithMessage($"{commandTokens.Source.Author.Mention}, it's not your turn!");
			return; // not your turn
		}
		
		//////////

		switch (gameAction)
		{
			case CardGameCommand.Summon:
				if (!SummonAction(commandTokens.Parameters,messageBuilder,boardState,out boardState.LastGameAction))
					return;
				break;

			case CardGameCommand.Attack:
				if (!AttackAction(commandTokens.Parameters,messageBuilder,boardState,out boardState.LastGameAction))
					return;
				break;

			case CardGameCommand.Special:
			case CardGameCommand.Target:
			case CardGameCommand.EndTurn:
				boardState.PassTurn();
				break;
		}
		
		//////////

		boardState
			.ValidateBoardState();

		//////////
		
		messageBuilder
			.WithMessage(boardState.ToString());
	}
}