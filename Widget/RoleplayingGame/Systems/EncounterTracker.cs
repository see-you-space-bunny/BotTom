using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using RoleplayingGame.Contexts;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Systems;

internal class EncounterTracker
{
#region (-) Fields
	private readonly List<CombatContext> _activeCombats;
#endregion


#region (~) Get
	internal IEnumerable<CombatContext> GetCombatEncountersByUser(User value)	=>
		_activeCombats.Where(li=>li.HasParticipant(value));
#endregion


#region (~) New
	internal CombatContext NewCombatEncounter(Actor[] values = null!)
	{
		values ??= [];
		CombatContext result = new CombatContext()
			.WithParticipants(values);
		_activeCombats.Add(result);
		return result;
	}
#endregion


#region Constructor
	internal EncounterTracker()
	{
		_activeCombats	= [];
	}
#endregion
}