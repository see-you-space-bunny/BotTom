using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;
using RoleplayingGame.Enums;
using RoleplayingGame.Interfaces;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Contexts;

internal class BaseContext : IContext<BaseContext>
{
#region (#) Fields
	protected List<Actor>	_participants;
	protected ContextState	_state;
#endregion


#region IContext
	public bool HasNpcParticipant()	=>
		_participants.Any(li=>li is NonPlayerEnemy);

	public TParticipant FirstNpcParticipant<TParticipant>() where TParticipant : Actor	=>
		(_participants.First(li=>li is NonPlayerEnemy) as TParticipant)!;

	public bool HasParticipant(Actor value)	=>
		_participants.Any(li=>li==value);
	
	public bool HasParticipant(User value)	=>
		_participants.OfType<CharacterSheet>().Any(li=>li.User==value);

	public BaseContext WithParticipant(Actor value)
	{
		_participants.Add(value);
		return this;
	}

	public BaseContext WithParticipants(params Actor[] values)
	{
		foreach (Actor value in values)
		{
			_participants.Add(value);
		}
		return this;
	}

	public BaseContext EnqueueMessage(ApiConnection api)
	{
		var message = new FChatMessageBuilder();
		
		api.EnqueueMessage(message);
		return this;
	}
#endregion


#region Constructor
	internal BaseContext()
	{
		_state			= ContextState.Initialized;
		_participants	= [];
	}
#endregion
}