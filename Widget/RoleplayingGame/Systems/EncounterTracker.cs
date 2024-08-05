using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RoleplayingGame.Contexts;

namespace RoleplayingGame.Systems;

public class EncounterTracker
{
	private readonly List<CombatContext> _activeCombats;

	internal CombatContext NewCombatEncounter()
	{
		CombatContext result = new ();
		_activeCombats.Add(result);
		return result;
	}

	internal EncounterTracker()
	{
		_activeCombats	= [];
	}
}