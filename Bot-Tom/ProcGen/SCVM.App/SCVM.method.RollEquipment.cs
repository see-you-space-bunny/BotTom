namespace BotTom;

public partial class SCVM
{
  private static (int Weapon, int Armor, int Silver) RollEquipment(string weaponFormula, string armorFormula, string silverFormula) => (
    DiceParser.BasicRoll( weaponFormula ).Item2,
    DiceParser.BasicRoll( armorFormula ).Item2,
    DiceParser.BasicRoll( silverFormula ).Item2
  );
}