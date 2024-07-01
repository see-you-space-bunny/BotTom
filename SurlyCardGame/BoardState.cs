using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SurlyCardGame;

public class BoardState(MatchPlayer player1,MatchPlayer player2)
{
    private const string OutputFormat = "{0} It is turn {1} and the state of the game is now: [spoiler]\n{2}\n{3}[/spoiler]";

    private readonly MatchPlayer _player1 = player1;
    private readonly MatchPlayer _player2 = player2;

    private bool Player1Turn = true;
    private ushort MatchTurn = 1;

    private void PassTurn()
    {
        Player1Turn = !Player1Turn;
        ++MatchTurn;
    }

    public override string ToString()
    {
        return string.Format(OutputFormat,
            "Message describing the last game action.",
            MatchTurn,
            _player1.ToString( Player1Turn),
            _player2.ToString(!Player1Turn)
        );
    }
}