using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace ProcGen.Scumify
{
    internal static class HumanXmlDeserializer
    {
        public static void /** ItemSettings */ GetItemSettings(string filePath) { }

        public static void /** FeatSettings */ GetFeatSettings(string filePath) { }

        public static ScumSettings GetClassSettings(string filePath)
        {
            XmlDocument mbClass = new();
            mbClass.Load(filePath);
            string name = mbClass.SelectSingleNode("/root-element/name")!.InnerText;

            string creditUrl = mbClass.SelectSingleNode("/root-element/credit")!.Attributes!["url"]!.Value;
            string creditText = mbClass.SelectSingleNode("/root-element/credit")!.InnerText;
            
            int agilityMod   = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/abilities")!.Attributes!["agility"]!.Value.Replace("+","") );
            int presenceMod  = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/abilities")!.Attributes!["presence"]!.Value.Replace("+","") );
            int strengthMod  = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/abilities")!.Attributes!["strength"]!.Value.Replace("+","") );
            int toughnessMod = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/abilities")!.Attributes!["toughness"]!.Value.Replace("+","") );

            string weaponFormula = mbClass.SelectSingleNode("/root-element/equipment")!.Attributes!["weapon"]!.Value.Replace("d","");
            string armorFormula  = mbClass.SelectSingleNode("/root-element/equipment")!.Attributes!["armor"]!.Value.Replace("d","");
            string silverFormula = mbClass.SelectSingleNode("/root-element/equipment")!.Attributes!["silver"]!.Value;

            string hitPointsFormula = mbClass.SelectSingleNode("/root-element/survival")!.Attributes!["hitpoints"]!.Value;
            int omenDie = Convert.ToInt32( mbClass.SelectSingleNode("/root-element/survival")!.Attributes!["omens"]!.Value.Replace("d","") );

            string backgroundFormula = mbClass.SelectSingleNode("/root-element/background")!.Attributes!["roll"]!.Value;
            string[] backgroundTable = mbClass.SelectNodes("/root-element/background/result")!.Cast<XmlNode>().Select((n)=>n.InnerText).ToArray();

            string? specialFormula = null;
            string[]? specialTable = null;
            try{
                specialFormula = mbClass.SelectSingleNode("/root-element/special")!.Attributes!["roll"]!.Value;
                specialTable = mbClass.SelectNodes("/root-element/special/result")!.Cast<XmlNode>().Select((n)=>n.InnerText).ToArray();
            }
            catch
            { }

            return new ScumSettings(
                name,
                creditUrl,
                creditText,
                agilityMod,
                presenceMod,
                strengthMod,
                toughnessMod,
                weaponFormula,
                armorFormula,
                silverFormula,
                hitPointsFormula,
                omenDie,
                backgroundFormula,
                backgroundTable,
                specialFormula,
                specialTable
            );
        }
    }
}