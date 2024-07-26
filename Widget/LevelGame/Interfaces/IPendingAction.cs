using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;

namespace LevelGame.Interfaces;

internal interface IPendingAction
{
    internal IPendingAction WithResponder(User value);
    internal IPendingAction WithChannel(Channel value);
    internal IPendingAction ExecuteEffect();
    internal IPendingAction EnqueueMessage(ApiConnection api);
    internal User? Responder { get; }
    internal Channel? Channel { get; }
	internal bool RequiresResponse => Responder is not null && Responder != default;
}