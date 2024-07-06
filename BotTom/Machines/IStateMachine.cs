using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotTom.Machines;

internal interface IStateMachine
{
    internal bool AtTerminalStage { get; }
    internal bool IsExpired { get; }
}
