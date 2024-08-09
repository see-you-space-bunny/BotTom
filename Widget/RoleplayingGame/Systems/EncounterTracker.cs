using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using RoleplayingGame.Contexts;
using RoleplayingGame.Interfaces;
using RoleplayingGame.Objects;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Systems;

internal class EncounterTracker : ObjectManager<BaseContext>, IObjectManager, IList<BaseContext>
{
#region (-) Fields
	private IEnumerable<CombatContext> ActiveCombats	=> _objects.OfType<CombatContext>();
#endregion


#region (~) Get
	internal TEncounter[] GetEncounters<TEncounter>(User value) where TEncounter : BaseContext	=>
		[.. _objects.OfType<TEncounter>().Where(li=>li.HasParticipant(value))];
	internal bool TryGetEncounters<TEncounter>(User value,out TEncounter[] result) where TEncounter : BaseContext
	{
		var encounters	=	_objects.OfType<TEncounter>().Where(li=>li.HasParticipant(value));
		if (encounters.Any())
		{
			result = [.. encounters];
			return true;
		}
		result	= [];
		return false;
	}
#endregion


#region (~) New
	internal CombatContext NewCombatEncounter(Actor[] values = null!)
	{
		values ??= [];
		CombatContext result = new CombatContext()
			.WithParticipants(values);
		_objects.Add(result);
		return result;
	}
#endregion


#region Constructor
	internal EncounterTracker()
	{ }
#endregion
}