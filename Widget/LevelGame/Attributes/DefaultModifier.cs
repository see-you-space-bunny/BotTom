namespace RoleplayingGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class DefaultModifierAttribute : Attribute
{
		// See the attribute guidelines at
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly float _value;
		
		// This is a positional argument
		public DefaultModifierAttribute(float value)
		{
				this._value = value;
				
				// TODO: Implement code here
				/* throw new System.NotImplementedException(); */
		}
		
		public float Value
		{
				get { return this._value; }
		}
		
		// This is a named argument
		/* public int NamedInt { get; set; } */
}