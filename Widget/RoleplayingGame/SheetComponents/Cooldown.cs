using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleplayingGame.SheetComponents;

internal class Cooldown
{
#region (-) Fields
	readonly TimeSpan _cooldown;
	DateTime _next;
#endregion


#region (+) Properties
	internal bool Ready		=>	DateTime.Now >= _next;
	internal int Remaining	=>	(int)Math.Max((_next-DateTime.Now).TotalMinutes+1,0);
#endregion


#region (+) Trigger
	internal void Trigger()
	{
		_next	= DateTime.Now + _cooldown;
	}
#endregion


#region Constructor 
	internal Cooldown(TimeSpan cooldown,bool beginOnCooldown = false)
	{
		_cooldown	= cooldown;
		if (beginOnCooldown)
		{
			_next		= DateTime.Now;
		}
		else
		{
			Trigger();
		}
	}
#endregion
}