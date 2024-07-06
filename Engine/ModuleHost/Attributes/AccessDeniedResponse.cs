using Engine.ModuleHost.Enums;

namespace Engine.ModuleHost.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class AccessDeniedResponseAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly string _value;

// This is a positional argument
/// <summary>
/// 
/// </summary>
public AccessDeniedResponseAttribute()
{
	this._value     = $"{MyPrefix}: N/A";
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

// This is a positional argument
/// <summary>
/// 
/// </summary>
/// <param name="accessDenied">used to indicate that the user is not privileged for this command</param>
public AccessDeniedResponseAttribute(string accessDenied)
{
	this._value     = accessDenied;
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public string Message    => _value;

private const string MyPrefix = "AccessDeniedResponse";
public const string BanTimeout = "If you are reading this message, you have been denied access to the bot! Do not attempt to cirvumvent this.";
public const string Generic = "You do not have permission to do that.";

// This is a named argument
/* public int NamedInt { get; set; } */
}
