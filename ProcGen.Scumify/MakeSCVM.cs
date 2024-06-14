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
using BotTom.DiceRoller;

namespace BotTom.ProcGen.Scumify
{
  public class MakeSCVM
  {
    public string classesDirName;
    public string tablesDirName;
    private readonly string[] mbClassFilePaths;
    private int Index { get; set; } = 0;

    public MakeSCVM()
    {
      classesDirName = Path.Combine( Environment.CurrentDirectory, "data", "morkborg", "classes" );
      tablesDirName =  Path.Combine( Environment.CurrentDirectory, "data", "morkborg", "tables" );
      mbClassFilePaths = Directory.GetFiles(classesDirName).Where((s)=>!s.Contains("blank-")).ToArray();
      if( !Directory.Exists(classesDirName) )
        throw new Exception($"Directory {classesDirName} does not exist.");
    }

    public SCVM Next()
    {
      SCVM nextSCVM = ExtractClassDataFromXml( mbClassFilePaths[Index] );
      ++Index;
      return nextSCVM;
    }

    public SCVM Random()
    {
      int randomIndex = DiceParser.BasicRoll($"1d{mbClassFilePaths.Length}").Item2 - 1;
      return ExtractClassDataFromXml( mbClassFilePaths[randomIndex] );
    }

    private static SCVM ExtractClassDataFromXml( string mbClassFilePath )
    {
        XmlDocument mbClass = new();
        mbClass.Load(mbClassFilePath);
        string name = mbClass.SelectSingleNode("/root-element/name")!.InnerText;

        string creditUrl = mbClass.SelectSingleNode("/root-element/credit")!.Attributes!["url"]!.Value;
        string creditText = mbClass.SelectSingleNode("/root-element/credit")!.InnerText;
        
        int agilityMod = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/abilities")!.Attributes!["agility"]!.Value.Replace("+","") );
        int presenceMod = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/abilities")!.Attributes!["presence"]!.Value.Replace("+","") );
        int strengthMod = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/abilities")!.Attributes!["strength"]!.Value.Replace("+","") );
        int toughnessMod = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/abilities")!.Attributes!["toughness"]!.Value.Replace("+","") );

        string weaponFormula =  mbClass.SelectSingleNode("/root-element/equipment")!.Attributes!["weapon"]!.Value.Replace("d","");
        string armorFormula = mbClass.SelectSingleNode("/root-element/equipment")!.Attributes!["armor"]!.Value.Replace("d","");
        string silverFormula = mbClass.SelectSingleNode("/root-element/equipment")!.Attributes!["silver"]!.Value;

        string hitPointsFormula = mbClass.SelectSingleNode("/root-element/survival")!.Attributes!["hitpoints"]!.Value;
        int omenDie = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/survival")!.Attributes!["omens"]!.Value.Replace("d","") );

        string backgroundFormula = mbClass.SelectSingleNode("/root-element/background")!.Attributes!["roll"]!.Value;
        string background = mbClass.SelectNodes("/root-element/background/result")![DiceParser.BasicRoll( backgroundFormula ).Item2 - 1]!.InnerText;

        string? special;
        try{
          string specialFormula = mbClass.SelectSingleNode("/root-element/special")!.Attributes!["roll"]!.Value;
          special = mbClass.SelectNodes("/root-element/special/result")![DiceParser.BasicRoll( specialFormula ).Item2 - 1]!.InnerText;
        }
        catch
        {
          special = null;
        }
        return new SCVM(
          name, creditUrl, creditText,
          agilityMod, presenceMod, strengthMod, toughnessMod,
          weaponFormula, armorFormula, silverFormula,
          hitPointsFormula, omenDie,
          background, special, mbClassFilePath
        );
    }
  }
}