using System.Diagnostics;
using System.Text.RegularExpressions;
using BotTom.DiceRoller;

namespace BotTom.ProcGen.Scumify;

public partial class SCVM
{
  private int RollHitPoints(string hitPointsFormula) => DiceParser.BasicRoll(
      AbilityShorthandRegex().Replace(
        hitPointsFormula, (m) => {
          return m.Value switch
          {
              "A" => Abilities.Agility.ToString(),
              "P" => Abilities.Presence.ToString(),
              "S" => Abilities.Strength.ToString(),
              "T" => Abilities.Toughness.ToString(),
              _ => throw new UnreachableException(),
          };
        }
      )
    ).Item2;

    [GeneratedRegex(@"[APST]{1}")]
    private static partial Regex AbilityShorthandRegex();
}