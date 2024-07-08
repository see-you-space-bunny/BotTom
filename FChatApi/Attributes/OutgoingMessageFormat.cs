using System;

namespace FChatApi.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class OutgoingMessageFormatAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly string _value;

// This is a positional argument
/// <summary>
/// 
/// </summary>
/// <param name="failure">used to indicate that something went wrong</param>
public OutgoingMessageFormatAttribute(string format)
{
	this._value    = format;
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public string Format   => _value;
// This is a named argument
/* public int NamedInt { get; set; } */
}
