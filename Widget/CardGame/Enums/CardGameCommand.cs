using System.ComponentModel;

namespace CardGame.Enums;

public enum CardGameCommand
{
	[Description("")]
	Invalid = 0x00,
	
	/// <summary>summon STAT SLOT</summary>
	[Description("")]
	Summon,

	/// <summary>play SLOT SLOT</summary>
	[Description("")]
	Attack,
	
	/// <summary>special STAT</summary>
	[Description("")]
	Special,
	
	/// <summary>target SLOT</summary>
	[Description("")]
	Target,
	
	/// <summary>endturn</summary>
	[Description("")]
	EndTurn,
	
	/// <summary>confirm</summary>
	[Description("")]
	Confirm,
	
	/// <summary>info [concept]</summary>
	[Description("")]
	Info,
	
	/// <summary>set STAT VALUE</summary>
	[Description("")]
	CgImportStats,

	/// <summary>challenge STAT1 STAT2 [user]charactername[/user]</summary>
	[Description("")]
	Challenge,

	/// <summary>accept STAT1 STAT2</summary>
	[Description("")]
	Accept,

	/// <summary>reject</summary>
	[Description("")]
	Reject,
}