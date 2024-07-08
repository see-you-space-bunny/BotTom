using System;

namespace FChatApi.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class FailureResponseAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly string _value;

// This is a positional argument
/// <summary>
/// 
/// </summary>
public FailureResponseAttribute()
{
	this._value    = $"{MyPrefix}: N/A";
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

// This is a positional argument
/// <summary>
/// 
/// </summary>
/// <param name="failure">used to indicate that something went wrong</param>
public FailureResponseAttribute(string failure)
{
	this._value    = failure;
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public string Message   => _value;
public const string MyPrefix = "FailureResponse";
public const string Generic = "I can't do that right now!";
// This is a named argument
/* public int NamedInt { get; set; } */
}
