using System.ComponentModel;

using FChatApi.Enums;

using CardGame.Attributes;

namespace CardGame.Enums;

public enum CharacterStat
{
	NON = 0xFF,

	[Description("")]
	[StatAlias(["Level","Lv"])]
	[StatGroup]
	LVL = 0x01,

	[Description("")]
	[StatAlias(["Strength"])]
	[StatDecoration("💪",BBCodeColor.orange)]
	[StatGroup(CharacterStatGroup.Physical)]
	STR = 0x02,

	[Description("")]
	[StatAlias(["Vitality"])]
	[StatDecoration("🫀",BBCodeColor.yellow)]
	[StatGroup(CharacterStatGroup.Physical)]
	VIT = 0x03,

	[Description("")]
	[StatAlias(["Dexterity"])]
	[StatDecoration("🖐️",BBCodeColor.red)]
	[StatGroup(CharacterStatGroup.Physical)]
	DEX = 0x04,

	[Description("")]
	[StatAlias(["Intelligence"])]
	[StatDecoration("🧠",BBCodeColor.cyan)]
	[StatGroup(CharacterStatGroup.Mental)]
	INT = 0x05,

	[Description("")]
	[StatAlias(["Charisma"])]
	[StatDecoration("💋",BBCodeColor.pink)]
	[StatGroup(CharacterStatGroup.Mental)]
	CHA = 0x06,

	[Description("")]
	[StatAlias(["Luck"])]
	[StatDecoration("🍀",BBCodeColor.green)]
	[StatGroup(CharacterStatGroup.Mental)]
	LUC = 0x07,
}