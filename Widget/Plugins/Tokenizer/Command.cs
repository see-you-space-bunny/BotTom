using System.ComponentModel;
using Plugins.Attributes;
using FChatApi.Enums;

namespace Plugins.Tokenizer;

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
//		((?'Number'[0-9]{1,7}[\.\,]{0,1}[0-9]{1,4})(\s+|\]|$))

//	"Player"
//		((\[user\]){0,1}((?'Player'[a-zA-Z\-_\s0-9]{1,32})(\[\/user\]){0,1})(\s+|\]|$))

public enum Command
{
	/// <summary></summary>
	[Description("")]
	[MinimumPrivilege(Privilege.None)]
	Invalid = 0x00,
	
	/// <summary>summon STAT SLOT</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Summon,

	/// <summary>attack SLOT SLOT</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Attack,
	
	/// <summary>special STAT</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Special,
	
	/// <summary>target SLOT</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Target,
	
	/// <summary>endturn</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	EndTurn,
	
	/// <summary>confirm</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Confirm,
	
	/// <summary>info [concept]</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Info,
	
	/// <summary>set STAT VALUE</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	CgImportStats,

	/// <summary>challenge STAT1 STAT2 [user]charactername[/user]</summary>
	[Description("")]
	//[UsedByModule(Module.CardGame,,true)]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Challenge,

	/// <summary>accept STAT1 STAT2</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Accept,

	/// <summary>reject</summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Reject,

	/// <summary></summary>
	[Description("Registers the user with the bot and allows them to use other commands.")]
	[MinimumPrivilege(Privilege.UnregisteredUser)]
	Register,

	/// <summary></summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	UnRegister,

	/// <summary></summary>
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Whoami,

	/// <summary></summary>
	[Description("")]
	[MinimumPrivilege(Privilege.GlobalOperator)]
	OpInvite,

	/// <summary></summary>
	[Description("")]
	[MinimumPrivilege(Privilege.GlobalOperator)]
	OpChCreate,

	/// <summary></summary>
	[Description("")]
	[MinimumPrivilege(Privilege.OwnerOperator)]
	OpPromote,

	/// <summary></summary>
	[Description("")]
	[MinimumPrivilege(Privilege.OwnerOperator)]
	OpDemote,

	/// <summary></summary>
	[Description("")]
	[MinimumPrivilege(Privilege.OwnerOperator)]
	Shutdown,
}
