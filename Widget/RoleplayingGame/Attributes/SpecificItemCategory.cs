using FChatApi.Enums;
using RoleplayingGame.Enums;

namespace RoleplayingGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class SpecificItemCategoryAttribute : Attribute
{
		// See the attribute guidelines at
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly ItemCategory _value;
		
		// This is a positional argument
		public SpecificItemCategoryAttribute(ItemCategory value)
		{
				this._value = value;
				
				// TODO: Implement code here
				/* throw new System.NotImplementedException(); */
		}
		
		public ItemCategory Category
		{
			get { return this._value; }
		}
		
		// This is a named argument
		/* public int NamedInt { get; set; } */
}