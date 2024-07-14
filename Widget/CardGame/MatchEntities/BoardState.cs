using System.Text;
using CardGame.Attributes;
using CardGame.Enums;
using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Interfaces;
using FChatApi.Objects;
using FChatApi.Attributes;

namespace CardGame.MatchEntities;

public class BoardState(MatchPlayer player1,MatchPlayer player2)
{
	private const string IntroMessage = "Your Duel Disks are spun up and ready; [color=red][u][color=white]the safeties are off![/color][/u][/color] Let the game commence! [b][color=pink]ครtгคl ςђค๓קเ๏ภร[/color][/b]!\n";
	private const string OutputFormat = "{0} It is now turn {1} and the state of the game is as follows: \n{2}\n{3}{4}";

	private const string ReminderFormat = "{0} It's turn {1}: [spoiler]\n{2}\n{3}[/spoiler]";
	private const string Dot = "[color=black][b]•[/b][/color]";
	private const string Indent2 = "\n      ";
	private const string ActionInfo		= "\n[spoiler]\n"+
										"Available actions are:\n"+
										"  "+Dot+" "+ActionInfoSummon+ "\n"+
										"  "+Dot+" "+ActionInfoAttack+ "\n"+
										"  "+Dot+" "+ActionInfoSpecial+"\n"+
										"  "+Dot+" "+ActionInfoEndTurn+
										"[/spoiler]";
	private const string ActionInfoSummon	= "Once per turn, you may create a [color=pink]Summon[/color] to fight for you."+Indent2+
												"[color=yellow](Example:[/color][spoiler] tom!xcg summon VIT 2[/spoiler][color=yellow])[/color]";
	private const string ActionInfoAttack	= "Once for each summon you have on the field, you may [color=pink]Attack[/color] your opponent or their summons."+Indent2+
												"[color=yellow](Example:[/color][spoiler] tom!xcg summon 1 2[/spoiler][color=yellow])[/color] ― "+
												"[color=orange](Usage:[/color][spoiler] Specify the slot of [u]your summon first[/u], then [u]your target second[/u]. Your opponent is [b]slot 0[/b].[/spoiler][color=orange])[/color]";
	//  Each summon may only attack once per turn. Summons can't attack the turn they come into play. (Exception:[spoiler] [color=red]DEX[/color] summons may attack [u]other summons[/u] the turn they enter play.[/spoiler])
	private const string ActionInfoSpecial	= "Take a [color=pink]Special[/color] action."+Indent2+
												"[color=yellow](Example:[/color][spoiler] tom!xcg special CHA[/spoiler][color=yellow])[/color] ― "+
												"[color=orange](Usage:[/color][spoiler] Check [user]XASTLE XATRAKIEL[/user] for more info on what special actions do.[/spoiler][color=orange])[/color]";
	private const string ActionInfoEndTurn 	= "[color=pink]End[/color] your [color=pink]Turn[/color] once you have made your plays."+Indent2+
												"[color=yellow](Example:[/color][spoiler] tom!xcg endturn[/spoiler][color=yellow])[/color]";

	internal string LastGameAction = IntroMessage;

	internal bool AwaitingChannel => Channel == null || Channel == default;

	internal bool WelcomeMessageSent = false;

	internal short CardsDrawnThisTurn = 0;

	internal bool ActionInfoDisplayedThisTurn = false;

	public Channel? Channel { get; set; }

	public MatchPlayer Player1 { get; } = player1;
	public MatchPlayer Player2 { get; } = player2;

	private bool Player1Turn = true;
	public ushort MatchTurn = 1;

	internal MatchPlayer GetActivePlayer() =>
		Player1Turn ? Player1 : Player2;

	internal MatchPlayer GetNonActivePlayer() =>
		Player1Turn ? Player2 : Player1;

	internal bool IsGameChannelValid() =>
		Channel != null &&
		Channel.Users.ContainsValue(Player1.User) &&
		Channel.Users.ContainsValue(Player2.User);

	internal void ValidateBoardState()
	{
		foreach (PlaySlot slot in Player1.PlaySlots)
		{
			if (slot.Card is not null && slot._damage >= slot.Card!.Health)
				slot.Card = null;
		}

		foreach (PlaySlot slot in Player2.PlaySlots)
		{
			if (slot.Card is not null && slot._damage >= slot.Card!.Health)
				slot.Card = null;
		}
	}

	internal void SendWelcomeMessage(ApiConnection api)
	{
		api.EnqueueMessage(
			new FChatMessageBuilder()
				.WithAuthor(ApiConnection.ApiUser)
				.WithChannel(Channel)
				.WithMessageType(FChatMessageType.Basic)
				.WithMessage(this.ToString())
		);
		WelcomeMessageSent = true;
	}
	
	internal string SummonAction(int slot,CharacterStat stat,string name)
	{
		var player			= GetActivePlayer(); 
		int dieRoll			= new Random().Next(1,101);
		int statModifier	= player.PlayerCharacter.Stats[stat]/10;
		int rollSum			= (dieRoll+statModifier)/10;
		--player.DeckSize;
		++CardsDrawnThisTurn;

		player.PlaySlots[slot].Card = new SummonCard(stat,(ushort)rollSum,(ushort)Math.Max(rollSum,1),MatchTurn,name);

		return string.Format("({0}{1}+🎲{2})",
			stat.GetEnumAttribute<CharacterStat,StatDecorationAttribute>().Emoji,
			statModifier.ToString(),
			dieRoll.ToString()
		);
	}

	internal void ExchangeAttacks(int slotActivePlayer, int slotNonActivePlayer, ushort turn)
	{
		var activePlayer	= GetActivePlayer();
		var nonActivePlayer	= GetNonActivePlayer();
		activePlayer	.PlaySlots[slotActivePlayer		]._damage	+= (short)nonActivePlayer	.PlaySlots[slotNonActivePlayer	].Card!.PowerVsSummons;
		nonActivePlayer	.PlaySlots[slotNonActivePlayer	]._damage	+= (short)activePlayer		.PlaySlots[slotActivePlayer		].Card!.PowerVsSummons;
		activePlayer.PlaySlots[slotActivePlayer].Card!.TurnAttacked = turn;
	}

	internal void DirectAttack(int slotActivePlayer,ushort turn)
	{
		var activePlayer	= GetActivePlayer();
		var nonActivePlayer	= GetNonActivePlayer();
		
		nonActivePlayer.Health -= (short)activePlayer.PlaySlots[slotActivePlayer].Card!.Power;
		activePlayer.PlaySlots[slotActivePlayer].Card!.TurnAttacked = turn;
	}

	public void PassTurn()
	{
		LastGameAction = $"{GetActivePlayer().User.Mention} has passed the turn to {GetNonActivePlayer().User.Mention}.";
		Player1Turn = !Player1Turn;
		CardsDrawnThisTurn = 0;
		ActionInfoDisplayedThisTurn = false;
		++MatchTurn;
	}

	public override string ToString()
	{
		if (!ActionInfoDisplayedThisTurn)
		{
			ActionInfoDisplayedThisTurn = true;
			return string.Format(OutputFormat,
			LastGameAction,
			MatchTurn,
			Player1.ToString( Player1Turn),
			Player2.ToString(!Player1Turn),
			ActionInfo
		);
		}
		else
		{
			return string.Format(ReminderFormat,
				LastGameAction,
				MatchTurn,
				Player1.ToString( Player1Turn),
				Player2.ToString(!Player1Turn)
			);
		}
	}
}