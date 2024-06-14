using BotTom.DiceRoller;

namespace BotTom.ProcGen.Scumify;

public partial class SCVM
{
  private static (int Roll, int Die) RollOmens(int die) => (DiceParser.BasicRoll($"1d{die}").Item2, die);
}