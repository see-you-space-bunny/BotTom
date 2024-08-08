using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;
using RoleplayingGame.Enums;

namespace RoleplayingGame.Interfaces;

internal interface IPendingEvent
{
    internal IPendingEvent WithInitiator(User value);
    internal IPendingEvent WithResponder(User value);
    internal IPendingEvent WithChannel(Channel value);
    internal IPendingEvent EnqueueMessage(ApiConnection api);
	internal RoleplayingGameCommand[] ExpectedResponses { get; }
    internal User? Initiator { get; }
    internal User? Responder { get; }
    internal Channel? Channel { get; }
	internal bool RequiresResponse => Responder is not null;
	internal bool IsPlayerInitiated => Initiator is not null;
}