using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using RoleplayingGame.Effects;
using RoleplayingGame.Enums;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Serialization
{
    public class DeserializeKommaVaues
    {
        public static List<CharacterClass> GetClasses(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader,CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<CharacterClassBuilder>().Select(ccb=>ccb.Build()).ToList();
            }
        }
        public static List<AttackChassis> GetAttacks(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader,CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<AttackChassisBuilder>().Select(ccb=>ccb.Build()).ToList();
            }
        }
    }
}