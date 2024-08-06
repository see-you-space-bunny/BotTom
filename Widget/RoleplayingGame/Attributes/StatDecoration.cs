using FChatApi.Enums;

namespace RoleplayingGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class StatDecorationAttribute : Attribute
{
	// See the attribute guidelines at
	//  http://go.microsoft.com/fwlink/?LinkId=85236
	readonly string _value1;
	readonly BBCodeColor _value2;

	// This is a positional argument
	public StatDecorationAttribute(string emoji,BBCodeColor color)
	{
		this._value1 = emoji;
		this._value2 = color;
		
		// TODO: Implement code here
		/* throw new System.NotImplementedException(); */
	}

	public string Emoji => _value1;
	public BBCodeColor Color => _value2;

	// This is a named argument
	/* public int NamedInt { get; set; } */
}