using System.ComponentModel;
using Plugins.Attributes;

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
	[CommandPattern(@"(?i)((?'Stats'[a-zA-Z\-_0-9]{1,24})\s+){2}((\[user\]){0,1}((?'Player'[a-z0-9\-\ ]{2,32})(\[\/user\]){0,1})(\s+|\]|$))")]
	Challenge,

	/// <summary>accept STAT1 STAT2</summary>
	[Description("")]
	Accept,

	/// <summary>reject</summary>
	[Description("")]
	Reject,
}