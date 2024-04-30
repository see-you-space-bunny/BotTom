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
    private string classesDirName = "/home/user/Projects/dotnet/Bot-Tom/Bot-Tom/data/morkborg/classes/";
    private string tablesDirName = "/home/user/Projects/dotnet/Bot-Tom/Bot-Tom/data/morkborg/tables/";
    private StringBuilder _xmlTest = new StringBuilder(); 

    public MakeSCVM(string mbClass)
    {      
      if (Directory.Exists(classesDirName))
      {
        _xmlTest.AppendLine($"Directory {classesDirName} exists.");
        int fileCount = Directory.GetFiles(classesDirName).Count( (s) => s.EndsWith(".xml") && s.IndexOf("blank-") is -1 );

        var simpleRoll = DiceParser.BasicRoll($"1d{fileCount}");
        string rolledClassXML = Directory.GetFiles(classesDirName)[simpleRoll.Item2];
        
        int ws = 0;
        int dc = 0;
        int cc = 0;
        int ac = 0;
        int et = 0;
        int el = 0;
        int xd = 0;
        // Read a document
        XmlTextReader textReader = new XmlTextReader(rolledClassXML);
        // Read until end of file
        while (textReader.Read())
        {
            XmlNodeType nType = textReader.NodeType;
            // If node type us a declaration  
            if (nType == XmlNodeType.XmlDeclaration) {  
                _xmlTest.AppendLine("Declaration:" + textReader.Name.ToString());  
                xd = xd + 1;  
            }  
            // if node type is a comment  
            if (nType == XmlNodeType.Comment) {  
                _xmlTest.AppendLine("Comment:" + textReader.Name.ToString());  
                cc = cc + 1;  
            }  
            // if node type us an attribute  
            if (nType == XmlNodeType.Attribute) {  
                _xmlTest.AppendLine("Attribute:" + textReader.Name.ToString());  
                ac = ac + 1;  
            }  
            // if node type is an element  
            if (nType == XmlNodeType.Element) {  
                _xmlTest.AppendLine("Element:" + textReader.Name.ToString());  
                el = el + 1;  
            }  
            // if node type is an entity\  
            if (nType == XmlNodeType.Entity) {  
                _xmlTest.AppendLine("Entity:" + textReader.Name.ToString());  
                et = et + 1;  
            }
            // if node type a document  
            if (nType == XmlNodeType.DocumentType) {  
                _xmlTest.AppendLine("Document:" + textReader.Name.ToString());  
                dc = dc + 1;  
            }  
            // if node type is white space  
            if (nType == XmlNodeType.Whitespace) {  
                _xmlTest.AppendLine("WhiteSpace:" + textReader.Name.ToString());  
                ws = ws + 1;  
            }  
        }  
        // Write the summary  
        _xmlTest.AppendLine("Total Comments:" + cc.ToString());  
        _xmlTest.AppendLine("Total Attributes:" + ac.ToString());  
        _xmlTest.AppendLine("Total Elements:" + el.ToString());  
        _xmlTest.AppendLine("Total Entity:" + et.ToString());
        _xmlTest.AppendLine("Total Declaration:" + xd.ToString());  
        _xmlTest.AppendLine("Total DocumentType:" + dc.ToString());  
        _xmlTest.AppendLine("Total WhiteSpaces:" + ws.ToString());  
      }
    }

    public override string ToString()
    {
      return "Not Implemented.";
    }

    public string XmlTest { get=>_xmlTest.ToString();}
  }

  public class SCVM
  {

    private (int Agility,int Presence,int Strength,int Toughness) _abilities;
    private (int Weapon,int Armor,int Silver) _equipment;
    private (int Roll,int Die) _omens;
    private int _hitPoints;
    private string _background;
    private string? _special;

    public SCVM(int agilityMod, int presenceMod, int strengthMod, int toughnessMod,
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
  }
}