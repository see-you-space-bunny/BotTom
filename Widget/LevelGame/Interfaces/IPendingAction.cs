using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;

namespace LevelGame.Interfaces;

internal interface IPendingAction
{
    internal void ExecuteEffect();
    internal User Responder { get; }
    internal Channel? Channel { get; }
}