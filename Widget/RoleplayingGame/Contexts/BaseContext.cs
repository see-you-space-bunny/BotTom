using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;
using RoleplayingGame.Enums;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Contexts;

public class BaseContext
{
	protected List<Actor> _participants;
	protected ContextState _state;

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

	
	internal BaseContext EnqueueMessage(ApiConnection api)
	{
		var message = new FChatMessageBuilder();
		
		api.EnqueueMessage(message);
		return this;
	}

	public BaseContext()
	{
		_state			= ContextState.Initialized;
		_participants	= [];
	}
}