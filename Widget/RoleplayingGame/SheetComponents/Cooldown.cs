using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoleplayingGame.SheetComponents;

internal class Cooldown
{
#region (-) Fields
	readonly TimeSpan	_cooldown;
	readonly ushort		_capacity;
#endregion


#region (-) Fields
	DateTime	_next;
#endregion


#region (+) Properties
	internal ushort	Charges		=>	(ushort)Math.Min((DateTime.Now - _next) / _cooldown,_capacity);
	internal ushort	Capacity	=>	_capacity;
	internal bool	AtCapacity	=>	Charges	>=	_capacity;
	internal int	Remaining	=>	(int)Math.Max((_next-DateTime.Now).TotalMinutes+1,0);
	internal bool	Ready		=>	DateTime.Now	>=	_next;
#endregion


#region (+) Trigger
	internal void Trigger()
	{
		if (AtCapacity)
		{
			_next	= DateTime.Now - (_cooldown * (_capacity - 1));
		}
		else
		{
			_next	+= _cooldown;
		}
	}
#endregion


#region (+) ToString
	public override string ToString()
	{
		return base.ToString()!;
	}
#endregion


#region Serialization
	public static Cooldown Deserialize(BinaryReader reader)
	{
		Cooldown result = new (new TimeSpan(reader.ReadInt64()),reader.ReadUInt16())
			{ _next = new DateTime(reader.ReadInt64()) };
		return result;
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((long)		_cooldown.Ticks);
		writer.Write((ushort)	_capacity);
		writer.Write((long)		_next.Ticks);
	}
#endregion


#region Constructor
	internal Cooldown(TimeSpan cooldown,ushort capacity = 1,bool beginOnCooldown = false)
	{
		_cooldown	=	cooldown;
		_capacity	=	capacity;
		if (beginOnCooldown)
		{
			Trigger();
		}
		else
		{
			_next		= DateTime.Now;
		}
	}
#endregion
}