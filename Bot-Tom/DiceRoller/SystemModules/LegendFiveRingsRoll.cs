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
    internal class LegendFiveRingsRoll
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
        private long _ringDice;

        /// <value></value>
        private long? _skillDice;

        /// <value>.</value>
        private string? _label;

        /// <value>Result of the roll.</value>
        private Tuple<string,string,int,int,int>? _result;

        /// <summary>
        /// Constructor for the <c>LegendFiveRingsRoll</c> class.
        /// </summary>
        /// <param name="ringDice"></param>
        /// <param name="skillDice"></param>
        /// <param name="label"></param>
        public LegendFiveRingsRoll(int ringDice, int skillDice, string? label)
        {
            _ringDice = ringDice;

            _skillDice = skillDice;
                
            _label = label;
        }

        public void Roll()
        {
            StringBuilder sbR = new();
            StringBuilder sbS = new();
            int[] ringRolls = DiceParser.RollDice((int)_ringDice, 6);
            int[]? skillRolls;
            int successes = 0;
            int strife = 0;
            int oppurtunities = 0;

            for (int i = 0; i < ringRolls.Length; i++)
            {
                if(ringRolls[i] == 1)
                    sbR.Append(":black_large_square:");
                else if(ringRolls[i] == 2 || ringRolls[i] == 3)
                {
                    oppurtunities += 1;
                    sbR.Append(":milky_way:");
                }
                    else if(ringRolls[i] == 4 || ringRolls[i] == 5)
                {
                    successes += 1;
                    sbR.Append(":sparkles:");
                }
                    else if(ringRolls[i] == 6)
                {
                    successes += 1;
                    sbR.Append(":fireworks:");
                }

                if(ringRolls[i] == 2 || ringRolls[i] == 4 || ringRolls[i] == 6)
                {
                    strife += 1;
                    sbR.Append(":fire:");
                }
                else
                    sbR.Append(":black_large_square:");
                
                sbR.Append(" ");
            }
                

            if(_skillDice > 0)
            {
                skillRolls = DiceParser.RollDice((int)_skillDice, 12);
                for (int i = 0; i < skillRolls.Length; i++)
                {
                    if(skillRolls[i] <= 2)
                    {
                        sbS.Append(":white_large_square:");
                    }
                    else if(skillRolls[i] >= 3 && skillRolls[i] <= 5)
                    {
                        oppurtunities += 1;
                        sbS.Append(":milky_way:");
                    }
                    else if(skillRolls[i] >= 6 && skillRolls[i] <= 10)
                    {
                        successes += 1;
                        sbS.Append(":sparkles:");
                    }
                    else if(skillRolls[i] >= 11)
                    {
                        successes += 1;
                        sbS.Append(":fireworks:");
                    }

                    if(skillRolls[i] == 6 || skillRolls[i] == 7 || skillRolls[i] == 11)
                    {
                        strife += 1;
                        sbS.Append(":fire:");
                    }
                    else if(skillRolls[i] == 10)
                    {
                        oppurtunities += 1;
                        sbS.Append(":milky_way:");
                    }
                    else
                    {
                        sbS.Append(":white_large_square:");
                    }
                    
                    sbS.Append(" ");
                }
            }
            
            _result = new Tuple<string,string,int,int,int> (sbR.ToString(),sbS.ToString(),successes,oppurtunities,strife);
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


            sb.Append($"### **Ring Dice**\n> {_result.Item1}");

            if(_skillDice > 0)
                sb.Append($"\n\n### **Skill Dice**\n> {_result.Item2}");

            sb.Append("\n\n:fireworks: Exploding Success  :sparkles: Success\n:milky_way: Opportunity  :fire: Strife");

            return sb.ToString();
        }
    }
}