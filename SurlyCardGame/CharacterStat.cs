using System.ComponentModel;

namespace SurlyCardGame
{
    public enum CharacterStat
    {
        [Description("Nothing")]
        NON = 0x00,

        [Description("Strength")]
        STR = 0x01,

        [Description("Vitality")]
        VIT = 0x02,

        [Description("Dexterity")]
        DEX = 0x03,

        [Description("Intelligence")]
        INT = 0x04,

        [Description("Charisma")]
        CHA = 0x05,

        [Description("Luck")]
        LUC = 0x06,
    }
}