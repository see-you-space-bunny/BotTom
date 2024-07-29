using System;
using System.Text.RegularExpressions;
using FChatApi.Enums;
using Plugins.Enums;

namespace Plugins.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class CommandPatternAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly Regex _pattern;

// This is a positional argument
public CommandPatternAttribute(string pattern)
{
	this._pattern = new Regex(pattern);
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public Regex Pattern => _pattern;

// This is a named argument
/* public int NamedInt { get; set; } */
}
