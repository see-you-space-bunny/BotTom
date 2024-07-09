using System;

namespace FChatApi.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class SuccessResponseAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly string _value;

// This is a positional argument
/// <summary>
/// 
/// </summary>
/// <param name="success">added to most successful calls</param>
public SuccessResponseAttribute()
{
	this._value    = $"{MyPrefix}: ";
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

// This is a positional argument
/// <summary>
/// 
/// </summary>
/// <param name="success">added to most successful calls</param>
public SuccessResponseAttribute(string success)
{
	this._value    = success;
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public string Message   => _value;
public const string MyPrefix = "SuccessResponse";
public const string Generic = "Working. . .";

// This is a named argument
/* public int NamedInt { get; set; } */
}
