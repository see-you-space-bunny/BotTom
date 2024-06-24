
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace BotTom.DiceRoller.GameSystems;

/// <summary>
/// Rolling dice for the Pathfinder 2nd Edition game system.
/// </summary>
public class PathfinderRoll
{
    #region enums
    public enum DifficultyClassType
    {
        None = 0x00,
        Basic = 0x01,
        Level = 0x02,
        SpellLevel = 0x03,
    }

    public enum DegreeOfSuccess
    {
        Failure = 0x00,
        Success = 0x01,
    }
    #endregion

    #region Fields
    /// <value>Dice Modifier (DM) of the roll, an <see cref="System.Integer"/> between -99 and 99.</value>
    private readonly long _diceModifier;

    /// <value>The type of Difficulty Class that the roll is working with.</value>
    private readonly DifficultyClassType _difficultyClassType;

    /// <value>Difficulty Class of the roll, an <see cref="System.Integer"/> between 0 and 99.</value>
    private readonly long? _difficultyClass;

    /// <value>A <see cref="System.String"/> that describes the purpose of the roll.</value>
    private readonly string? _label;

    /// <value>Result of the roll.</value>
    private Tuple<int,DegreeOfSuccess?,bool?>? _result;
    #endregion
    
    #region Constructor
    /// <summary>
    /// Constructor for the <c>PathfinderRoll</c> class.
    /// </summary>
    /// <param name="diceModifier">Dice Modifier (DM) of the roll, an <see cref="System.Integer"/> between -99 and 99.</param>
    /// <param name="difficultyClass">The type of DC used (Basic, Level, SpellLevel) and the target number of the opposing roll, an <see cref="System.Integer"/> between 0 and 99.</param>
    /// <param name="label">A <see cref="string"/> that describes the purpose of the roll.</param>
    /// <remarks>
    /// ...
    /// </remarks>
    public PathfinderRoll(long diceModifier, Tuple<DifficultyClassType,int> difficultyClass, string? label)
    {
        _diceModifier = diceModifier;
        _label = label;
        _difficultyClassType = difficultyClass.Item1;
        switch(_difficultyClassType)
        {
            case DifficultyClassType.Basic:
                _difficultyClass = difficultyClass.Item2;
            break;
            case DifficultyClassType.Level:
                _difficultyClass = 14 + difficultyClass.Item2 + (long)Math.Min(Math.Floor((decimal)difficultyClass.Item2/3),7) + (long)Math.Max((decimal)difficultyClass.Item2-21,0);
                break;
            case DifficultyClassType.SpellLevel:
                _difficultyClass = 12 + difficultyClass.Item2 * 2 + (long) Math.Floor((1 + (decimal)difficultyClass.Item2)/3*2);
            break;
            case DifficultyClassType.None:
                _difficultyClass = 0;
            break;
        }
    }
    #endregion

    #region Roll
    public void Roll()
    {
        DegreeOfSuccess? degreeOfSuccess = null;
        bool? critical = null;
        int d20Roll = DiceParser.RollDie(20);
        
        if (!(_difficultyClassType == DifficultyClassType.None))
        {
            critical = false;
            if (d20Roll + _diceModifier >= _difficultyClass)
            {
                if (d20Roll == 1 && d20Roll + _diceModifier < _difficultyClass + 10)
                    degreeOfSuccess = DegreeOfSuccess.Failure;
                else if (d20Roll == 20 || d20Roll + _diceModifier >= _difficultyClass + 10)
                {
                    degreeOfSuccess = DegreeOfSuccess.Success;
                    critical = true;
                }
                else
                    degreeOfSuccess = DegreeOfSuccess.Success;
            }
            else if (d20Roll + _diceModifier < _difficultyClass)
            {
                if (d20Roll == 20 && d20Roll + _diceModifier < _difficultyClass - 10)
                    degreeOfSuccess = DegreeOfSuccess.Success;
                else if (d20Roll == 1 || d20Roll + _diceModifier < _difficultyClass - 10)
                {
                    degreeOfSuccess = DegreeOfSuccess.Failure;
                    critical = true;
                }
                else
                    degreeOfSuccess = DegreeOfSuccess.Failure;
            }
        }
        _result = new Tuple<int, DegreeOfSuccess?,bool?> (d20Roll, degreeOfSuccess, critical);
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

        sb.Append($"d20(**` {_result.Item1} `**) ");
        
        if (_diceModifier < 0)
            sb.Append('-');
        else
            sb.Append('+');

        sb.Append($" {Math.Abs(_diceModifier)} = __**` {_result.Item1+_diceModifier} `**__");

        if (!(_difficultyClassType == DifficultyClassType.None))
        {
            sb.Append(" vs ");

            if (_difficultyClassType == DifficultyClassType.SpellLevel)
                sb.Append("Spell ");
            
            if (_difficultyClassType == DifficultyClassType.Level || _difficultyClassType == DifficultyClassType.SpellLevel)
                sb.Append("Level ");

            sb.Append($"DC **` {_difficultyClass} `**\n#"); // →

            if (_result.Item3 == true)
                sb.Append(' ');
            else
                sb.Append("# ");

            if (_result.Item3 == true)
                sb.Append("Critical ");
            if (_result.Item2 == DegreeOfSuccess.Success)
                sb.Append($"Success!");
            else
                sb.Append($"Failure!");
            if (_result.Item3 == true)
                sb.Append('!');
        }
        return sb.ToString();
    }
    #endregion
}