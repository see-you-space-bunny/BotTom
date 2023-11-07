using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace BotTom
{
    /// <summary>
    /// Rolling dice for the Star Trek Adventures game system.
    /// </summary>
    internal class StarTrekRoll
    {

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

        /// <value></value>
        private long _targetNumber;

        /// <value></value>
        private long? _dice;

        /// <value></value>
        private long? _focusNumber;

        /// <value></value>
        private long? _threatNumber;

        /// <value></value>
        private long? _computerTargetNumber;

        /// <value></value>
        private long? _computerFocusNumber;

        /// <value>.</value>
        private string? _label;

        /// <value>Result of the roll.</value>
        private Tuple<int[],int,int,int>? _result;

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
        public StarTrekRoll(long targetNumber, long? focusNumber, long? dice, long? threatNumber, long? computerTargetNumber, long? computerFocusNumber, string? label)
        {
            _targetNumber = targetNumber;
            if (focusNumber is not null)
                _focusNumber = focusNumber;
            else
                _focusNumber = 1;
            if (dice is not null)
                _dice = dice;
            else
                _dice = 2;
            if (threatNumber is not null)
                _threatNumber = threatNumber;
            else
                _threatNumber = 20;
            _computerTargetNumber = computerTargetNumber;
            if (computerFocusNumber is not null)
                _computerFocusNumber = computerFocusNumber;
            else
                _computerFocusNumber = 1;
            _label = label;
        }

        public void Roll()
        {
            if (_dice is null)
                return;
            int[] rolls = DiceParser.RollDice((int)_dice, 20);
            int hits = 0;
            int threat = 0;
            int hitAtCost = 0;

            for (int i = 0; i < rolls.Length; i++)
            {
                if (i + 1 >= rolls.Length && _computerTargetNumber is not null)
                {
                    if (_computerFocusNumber is not null && rolls[i] <= _computerFocusNumber)
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
                    if (rolls[i] == 1 || (_focusNumber is not null && rolls[i] <= _focusNumber))
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
                        threat += 2;
                    }
                }
            }
            _result = new Tuple<int[],int,int,int> (rolls, hits, threat, hitAtCost);
        }


        public override string ToString()
        {
            StringBuilder sb = new();
            if (_label != null)
            {
                sb.AppendLine($"> {_label.Replace("\\n","\n> ")}");
            }

            if (_result == null)
                return "Not yet rolled";

            int computerDieOffset = 0;
            if (_computerTargetNumber is not null)
                computerDieOffset = 1;

            sb.Append($"{_dice-computerDieOffset}d20[`");
            for (int i = 0; i + computerDieOffset < _result.Item1.Length; i++)
            {
                sb.Append(_result.Item1[i]);
                if (i + 1 + computerDieOffset < _result.Item1.Length)
                    sb.Append(", ");
            }
            sb.Append($"`] vs TN/F __**` {_targetNumber}/{_focusNumber} `**__");

            if (computerDieOffset > 0)
                sb.Append($", 1d20[`{_result.Item1[^1]}`] vs TN/F __**` {_computerTargetNumber}/{_computerFocusNumber} `**__");

            sb.Append($"\n- Successes achieved: __**` {_result.Item2} `**__");
            if (_result.Item3 > 0)
                sb.Append($"\n- Threat generated: __**` {_result.Item3} `**__");
            if (_result.Item4 > 0)
                sb.Append($"\n- (Optional) Successes at a cost: __**` {_result.Item4} `**__");

            return sb.ToString();
        }
    }
}