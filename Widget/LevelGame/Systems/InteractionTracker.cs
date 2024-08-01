using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using RoleplayingGame.Effects;
using RoleplayingGame.Interfaces;

namespace LevelGame.Systems;

public class InteractionTracker
{
#region Fields
	private readonly List<IPendingEvent> PendingEvents;
	private IEnumerable<IPendingEvent> EventAwaitingResponses => PendingEvents.Where(a=>a.RequiresResponse);
#endregion


#region Porperties
	public int Count => PendingEvents.Count;
#endregion


#region GetEvents
	internal bool TryGetPendingEventsByUser(User user,out IPendingEvent[] result)
	{
		result = [.. PendingEvents.Where(a=>a.RequiresResponse && a.Responder == user)];
		return result.Length > 0;
	}
	internal bool TryGetPendingEventsByUser<TEventType>(User user,out TEventType[] result) where TEventType : IPendingEvent
	{
		result = [.. PendingEvents.OfType<TEventType>().Where(a=>a.RequiresResponse && a.Responder == user)];
		return result.Length > 0;
	}
#endregion


#region Constructor
	public InteractionTracker()
	{
		PendingEvents = [];
	}
#endregion
}