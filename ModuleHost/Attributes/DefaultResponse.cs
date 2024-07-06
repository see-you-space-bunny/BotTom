using ModuleHost.Enums;

namespace ModuleHost.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class DefaultResponseAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly string _defaultSuccess;
readonly string _defaultBadSyntax;
readonly string _defaultFailure;
readonly string _defaultDenied;

// This is a positional argument
/// <summary>
/// 
/// </summary>
/// <param name="defaultSuccess">added to most successful calls</param>
/// <param name="defaultBadSyntax">used to indicate bad command input</param>
/// <param name="defaultFailure">used to indicate that something went wrong</param>
/// <param name="defaultDenied">used to indicate that the user is not privileged for this command</param>
public DefaultResponseAttribute(string defaultSuccess,string defaultBadSyntax,string defaultFailure,string defaultDenied)
{
    this._defaultSuccess    = defaultSuccess;
    this._defaultBadSyntax  = defaultBadSyntax;
    this._defaultFailure    = defaultFailure;
    this._defaultDenied     = defaultDenied;
    
    // TODO: Implement code here
    /* throw new System.NotImplementedException(); */
}

public string Success   => _defaultSuccess;
public string BadSyntax => _defaultBadSyntax;
public string Failure   => _defaultFailure;
public string Denied    => _defaultDenied;

// This is a named argument
/* public int NamedInt { get; set; } */
}
