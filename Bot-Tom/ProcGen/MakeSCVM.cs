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
using System.Runtime.CompilerServices;
using System.Xml;

namespace BotTom
{
  public class MakeSCVM
  {
    /**
    private string classesDirName = Path.Combine(Directory.GetCurrentDirectory(), "/data/morkborg/classes/");
    private string tablesDirName = Path.Combine(Directory.GetCurrentDirectory(), "/morkborg/tables/");
    */

    private string CurrentWorkingDirectory = Directory.GetCurrentDirectory();

    public string classesDirName = "/data/morkborg/classes/";
    public string tablesDirName =  "/data/morkborg/tables/";
    private StringBuilder _xmlTest = new StringBuilder(); 

    public string Name { get; set; }
    public string Background { get; set; }

    public MakeSCVM()
    {      
      if (Directory.Exists(CurrentWorkingDirectory+classesDirName))
      {
        _xmlTest.AppendLine($"Directory {CurrentWorkingDirectory+classesDirName} exists.");
        int fileCount = Directory.GetFiles(CurrentWorkingDirectory+classesDirName).Count( (s) => s.EndsWith(".xml") && !s.Contains("blank-") );

        var simpleRoll = DiceParser.BasicRoll($"1d{fileCount}-1");
        string rolledClassXML = Directory.GetFiles(CurrentWorkingDirectory+classesDirName).Where((s)=>!s.Contains("blank-")).ToArray()[simpleRoll.Item2];

        // Read a document
        XmlDocument randomClass = new XmlDocument();
        randomClass.Load(rolledClassXML);

// /home/user/Projects/dotnet/Bot-Tom/Bot-Tom/bin/Debug/net7.0/data/morkborg/classes/
        Name = randomClass.SelectSingleNode("/root-element/name")!.InnerText;
        string creditUrl = randomClass.SelectSingleNode("/root-element/credit")!.Attributes!["url"]!.Value;
        string creditText = randomClass.SelectSingleNode("/root-element/credit")!.InnerText;
        Background = randomClass.SelectNodes("/root-element/background/result")![DiceParser.BasicRoll( randomClass.SelectSingleNode("/root-element/background")!.Attributes!["roll"]!.Value ).Item2 - 1]!.InnerText;
      }
      else
      {
        throw new Exception($"Directory {CurrentWorkingDirectory+classesDirName} does not exist.");
      }
    }

    public MakeSCVM(string mbClass)
    {      
      throw new NotImplementedException("You can't roll a specific class yet.");
    }

    public override string ToString()
    {
      return "Not Implemented.";
    }

  }

  public class SCVM
  {

    private (int Agility,int Presence,int Strength,int Toughness) _abilities;
    private (int Weapon,int Armor,int Silver) _equipment;
    private (int Roll,int Die) _omens;
    private int _hitPoints;
    private string _background;
    private string? _special;
    private string _name;

    public SCVM(string name, int agilityMod, int presenceMod, int strengthMod, int toughnessMod,
                int weaponDie, int armorDie, string silverFormula,
                int hitPointsDie, int omenDie,
                string[] backgrounds, string[]? special)
    {
      _abilities = rollAbilities(agilityMod, presenceMod, strengthMod, toughnessMod);
      _equipment = rollEquipment(weaponDie, armorDie, silverFormula);
      _omens = rollOmens(omenDie);
      _hitPoints = rollHitPoints(hitPointsDie);
      _background = rollBackground(backgrounds);
      _special = rollSpecial(special);
      _name = name;
    }

    private (int Agility, int Presence, int Strength, int Toughness) rollAbilities(int agilityMod, int presenceMod, int strengthMod, int toughnessMod)
    {
      throw new NotImplementedException();
    }

    private (int Weapon, int Armor, int Silver) rollEquipment(int weaponDie, int armorDie, string silverFormula)
    {
      throw new NotImplementedException();
    }

    private (int Roll, int Die) rollOmens(int die)
    {
      int r = 1;
      // TODO
      return (Math.Max(r,1), die);
    }

    private int rollHitPoints(int hitPointsDie)
    {
      throw new NotImplementedException();
    }

    private string rollBackground(string[] backgrounds)
    {
      throw new NotImplementedException();
    }

    private string? rollSpecial(string[]? special)
    {
      throw new NotImplementedException();
    }

    public (int Agility,int Presence,int Strength,int Toughness) Abilities { get=>_abilities; }
    public (int Weapon,int Armor,int Silver) Equipment { get=>_equipment; }
    public (int Roll,int Die) Omens { get=>_omens; }
    public int HitPoints { get=>_hitPoints; }
    public string Background { get=>_background; }
    public string? Special { get=>_special; }
    public string Name { get=>_name; }
  }
}