using System;
using System.Text.RegularExpressions;
using FChatApi.Enums;
using Plugins.Enums;

namespace Plugins.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class CommandAliasAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly string[] _values;

// This is a positional argument
public CommandAliasAttribute(params string[] values)
{
	this._values = values;
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public string[] Values => _values;

// This is a named argument
/* public int NamedInt { get; set; } */
}
