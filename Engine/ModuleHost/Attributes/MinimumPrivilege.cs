using Engine.ModuleHost.Enums;

namespace Engine.ModuleHost.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class MinimumPrivilegeAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly Privilege _privilege;

// This is a positional argument
public MinimumPrivilegeAttribute(Privilege privilege)
{
    this._privilege = privilege;
    
    // TODO: Implement code here
    /* throw new System.NotImplementedException(); */
}

public Privilege Privilege => _privilege;

// This is a named argument
/* public int NamedInt { get; set; } */
}
