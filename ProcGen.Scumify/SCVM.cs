using System.Text;

namespace BotTom.ProcGen.Scumify;

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
    StringBuilder sb = new();
    sb.AppendLine($"# {Name}");
    sb.AppendLine($"> *By [{CreditText}]({CreditUrl})*");
    sb.AppendLine($"Background: {Background}");
    sb.AppendLine($"Strength {(Abilities.Strength>=0?'+':string.Empty)}{Abilities.Strength}, Agility {(Abilities.Agility>=0?'+':string.Empty)}{Abilities.Agility}, Presence {(Abilities.Presence>=0?'+':string.Empty)}{Abilities.Presence}, Toughness {(Abilities.Toughness>=0?'+':string.Empty)}{Abilities.Toughness}");
    sb.AppendLine($"{Omens.Roll} Omen{(Omens.Roll!=1?'s':string.Empty)} (d{Omens.Die}), {Equipment.Silver} Silver");
    sb.AppendLine($"Weapon {Equipment.Weapon}, Armor {Equipment.Armor}");
    if(HasSpecial)
      sb.AppendLine($"{Special.Trim()}");
    return sb.ToString();
  }
}