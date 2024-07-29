using System;
using FChatApi.Enums;
using Plugins.Enums;

namespace Plugins.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class UsageScopeAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly CommandScope _scope;

// This is a positional argument
public UsageScopeAttribute(CommandScope scope)
{
	this._scope = scope;
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public CommandScope Scope => _scope;

// This is a named argument
/* public int NamedInt { get; set; } */
}
