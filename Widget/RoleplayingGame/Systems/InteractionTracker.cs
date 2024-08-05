using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using RoleplayingGame.Effects;
using RoleplayingGame.Interfaces;

namespace RoleplayingGame.Systems;

internal class InteractionTracker
{
#region Fields
	private readonly List<IPendingEvent> PendingEvents;
#endregion


#region Porperties
	internal int Count => PendingEvents.Count;
#endregion


#region Add/Remove
	internal void Add(IPendingEvent value) => PendingEvents.Add(value);
	internal void Remove(IPendingEvent value) => PendingEvents.Remove(value);
#endregion


#region TryGetEvents
	internal TEvent FirstPendingEventByInitiator<TEvent>(User user) where TEvent : IPendingEvent =>
		PendingEvents.OfType<TEvent>().Where(a=>a.IsPlayerInitiated && a.Initiator == user).First();
	internal TEvent FirstPendingEventByResponder<TEvent>(User user) where TEvent : IPendingEvent =>
		PendingEvents.OfType<TEvent>().Where(a=>a.RequiresResponse && a.Responder == user).First();
#endregion


#region TryGetEvents
	internal bool TryGetPendingEventsByInitiator<TEvent>(User user,out TEvent[] result) where TEvent : IPendingEvent
	{
		result = [.. PendingEvents.OfType<TEvent>().Where(a=>a.IsPlayerInitiated && a.Initiator == user)];
		return result.Length > 0;
	}
	internal bool TryGetPendingEventsByResponder<TEvent>(User user,out TEvent[] result) where TEvent : IPendingEvent
	{
		result = [.. PendingEvents.OfType<TEvent>().Where(a=>a.RequiresResponse && a.Responder == user)];
		return result.Length > 0;
	}
#endregion


#region Constructor
	internal InteractionTracker()
	{
		PendingEvents = [];
	}
#endregion
}