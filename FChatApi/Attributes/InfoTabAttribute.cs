using System;
using FChatApi.Enums;

namespace FChatApi.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class InfoTabAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly ProfileInfoTab _value;

// This is a positional argument
public InfoTabAttribute(ProfileInfoTab value)
{
	this._value = value;
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public ProfileInfoTab Tab => _value;

// This is a named argument_value
/* public int NamedInt { get; set; } */
}
