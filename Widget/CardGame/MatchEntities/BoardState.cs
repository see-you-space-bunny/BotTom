using System.Text;
using FChatApi.Interfaces;
using FChatApi.Objects;

namespace CardGame.MatchEntities;

public class BoardState(MatchPlayer player1,MatchPlayer player2)
{
	private const string OutputFormat = "{0} It is now turn {1} and the state of the game is as follows: [spoiler]\n{2}\n{3}[/spoiler]";

	internal bool AwaitingChannel => Channel == null || Channel == default;

	public Channel? Channel { get; set; }

	public MatchPlayer Player1 { get; } = player1;
	public MatchPlayer Player2 { get; } = player2;

	private bool Player1Turn = true;
	private ushort MatchTurn = 1;

	public void PassTurn()
	{
		Player1Turn = !Player1Turn;
		++MatchTurn;
	}

	internal MatchPlayer GetActivePlayer() => Player1Turn ? Player1 : Player2;

	public override string ToString()
	{
		return string.Format(OutputFormat,
			"Message describing the last game action.",
			MatchTurn,
			Player1.ToString( Player1Turn),
			Player2.ToString(!Player1Turn)
		);
	}
}