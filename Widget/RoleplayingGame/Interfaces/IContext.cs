using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Interfaces;

internal interface IContext<TContext>
{
	internal bool HasParticipant(Actor value);
	internal bool HasParticipant(User value);
	internal TContext WithParticipant(Actor value);
	internal TContext WithParticipants(Actor[] value);
	internal TContext EnqueueMessage(ApiConnection api);
}