using System.Text;

namespace Engine.DiceRoll.GameSystems;

/// <summary>
/// Rolling dice for Forged In The Dark game systems.
/// </summary>
/// <param name="dicePool"></param>
/// <param name="label"></param>
public class ForgedInTheDarkRoll
{
  #region Const
  private const string CritMessage = "**Critical Success!** Things go well __and__ you gain some additional advantage.";
  private const string SuccessMessage = "**Full Success!** Things go well.";
  private const string MixedMessage = "**Partial Success!** You do what you were trying to do, but there are consequences: trouble, harm, reduced effect, etc.";
  private const string FailMessage = "**Bad outcome!** Things go poorly. You probably don't achieve your goal and you suffer complications, too.";
  #endregion

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
    bool critical       = keepTarget == 6 && diceRolls.AsEnumerable().Count((r)=>r==keepTarget) > 1;
    bool firstHighlight = false;

    for(int i=0;i<diceRolls.Length;)
    {
      if(diceRolls[i]==keepTarget && (critical || !firstHighlight))
      {
        sb.Append($"__**` {diceRolls[i]} `**__");
        firstHighlight = true;
      }
      else
        sb.Append($"` {diceRolls[i]} `");

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

      sb.Append(_result.Item2 switch {
        int n when n <= 3 => FailMessage,
        int n when n == 4 || n == 5 => MixedMessage,
        int n when n == 6 => _result.Item3 ? CritMessage : SuccessMessage,
        _ => throw new Exception($"Impossible result ({_result.Item1},{_result.Item2},{_result.Item3}) in ForgedInTheDarkRoll"),
      });

      return sb.ToString();
  }
  #endregion
}