using System.ComponentModel;

using FChatApi.Enums;

using CardGame.Attributes;

namespace CardGame.Enums;

public enum CharacterStat
{
	[Description("")]
	[StatAlias(["Nothing"])]
	[StatColor(BBCodeColor.white)]
	[StatGroup(CharacterStatGroup.Untyped)]
	NON = 0xFF,

	[Description("")]
	[StatAlias(["Level","Lv"])]
	[StatGroup(CharacterStatGroup.Untyped)]
	LVL = 0x01,

	[Description("")]
	[StatAlias(["Strength"])]
	[StatColor(BBCodeColor.orange)]
	[StatGroup(CharacterStatGroup.Physical)]
	STR = 0x02,

	[Description("")]
	[StatAlias(["Vitality"])]
	[StatColor(BBCodeColor.yellow)]
	[StatGroup(CharacterStatGroup.Physical)]
	VIT = 0x03,

	[Description("")]
	[StatAlias(["Dexterity"])]
	[StatColor(BBCodeColor.red)]
	[StatGroup(CharacterStatGroup.Physical)]
	DEX = 0x04,

	[Description("")]
	[StatAlias(["Intelligence"])]
	[StatColor(BBCodeColor.cyan)]
	[StatGroup(CharacterStatGroup.Mental)]
	INT = 0x05,

	[Description("")]
	[StatAlias(["Charisma"])]
	[StatColor(BBCodeColor.pink)]
	[StatGroup(CharacterStatGroup.Mental)]
	CHA = 0x06,

	[Description("")]
	[StatAlias(["Luck"])]
	[StatColor(BBCodeColor.green)]
	[StatGroup(CharacterStatGroup.Mental)]
	LUC = 0x07,
}