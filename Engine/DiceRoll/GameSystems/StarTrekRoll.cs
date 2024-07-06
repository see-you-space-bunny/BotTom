using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace Engine.DiceRoll.GameSystems;

/// <summary>
/// Rolling dice for the Star Trek Adventures game system.
/// </summary>
public class StarTrekRoll
{
    #region Fields
    /// <value></value>
    private readonly long _targetNumber;

    /// <value></value>
    private readonly long _dice;

    /// <value></value>
    private readonly long _focusNumber;

    /// <value></value>
    private readonly long _threatNumber;

    /// <value></value>
    private readonly long _computerTargetNumber;

    /// <value></value>
    private readonly long _computerFocusNumber;

    /// <value>.</value>
    private readonly string? _label;
    private readonly int _computerDieOffset;

    /// <value>Result of the roll.</value>
    private Tuple<int[],int,int,int>? _result;
    #endregion

    #region Constructor
    /// <summary>
    /// Constructor for the <c>StarTrekRoll</c> class.
    /// </summary>
    /// <param name="targetNumber"></param>
    /// <param name="dice"></param>
    /// <param name="focusNumber"></param>
    /// <param name="bonusHits"></param>
    /// <param name="computerTargetNumber"></param>
    /// <param name="computerFocusNumber"></param>
    /// <param name="label"></param>
    public StarTrekRoll(long targetNumber, long focusNumber, long dice, long threatNumber, long? computerTargetNumber, long computerFocusNumber, string? label)
    {
        _targetNumber = targetNumber;
        _focusNumber = focusNumber;
        _dice = dice;
        _threatNumber = threatNumber;
        _computerTargetNumber = computerTargetNumber ?? long.MinValue;
        _computerFocusNumber = computerFocusNumber;
        _label = label;
        _computerDieOffset = 0;
        if (computerTargetNumber > 0)
            _computerDieOffset = 1;
    }
    #endregion

    #region Roll
    public void Roll()
    {
        int[] rolls = DiceParser.RollDice((int)_dice, 20);
        int hits = 0;
        int threat = 0;
        int hitAtCost = 0;

        for (int i = 0; i < rolls.Length; i++)
        {
            if (i + 1 >= rolls.Length && _computerDieOffset > 0)
            {
                if (rolls[i] <= _computerFocusNumber)
                {
                    hits += 2;
                }
                else if (rolls[i] < _computerTargetNumber)
                {
                    hits += 1;
                }
                else if (rolls[i] == _computerTargetNumber)
                {
                    hitAtCost += 1;
                }
                else if (rolls[i] == 20)
                {
                    threat += 2;
                }
            }
            else
            {
                if (rolls[i] == 1 || (rolls[i] <= _focusNumber))
                {
                    hits += 2;
                }
                else if (rolls[i] < _targetNumber)
                {
                    hits += 1;
                }
                else if (rolls[i] == _targetNumber)
                {
                    hitAtCost += 1;
                }
                else if (rolls[i] == 20)
                {
                    threat += 1;
                }
                if (rolls[i] >= _threatNumber)
                {
                    threat += 1;
                }
            }
        }
        _result = new Tuple<int[],int,int,int> (rolls, hits, threat, hitAtCost);
    }
    #endregion

    #region ToString
    public override string ToString()
    {
        StringBuilder sb = new();
        if (_label != null)
        {
            sb.AppendLine($"> {_label.Replace("\\n","\n> ")}");
        }

        if (_result == null)
            return "Not yet rolled";

        sb.Append($"{_dice-_computerDieOffset}d20[`");
        for (int i = 0; i + _computerDieOffset < _result.Item1.Length; i++)
        {
            sb.Append(_result.Item1[i]);
            if (i + 1 + _computerDieOffset < _result.Item1.Length)
                sb.Append(", ");
        }
        sb.Append($"`] vs TN/F ` {_targetNumber}/{_focusNumber} `");

        if (_computerDieOffset > 0)
            sb.Append($", 1d20[`{_result.Item1[^_computerDieOffset]}`] vs TN/F ` {_computerTargetNumber}/{_computerFocusNumber} `");

        sb.Append($"\n- Successes achieved: __**` {_result.Item2} `**__");
        if (_result.Item3 > 0)
            sb.Append($"\n- Threat generated: __**` {_result.Item3} `**__");
        if (_result.Item4 > 0)
            sb.Append($"\n- (Optional) Successes at a cost: __**` {_result.Item4} `**__");

        return sb.ToString();
    }
    #endregion
}