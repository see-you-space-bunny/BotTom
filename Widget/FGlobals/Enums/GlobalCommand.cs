using System.ComponentModel;
using Engine.ModuleHost;
using Engine.ModuleHost.Attributes;
using Engine.ModuleHost.Enums;

namespace Widget.FGlobals.Enums;

public enum GlobalCommand
{
    [Description("Registers the user with the bot and allows them to use other commands.")]
    [MinimumPrivilege(Privilege.UnregisteredUser)]
    [SuccessResponse("You have successfully registered! Welcome!")]
    [FailureResponse("You are already registered!")]
    [AccessDeniedResponse]
    Register,

    [Description("")]
    [MinimumPrivilege(Privilege.RegisteredUser)]
    [SuccessResponse("You have successfully Unregistered! Goodbye!")]
    [FailureResponse("You are not registered! You can register with \"tom! register\"")]
    [AccessDeniedResponse]
    UnRegister,

    [Description("")]
    [MinimumPrivilege(Privilege.RegisteredUser)]
    [SuccessResponse]
    [FailureResponse("I don't know who you are. You can register with \"tom! register\"")]
    [AccessDeniedResponse]
    Whoami,

    [Description("")]
    [MinimumPrivilege(Privilege.OwnerOperator)]
    [SuccessResponse("Shutting down!")]
    [FailureResponse]
    [AccessDeniedResponse("You do not have permission to do that!")]
    Shutdown,
}
