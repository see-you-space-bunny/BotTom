using System.ComponentModel;
using System.Runtime.InteropServices;
using RoleplayingGame.Attributes;

namespace RoleplayingGame.Enums;

// Abilities always in order: Offense, Defense, Flexile
public enum Ability
{
	[Description("")]
	[ShortForm("NON")]
	[AbilityInfo(AbilityGroup.None,AbilityType.None)]
	None		= 0x00,

	[Description("")]
	[ShortForm("PER")]
	[AbilityInfo(AbilityGroup.Overall,AbilityType.None)]
	//[AllyAbilities(Consider,Defy)]		//| Adapt (Resist Status)
	//[EnemyAbilities(Subvert,Provoke)]		//| Inflict (Status)
	Percieve,
	
	[Description("")]
	[ShortForm("CON")]
	[AbilityInfo(AbilityGroup.Overall,AbilityType.None)]
	//[AllyAbilities(Perceive,Subvert)]		//| Anticipate (Resist Attack)
	//[EnemyAbilities(Provoke,Defy)]		//| Clash (Attack)
	Consider, 
	
	[Description("")]
	[ShortForm("SUB")]
	[AbilityInfo(AbilityGroup.Overall,AbilityType.None)]
	//[AllyAbilities(Consider,Provoke)]		//| Spellcast (Attack)
	//[EnemyAbilities(Perceive,Defy)]		//| Empathise (Buff Status)
	Subvert,
	
	[Description("")]
	[ShortForm("PRO")]
	[AbilityInfo(AbilityGroup.Overall,AbilityType.None)]
	//[AllyAbilities(Subvert,Defy)]			//| Recover (Resist Attack)
	//[EnemyAbilities(Perceive,Consider)]	//| Analyse (Buff Status)
	Provoke,
	
	[Description("")]
	[ShortForm("DEF")]
	[AbilityInfo(AbilityGroup.Overall,AbilityType.None)]
	//[AllyAbilities(Perceive,Provoke)]		//| Strike (Attack)
	//[EnemyAbilities(Consider,Subvert)]	//| Manipulate (Status)
	Defy,
	
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
