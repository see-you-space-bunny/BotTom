using System.ComponentModel;
using FChatApi.Attributes;
using FChatApi.Enums;
using Plugins.Attributes;

namespace FGlobals.Enums;

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

public enum GlobalCommand
{
	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	UnRegister,

	[Description("Registers the user with the bot and allows them to use other commands.")]
	[MinimumPrivilege(Privilege.UnregisteredUser)]
	Register,

	[Description("")]
	[MinimumPrivilege(Privilege.RegisteredUser)]
	Whoami,

	[Description("")]
	[MinimumPrivilege(Privilege.GlobalOperator)]
	ChInvite,

	[Description("")]
	[MinimumPrivilege(Privilege.GlobalOperator)]
	ChCreate,

	[Description("")]
	[MinimumPrivilege(Privilege.OwnerOperator)]
	Promote,

	[Description("")]
	[MinimumPrivilege(Privilege.OwnerOperator)]
	Demote,

	[Description("")]
	[MinimumPrivilege(Privilege.OwnerOperator)]
	Shutdown,
}
