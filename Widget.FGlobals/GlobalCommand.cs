using System.ComponentModel;
using ModuleHost;
using ModuleHost.Attributes;
using ModuleHost.Enums;

namespace Widget.FGlobals.Enums;

public enum GlobalCommand
{
    [Description("")]
    [MinimumPrivilege(Privilege.UnregisteredUser)]
    [DefaultResponse("You have successfully registered! Welcome!","Bad Syntax!","Something went wrong!","You are already registered!")]
    Register,

    [Description("")]
    [MinimumPrivilege(Privilege.RegisteredUser)]
    [DefaultResponse("You have successfully Unregistered! Goodbye!","Bad Syntax!","Something went wrong!","You are not registered!")]
    UnRegister,

    [Description("")]
    [MinimumPrivilege(Privilege.None)]
    [DefaultResponse("This is what I know about you: ","Bad Syntax!","Something went wrong!","I don't know who you are. You can register with \"tom! register\"")]
    Whoami,

    [Description("")]
    [MinimumPrivilege(Privilege.None)]
    [DefaultResponse("This is what I know about you: ","Bad Syntax!","Something went wrong!","I don't know who you are. You can register with \"tom! register\"")]
    Me,

    [Description("")]
    [MinimumPrivilege(Privilege.OwnerOperator)]
    [DefaultResponse("Shutting down!","Bad Syntax!","Something went wrong!","You do not have permission to do that!")]
    Shutdown,
}
