namespace RoleplayingGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class XmlKeyAttribute : Attribute
{
		// See the attribute guidelines at
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly string _value;
		
		// This is a positional argument
		public XmlKeyAttribute(string value)
		{
				this._value = value;
				
				// TODO: Implement code here
				/* throw new System.NotImplementedException(); */
		}
		
		public string Value
		{
				get { return this._value; }
		}
		
		// This is a named argument
		/* public int NamedInt { get; set; } */
}