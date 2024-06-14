
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using org.mariuszgromada.math.mxparser;
using System.Runtime.InteropServices;
using System.Linq.Expressions;

namespace BotTom.DiceRoller.GameSystems
{
    /// <summary>
    /// Rolling dice for Storyteller game systems.
    /// </summary>
    public class StorytellerRoll
    {

        int _dicePool;
        int _hitThreshold;
        int _againThreshold;
        int _exceptionalThreshold;
        int _bonusHits;
        bool _roteAction;
        string? _label;

        bool _chanceDie;
        Tuple<int,string>? _result;

        string msg_success = "## Success\n> Your character's action goes off as planned.";
        string msg_failure = "## Failure\n> Your character's action fails. This doesn't mean “nothing happens,” just that she doesn't get what she wants and complications are headed her way. You can take a Beat in exchange for turning a normal failure into a dramatic failure.";
        string msg_exceptional = "# Exceptional Success\n> Your character's action succeeds beyond her expectations. Your character gains a beneficial Condition or a Beat. Usually, the Inspired Condition is the most appropriate. You can give this Condition to another character when it's appropriate to the story.\n> **Note:** *This does not apply to weapon attack rolls.*";
        string msg_dramatic = "# Dramatic Failure\n> Your character fails badly, and things are about to get a whole lot worse.";
        int recursionDepth;

        public StorytellerRoll (int dicePool, int hitThreshold, int againThreshold, int exceptionalThreshold, int bonusHits, bool roteAction, string? label)
        {
            if(dicePool == 0)
            {
                _dicePool = 1;
                _chanceDie = true;
            }
            else
            {
                _dicePool = dicePool;
                _chanceDie = false;
            }
            _hitThreshold = hitThreshold;
            _againThreshold = againThreshold;
            if(_againThreshold == 0)
            {
                _againThreshold = 11;
            }
            else
            {
                _againThreshold = againThreshold;
            }
            _exceptionalThreshold = exceptionalThreshold;
            _bonusHits = bonusHits;
            _roteAction = roteAction;
            _label = label;
        }

        public void Roll()
        {
            int[] rolls = DiceParser.RollDice(_dicePool, 10);
            int hits = 0;
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < rolls.Length; i++)
            {
                if(!_chanceDie && rolls[i] >= _hitThreshold || _chanceDie && rolls[i] == 10)
                {
                    hits += 1;
                    sb.Append($"**`{rolls[i]}`**");

                    if(!_chanceDie && rolls[i] >= _againThreshold)
                    {
                        recursionDepth = 0;
                        Tuple<int,string> explodedRoll = ExplodeRoll();
                        hits += explodedRoll.Item1;
                        sb.Append($"({explodedRoll.Item2})");
                    }
                }
                else if(_chanceDie && rolls[i] == 1)
                {
                    hits = -1;
                    sb.Append($"__`{rolls[i]}`__");
                }
                else
                {
                    if(_roteAction)
                    {
                        sb.Append($"~~{rolls[i]}~~");
                        recursionDepth = 0;
                        Tuple<int,string> explodedRoll = ExplodeRoll(true);
                        hits += explodedRoll.Item1;
                        sb.Append($"({explodedRoll.Item2})");
                    }
                    else
                    {
                        sb.Append($"{rolls[i]}");   
                    }
                }
                if (i + 1 < rolls.Length)
                    sb.Append(", ");
            }
            _result = new Tuple<int,string> (hits, sb.ToString());
        }

        private Tuple<int,string> ExplodeRoll(bool roteReroll = false)
        {
            int roll = DiceParser.RollDie(10);
            int hits = 0;
            StringBuilder sb = new StringBuilder();

            if(!_chanceDie && roll >= _hitThreshold || _chanceDie && roll == 10)
            {
                hits += 1;
                sb.Append($"**`{roll}`**");

                if(!_chanceDie && roll >= _againThreshold)
                {
                    recursionDepth += 1;
                    if(recursionDepth <= 12)
                    {
                        sb.Append(", ");
                        Tuple<int,string> explodedRoll = ExplodeRoll();
                        hits += explodedRoll.Item1;
                        sb.Append($"{explodedRoll.Item2}");
                    }
                }
            }
            else if(_chanceDie && roll == 1)
            {
                hits = -1;
                sb.Append($"__`{roll}`__");
            }
            else
            {
                if(_roteAction && roteReroll == false)
                {
                    sb.Append($"~~{roll}~~");
                    recursionDepth += 1;
                    if(recursionDepth <= 12)
                    {
                        sb.Append(", ");
                        Tuple<int,string> explodedRoll = ExplodeRoll(true);
                        hits += explodedRoll.Item1;
                        sb.Append($"{explodedRoll.Item2}");
                    }
                }
                else
                {
                    sb.Append($"{roll}");
                }
            }

            return new Tuple<int,string> (hits,sb.ToString());
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

            sb.Append("\n**Rolls:** ");
            sb.Append(_result.Item2);
            if(_result.Item1 > -1)
            {
                sb.Append($" **→ `{_result.Item1}` Successes");
                if(_result.Item1 > 0 && _bonusHits != 0)
                {
                    sb.Append(" (");
                    if(_bonusHits > 0)
                    {
                        sb.Append('+');
                    }
                    sb.Append($"`{_bonusHits}`)");
                }
                sb.Append("**");
            }
            sb.AppendLine();

            if(_result.Item1+_bonusHits >= _exceptionalThreshold)
            {
                sb.Append(msg_exceptional);
            }
            else if (_result.Item1 >= 1)
            {
                sb.Append(msg_success);
            }
            else if (_result.Item1 == 0)
            {
                sb.Append(msg_failure);
            }
            else if (_result.Item1 == -1)
            {
                sb.Append(msg_dramatic);
            }

            return sb.ToString();
        }
    }
}