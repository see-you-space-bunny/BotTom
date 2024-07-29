using System.ComponentModel;
using FChatApi.Attributes;
using FChatApi.Enums;
using Plugins.Attributes;

namespace FGlobals.Enums;

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
