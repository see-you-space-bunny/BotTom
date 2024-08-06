using RoleplayingGame.Enums;

namespace RoleplayingGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class DerivedAbilityInfoAttribute : Attribute
{
		// See the attribute guidelines at
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly Ability[] _abilities;
		readonly AbilityGroup _group;
		readonly AbilityType _type;
		
		// This is a positional argument
		public DerivedAbilityInfoAttribute(Ability[] abilities, AbilityGroup group,AbilityType type)
		{
				this._abilities	= abilities;
				this._group		= group;
				this._type		= type;
				
				// TODO: Implement code here
				/* throw new System.NotImplementedException(); */
		}
		
		public Ability[] Abilities
		{
				get { return this._abilities; }
		}

		public AbilityGroup Group
		{
				get { return this._group; }
		}
		
		public AbilityType Type
		{
				get { return this._type; }
		}
		
		// This is a named argument
		/* public int NamedInt { get; set; } */
}