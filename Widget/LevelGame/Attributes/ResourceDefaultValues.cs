using RoleplayingGame.Enums;

namespace RoleplayingGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class ResourceDefaultValuesAttribute : Attribute
{
		// See the attribute guidelines at
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly int _baseValue;
		readonly int _hardLimit;
		readonly int _softLimit;
		readonly bool _moreIsBetter;
		
		// This is a positional argument
		/// <param name="nameTagged"></param>
		/// <param name="cooldown">measured in minutes<br/>&gt; 60.0f = 1 hour<br/>&gt; 240.0f = 4 hours<br/>&gt; 1200.0f = 20 hours<br/>&gt; 9360.0f = ~1 week</param>
		public ResourceDefaultValuesAttribute(int baseValue = 0,int hardLimit = -1,int softLimit = -1,bool moreIsBetter = true)
		{
				this._baseValue		= baseValue;
				this._hardLimit		= hardLimit;
				this._softLimit		= softLimit;
				this._moreIsBetter	= moreIsBetter;
				
				// TODO: Implement code here
				/* throw new System.NotImplementedException(); */
		}
		
		public int BaseValue
		{
				get { return this._baseValue; }
		}
		
		public int HardLimit
		{
				get { return this._hardLimit; }
		}
		
		public int SoftLimit
		{
				get { return this._softLimit; }
		}
		
		public bool MoreIsBetter
		{
				get { return this._moreIsBetter; }
		}
		
		// This is a named argument
		/* public int NamedInt { get; set; } */
}