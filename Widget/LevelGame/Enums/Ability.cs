using System.ComponentModel;

using LevelGame.Attributes;

namespace LevelGame.Enums;

// Abilities always in order: Offense, Defense, Flexile
public enum Ability
{
	[Description("")]
	[ShortForm("NON")]
	[AbilityInfo(AbilityGroup.None,AbilityType.None)]
	None		= 0x00,
	
	#region Physical Abilities
	[Description("")]
	[ShortForm("POW")]
	[AbilityInfo(AbilityGroup.Physical,AbilityType.Offense)]
	Power		= 0x01,

	[Description("")]
	[ShortForm("BOD")]
	[AbilityInfo(AbilityGroup.Physical,AbilityType.Defense)]
	Body		= 0x02,

	[Description("")]
	[ShortForm("REF")]
	[AbilityInfo(AbilityGroup.Physical,AbilityType.Flexile)]
	Reflex		= 0x03,
	#endregion
	
	#region Mental Abilities
	[Description("")]
	[ShortForm("FOC")]
	[AbilityInfo(AbilityGroup.Mental,AbilityType.Offense)]
	Focus		= 0x04,

	[Description("")]
	[ShortForm("WIL")]
	[AbilityInfo(AbilityGroup.Mental,AbilityType.Defense)]
	Will		= 0x05,

	[Description("")]
	[ShortForm("WIT")]
	[AbilityInfo(AbilityGroup.Mental,AbilityType.Flexile)]
	Wit			= 0x06,
	#endregion
	
	#region Social Abilities
	[Description("")]
	[ShortForm("CHA")]
	[AbilityInfo(AbilityGroup.Social,AbilityType.Offense)]
	Charm		= 0x09,

	[Description("")]
	[ShortForm("INT")]
	[AbilityInfo(AbilityGroup.Social,AbilityType.Defense)]
	Integrity	= 0x08,

	[Description("")]
	[ShortForm("PRS")]
	[AbilityInfo(AbilityGroup.Social,AbilityType.Flexile)]
	Presence	= 0x07,
	#endregion

	[Description("")]
	[ShortForm("LUK")]
	[AbilityInfo(AbilityGroup.Overall,AbilityType.Flexile)]
	Luck		= 0x0A,

	[Description("")]
	[ShortForm("LVL")]
	[AbilityInfo(AbilityGroup.Overall,AbilityType.None)]
	Level		= 0xFF,
}
