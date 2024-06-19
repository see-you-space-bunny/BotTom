using System.Text;

namespace BotTom.DiceRoller.GameSystems;

/// <summary>
/// Rolling dice for Forged In The Dark game systems.
/// </summary>
/// <param name="dicePool"></param>
/// <param name="label"></param>
public class ForgedInTheDarkRoll
{
  #region enums
  private enum Keep
  {
    Low = 0x00,
    High = 0x01,
  }
  #endregion

  #region Fields
  readonly int _dicePool;
  readonly string? _label;
  readonly Keep _mode = Keep.High;
  /// <value>Result of the roll.</value>
  private Tuple<string,int,bool>? _result;
  #endregion

  #region Constructor
  public ForgedInTheDarkRoll(int dicePool, string? label)
  {
    if (dicePool == 0)
    {
      _dicePool = 2;
      _mode = Keep.Low;
    }
    else
      _dicePool = dicePool;
    _label = label;
  }
  #endregion

  #region Roll
  public void Roll()
  {
    StringBuilder sb    = new();
    int[] diceRolls     = DiceParser.RollDice(_dicePool, 6);
    int keepTarget      = _mode == Keep.High ? diceRolls.Max() : diceRolls.Min();
    bool critical       = (keepTarget == 6 || keepTarget == 1) && diceRolls.AsEnumerable().Count((r)=>r==keepTarget) > 1;
    bool firstHighlight = false;

    for(int i=0;i<diceRolls.Length;)
    {
      if(diceRolls[i]==keepTarget && (critical || !firstHighlight))
      {
        sb.Append($"__**` {diceRolls[i]} `**__");
        firstHighlight = true;
      }
      else
        sb.Append($"~~` {diceRolls[i]} `~~");

      if(++i<diceRolls.Length)
        sb.Append(", ");
    }

    _result = new Tuple<string,int,bool>(sb.ToString(),keepTarget,critical);
  }
  #endregion

  #region ToString
  public override string ToString()
  {
      StringBuilder sb = new();
      if(_label is not null)
          sb.AppendLine($"> {_label.Replace("\\n","\n> ")}");
      
      if (_result == null)
          return "Not yet rolled";

      sb.Append(_result.Item1);
      sb.Append(" → ");

      if (_result.Item3)
        sb.Append("**Critical ");

      sb.Append(_result.Item2 switch {
        int n when n < 3 => "Failure",
        int n when n == 4 || n == 5 => "Mixed Success",
        6 => "Success",
        _ => throw new Exception($"Impossible result ({_result.Item1},{_result.Item2},{_result.Item3}) in ForgedInTheDarkRoll"),
      });
      
      if (_result.Item3)
        sb.Append("!**");

      return sb.ToString();
  }
  #endregion
}