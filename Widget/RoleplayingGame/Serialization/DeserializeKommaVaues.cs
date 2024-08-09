using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using RoleplayingGame.Effects;
using RoleplayingGame.Enums;
using RoleplayingGame.Factories;
using RoleplayingGame.SheetComponents;
using RoleplayingGame.Systems;

namespace RoleplayingGame.Serialization
{
    internal class DeserializeKommaVaues
    {
        internal static List<CharacterClass> GetClasses(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader,CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<CharacterClassBuilder>().Select(ccb=>ccb.Build()).ToList();
            }
        }
        internal static List<AttackChassis> GetAttacks(string filePath,StatusEffectFactory statusEffectFactory,DieRoller dieRoller)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader,CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<AttackChassisBuilder>().Select(ccb=>ccb.Build(statusEffectFactory,dieRoller)).ToList();
            }
        }
    }
}