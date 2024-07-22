using LevelGame.Enums;

namespace LevelGame.Attributes;

[AttributeUsage(AttributeTargets.All)]
sealed class ActionPropertiesAttribute : Attribute
{
		// See the attribute guidelines at
		//  http://go.microsoft.com/fwlink/?LinkId=85236
		readonly bool _nameTagged;
		readonly float _cooldown;
		readonly GameAction _sharesCooldownWith;
		
		// This is a positional argument
		/// <param name="nameTagged"></param>
		/// <param name="cooldown">measured in minutes<br/>&gt; 60.0f = 1 hour<br/>&gt; 240.0f = 4 hours<br/>&gt; 1200.0f = 20 hours<br/>&gt; 9360.0f = ~1 week</param>
		public ActionPropertiesAttribute(bool nameTagged = false,float cooldown = 0.0f,GameAction sharesCooldownWith = GameAction.None)
		{
				this._nameTagged			= nameTagged;
				this._cooldown				= cooldown;
				this._sharesCooldownWith	= sharesCooldownWith;
				
				// TODO: Implement code here
				/* throw new System.NotImplementedException(); */
		}
		
		public float Cooldown
		{
				get { return this._cooldown; }
		}
		
		public bool NameTagged
		{
				get { return this._nameTagged; }
		}
		
		public GameAction SharesCooldownWith
		{
				get { return this._sharesCooldownWith; }
		}
		
		// This is a named argument
		/* public int NamedInt { get; set; } */
}