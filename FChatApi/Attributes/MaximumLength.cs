using System;

namespace FChatApi.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class MaximumLengthAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly int _value;

// This is a positional argument
public MaximumLengthAttribute(int value)
{
	this._value = value;
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public int Length => _value;

// This is a named argument_value
/* public int NamedInt { get; set; } */
}
