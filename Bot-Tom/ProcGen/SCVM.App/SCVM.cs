namespace BotTom;

public partial class SCVM
{
  public SCVM(string name, string creditUrl, string creditText,
              int agilityMod, int presenceMod, int strengthMod, int toughnessMod,
              string weaponFormula, string armorFormula, string silverFormula,
              string hitPointsFormula, int omenDie,
              string background, string? special, string sourceXml)
  {
    _name = name;
    _creditUrl = creditUrl;
    _creditText = creditText;
    _abilities = RollAbilities(agilityMod, presenceMod, strengthMod, toughnessMod);
    _equipment = RollEquipment(weaponFormula, armorFormula, silverFormula);
    _omens = RollOmens(omenDie);
    _hitPoints = RollHitPoints(hitPointsFormula);
    _background = background;
    _special = special;
    _sourceXml = sourceXml;
  }

  public override string ToString()
  {
    // TODO
    return base.ToString() ?? string.Empty;
  }
}