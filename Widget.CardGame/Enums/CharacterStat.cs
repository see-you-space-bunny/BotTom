using Widget.CardGame.Attributes;
using System.ComponentModel;

namespace Widget.CardGame.Enums;

public enum CharacterStat
{
    [Description("")]
    [StatAlias(["Level","Lv"])]
    [StatGroup(CharacterStatGroup.Untyped)]
    LVL = 0x00,

    [Description("")]
    [StatAlias(["Strength"])]
    [StatGroup(CharacterStatGroup.Physical)]
    STR = 0x01,

    [Description("")]
    [StatAlias(["Vitality"])]
    [StatGroup(CharacterStatGroup.Physical)]
    VIT = 0x02,

    [Description("")]
    [StatAlias(["Dexterity"])]
    [StatGroup(CharacterStatGroup.Physical)]
    DEX = 0x03,

    [Description("")]
    [StatAlias(["Intelligence"])]
    [StatGroup(CharacterStatGroup.Mental)]
    INT = 0x04,

    [Description("")]
    [StatAlias(["Charisma"])]
    [StatGroup(CharacterStatGroup.Mental)]
    CHA = 0x05,

    [Description("")]
    [StatAlias(["Luck"])]
    [StatGroup(CharacterStatGroup.Mental)]
    LUC = 0x06,
    
    [Description("")]
    [StatAlias(["Nothing"])]
    [StatGroup(CharacterStatGroup.Untyped)]
    NON = 0xFF,
}