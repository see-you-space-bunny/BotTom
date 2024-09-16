using System.ComponentModel;
using Plugins.Attributes;
using FChatApi.Enums;
using Plugins.Enums;

namespace CardGame.Enums;

//	Begin / End case-insensitive matching
//		(?i) / (?-i)

//	"Command"
//		((?'Command'[a-zA-Z]{1,24})(\s+|\]|$))

//	"Word"
//		((?'Word'[a-zA-Z\-_0-9]{1,24})(\s+|\]|$))

//	"WordArray"
//		((?'WordArray'[a-zA-Z\-_0-9]{1,24}([\,\+]{1}\s*[a-zA-Z]{1,24})*)(\s+|\]|$))

//	"WordNumberArray"
//		((?'WordNumberArray'(?'Word'[a-zA-Z\-_0-9]{1,24})\s(?'Number'[0-9]{1,4}[\,\+]{0,1}))+(\s+|\]|$))

//	"Number"
//		((?'Number'[0-9]{1,7}[\.\,]{0,1}[0-9]{0,4})(\s+|\]|$))

//	"Player"
//		((\[user\]){0,1}((?'Player'[a-zA-Z\-_\s0-9]{1,32})(\[\/user\]){0,1})(\s+|\]|$))

public enum CardGameCommand
{
	Invalid = 0x00,
	
	/// <summary>summon STAT SLOT</summary>
	[Description("Once on your turn you may use {0}summon STAT SLOT to create a combattant on your side of the field. You have 3 slots to choose from and cannot summon into an occupied slot. (Example:[spoiler] !summon INT 1[/spoiler])")]
	[CommandPattern(@"(?i)(?'Stat'[a-zA-Z\-_0-9]{3,24})(\s+(?'Slot'[0-3])){0,1}(\s+(?'CardName'[a-zA-Z\ \-_0-9]{1,24}))[\s+\]$]")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.ActiveChannel)]
	Summon,

	/// <summary>play SLOT SLOT</summary>
	[Description("")]
	[CommandPattern(@"(?'Slot1'[0-3])\s+(?'Slot2'[0-3])[\s+\]$]")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.ActiveChannel)]
	Attack,
	
	/// <summary>special STAT</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.ActiveChannel)]
	Special,
	
	/// <summary>target SLOT</summary>
	[Description("")]
	[CommandPattern(@"(?'Slot2'[0-3])[\s+\]$]")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.ActiveChannel)]
	Target,
	
	/// <summary>endturn</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.ActiveChannel)]
	EndTurn,
	
	/// <summary>confirm</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.ActiveChannel)]
	Confirm,
	
	/// <summary>info TOPIC</summary>
	[Description("")]
	[CommandPattern(@"(?i)(?'Topic'[a-zA-Z\-_0-9]{1,24})[\s+\]$]")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.ActiveChannel)]
	[CommandAlias("help")]
	Info,
	
	/// <summary>cgimportstats STATBLOCK</summary>
	[Description("")]
	[CommandPattern(@"(?i)Level (?'Level'[\-]{0,1}[0-9]{1,5})(.*\n)+\s+\[color=orange\]STR.*\](?'STR'[\-]{0,1}[0-9]{1,5})[↑↓]{0,1}\[/.*\[color=yellow\]VIT.*\](?'VIT'[\-]{0,1}[0-9]{1,5})[↑↓]{0,1}\[/.*\[color=red\]DEX.*\](?'DEX'[\-]{0,1}[0-9]{1,5})[↑↓]{0,1}\[/.*\n\s+\[color=cyan\]INT.*\](?'INT'[\-]{0,1}[0-9]{1,5})[↑↓]{0,1}\[/.*\[color=pink\]CHA.*\](?'CHA'[\-]{0,1}[0-9]{1,5})[↑↓]{0,1}\[/.*\[color=green\]LUC.*\](?'LUC'[\-]{0,1}[0-9]{1,5})[↑↓]{0,1}\[/")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Whisper)]
	[CommandAlias("cgimport")]
	CgImportStats,

	/// <summary>challenge STAT1 STAT2 CHARACTERNAME</summary>
	[Description("")]
	[CommandPattern(@"(?i)(?'Stat1'[a-z]{3,24})\s+(?'Stat2'[a-z]{3,24})\s+(\[user\]){0,1}(?'CharacterName'[a-z0-9\-\ ]{2,32})(\[\/user\]){0,1}[\s+\]$]")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Whisper)]
	[CommandAlias("chal")]
	Challenge,

	/// <summary>accept STAT1 STAT2</summary>
	[Description("")]
	[CommandPattern(@"(?i)(?'Stat1'[a-z]{1,24})\s+(?'Stat2'[a-z]{1,24})[\s+\]$]")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Whisper)]
	[CommandAlias("yes","ok")]
	Accept,

	/// <summary>reject</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	[UsageScope(CommandScope.Whisper)]
	[CommandAlias("no","decline")]
	Reject,
}