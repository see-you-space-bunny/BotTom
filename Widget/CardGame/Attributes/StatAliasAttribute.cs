namespace Widget.CardGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class StatAliasAttribute : Attribute
{
	// See the attribute guidelines at
	//  http://go.microsoft.com/fwlink/?LinkId=85236
	readonly string[] _alias;
	
	// This is a positional argument
	public StatAliasAttribute(string[] alias)
	{
		this._alias = alias;
		
		// TODO: Implement code here
		/* throw new System.NotImplementedException(); */
	}
	
	public string[] Alias => _alias;
	
	// This is a named argument
	/* public int NamedInt { get; set; } */

}