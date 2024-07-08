namespace BotTom.Machines;

internal interface IStateMachine
{
	internal bool AtTerminalStage { get; }
	internal bool IsExpired { get; }
}
