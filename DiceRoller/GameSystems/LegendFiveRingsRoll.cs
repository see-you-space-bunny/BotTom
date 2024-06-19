
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
/// Rolling dice for the Star Trek Adventures game system.
/// </summary>
public class LegendFiveRingsRoll
{
    #region Fields
    /// <value></value>
    private readonly long _ringDice;

    /// <value></value>
    private readonly long? _skillDice;

    /// <value>.</value>
    private readonly string? _label;

    /// <value>Result of the roll.</value>
    private Tuple<string,string,int,int,int>? _result;
    #endregion
    
    #region Constructor
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
    #endregion

    #region Roll
    public void Roll()
    {
        StringBuilder sbRingDice = new();
        StringBuilder sbSkillDice = new();
        int[] ringRolls = DiceParser.RollDice((int)_ringDice, 6);
        int[]? skillRolls;
        int successes = 0;
        int strife = 0;
        int oppurtunities = 0;

        for (int i = 0; i < ringRolls.Length; i++)
        {
            /// <:Ring_Blank:1196584977157075024>
            /// <:Ring_ExSt:1196584980311195728>
            /// <:Ring_Op:1196584986212585552>
            /// <:Ring_OpSt:1196584989324742797>
            /// <:Ring_Su:1196584998535450755>
            /// <:Ring_SuSt:1196585003451162684>
            switch(ringRolls[i])
            {
                case 1:
                    sbRingDice.Append("<:Ring_Blank:1196584977157075024>");
                    break;
                case 2:
                    strife += 1;
                    oppurtunities += 1;
                    sbRingDice.Append("<:Ring_OpSt:1196584989324742797>");
                    break;
                case 3:
                    oppurtunities += 1;
                    sbRingDice.Append("<:Ring_Op:1196584986212585552>");
                    break;
                case 4:
                    strife += 1;
                    successes += 1;
                    sbRingDice.Append("<:Ring_SuSt:1196585003451162684>");
                    break;
                case 5:
                    successes += 1;
                    sbRingDice.Append("<:Ring_Su:1196584998535450755>");
                    break;
                case 6:
                    strife += 1;
                    successes += 1;
                    sbRingDice.Append("<:Ring_ExSt:1196584980311195728>");
                    break;

            }
            sbRingDice.Append(' ');
        }
            

        if(_skillDice > 0)
        {
            /// <:Skill_Blank:1196585004680101959>
            /// <:Skill_Ex:1196585008509505536>
            /// <:Skill_ExSt:1196585018978488401>
            /// <:Skill_Op:1196585024976322580>
            /// <:Skill_Su:1196585026142359583>
            /// <:Skill_SuOp:1196585028767973519>
            /// <:Skill_SuSt:1196585031523635230>
            skillRolls = DiceParser.RollDice((int)_skillDice, 12);
            for (int i = 0; i < skillRolls.Length; i++)
            {
                switch(skillRolls[i])
                {
                    case int n when n <= 2:
                        sbSkillDice.Append("<:Skill_Blank:1196585004680101959>");
                        break;
                    case int n when n >= 3 && n <= 5:
                        sbSkillDice.Append("<:Skill_Op:1196585024976322580>");
                        break;
                    case int n when n == 6 || n == 7:
                        sbSkillDice.Append("<:Skill_SuSt:1196585031523635230>");
                        break;
                    case int n when n == 8 || n == 9:
                        sbSkillDice.Append("<:Skill_Su:1196585026142359583>");
                        break;
                    case 10:
                        sbSkillDice.Append("<:Skill_SuOp:1196585028767973519>");
                        break;
                    case 11:
                        sbSkillDice.Append("<:Skill_ExSt:1196585018978488401>");
                        break;
                    case 12:
                        sbSkillDice.Append("<:Skill_Ex:1196585008509505536>");
                        break;
                }
                
                sbSkillDice.Append(' ');
            }
        }
        
        _result = new Tuple<string,string,int,int,int> (sbRingDice.ToString(),sbSkillDice.ToString(),successes,oppurtunities,strife);
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


        sb.Append($"### **Ring Dice**\n{_result.Item1}");

        if(_skillDice > 0)
            sb.Append($"\n### **Skill Dice**\n{_result.Item2}");

        //sb.Append("\n\n:fireworks: Exploding Success  :sparkles: Success\n:milky_way: Opportunity  :fire: Strife");
        return sb.ToString();
    }
    #endregion
}