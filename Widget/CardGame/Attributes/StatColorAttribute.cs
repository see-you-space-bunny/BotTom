using CardGame.Enums;
using FChatApi.Enums;

namespace CardGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class StatColorAttribute : Attribute
{
// See the attribute guidelines at
//  http://go.microsoft.com/fwlink/?LinkId=85236
readonly BBCodeColor _value;

// This is a positional argument
public StatColorAttribute(BBCodeColor color)
{
	this._value = color;
	
	// TODO: Implement code here
	/* throw new System.NotImplementedException(); */
}

public BBCodeColor Color => _value;

// This is a named argument
/* public int NamedInt { get; set; } */
}