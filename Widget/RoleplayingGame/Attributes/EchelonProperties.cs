using RoleplayingGame.Enums;

namespace RoleplayingGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class EchelonPropertiesAttribute : Attribute
{
		// See the attribute guidelines at
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly GameAction _signatureAction;
		readonly short _tier;
		
		// This is a positional argument
		public EchelonPropertiesAttribute(short tier,GameAction signatureAction)
		{
				this._tier = tier;
				this._signatureAction	= signatureAction;
				// TODO: Implement code here
				/* throw new System.NotImplementedException(); */
		}
		
		public GameAction SignatureAction
		{
				get { return this._signatureAction; }
		}
		
		public short Tier
		{
				get { return this._tier; }
		}
		
		// This is a named argument
		/* public int NamedInt { get; set; } */
}