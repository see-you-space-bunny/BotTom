using ChatApi.Objects;
using ModuleHost.CardiApi;

namespace ModuleHost.CommandHandling;

public class BotCommand(User user,Channel? channel,BotModule botModule, string command, string[] parameters,bool isOp)
{
    public User? User { get; } = user;
    public bool IsRegisteredUser => User is RegisteredUser;
    public bool IsOp { get; } = isOp;
    public Channel? Channel { get; } = channel;
    public BotModule BotModule { get; } = botModule;
    public string Command { get; } = command;
    public string[] Parameters { get; } = parameters;
}

