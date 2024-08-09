using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using RoleplayingGame.Effects;
using RoleplayingGame.Interfaces;
using RoleplayingGame.SheetComponents;

namespace RoleplayingGame.Systems;

internal class InteractionTracker : ObjectManager<IPendingEvent>, IObjectManager, IList<IPendingEvent>
{
#region TryGetEvents
	internal TEvent FirstPendingEventByInitiator<TEvent>(User user) where TEvent : IPendingEvent =>
		_objects.OfType<TEvent>().Where(a=>a.IsPlayerInitiated && a.Initiator == user).First();
	internal TEvent FirstPendingEventByResponder<TEvent>(User user) where TEvent : IPendingEvent =>
		_objects.OfType<TEvent>().Where(a=>a.RequiresResponse && a.Responder == user).First();
#endregion


#region TryGetEvents
	internal bool TryGetPendingEventsByInitiator<TEvent>(User user,out TEvent[] result) where TEvent : IPendingEvent
	{
		result = [.. _objects.OfType<TEvent>().Where(a=>a.IsPlayerInitiated && a.Initiator == user)];
		return result.Length > 0;
	}
	internal bool TryGetPendingEventsByResponder<TEvent>(User user,out TEvent[] result) where TEvent : IPendingEvent
	{
		result = [.. _objects.OfType<TEvent>().Where(a=>a.RequiresResponse && a.Responder == user)];
		return result.Length > 0;
	}
#endregion


#region Constructor
	internal InteractionTracker() : base()
	{ }
#endregion
}