namespace BotTom.ProcGen.Scumify;

public partial class SCVM
{
  private readonly string _name;
  private readonly string _creditUrl;
  private readonly string _creditText;
  private readonly (int Agility,int Presence,int Strength,int Toughness) _abilities;
  private readonly (int Weapon,int Armor,int Silver) _equipment;
  private readonly (int Roll,int Die) _omens;
  private readonly int _hitPoints;
  private readonly string _background;
  private readonly string? _special;
  private const string _specialDefault = "None";
  private readonly string _sourceXml;
  
  public string Name { get=>_name; }
  public string CreditUrl { get=>_creditUrl; }
  public string CreditText { get=>_creditText; }
  public (int Agility,int Presence,int Strength,int Toughness) Abilities { get=>_abilities; }
  public (int Weapon,int Armor,int Silver) Equipment { get=>_equipment; }
  public (int Roll,int Die) Omens { get=>_omens; }
  public int HitPoints { get=>_hitPoints; }
  public string Background { get=>_background; }
  public bool HasSpecial { get=>_special is not null; }
  public string Special { get=>_special ?? _specialDefault; }

  public string SourceXml { get=>_sourceXml; }
}