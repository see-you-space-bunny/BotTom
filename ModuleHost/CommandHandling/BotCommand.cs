namespace ModuleHost.CommandHandling;

public class BotCommand(BotModule botModule, string command, string[] parameters)
{
    public BotModule BotModule { get; } = botModule;
    public string Command { get; } = command;
    public string[] Parameters { get; } = parameters;
}

