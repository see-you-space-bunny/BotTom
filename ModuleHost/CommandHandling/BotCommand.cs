using ChatApi.Objects;
using ModuleHost.CardiApi;
using ModuleHost.Enums;

namespace ModuleHost.CommandHandling;

public class BotCommand(string username,RegisteredUser? user,Channel? channel,BotModule botModule, string command, string[] parameters, Privilege privilege)
{
    public string UserName { get; } = username;
    public RegisteredUser? User { get; } = user;
    public Channel? Channel { get; } = channel;
    public BotModule BotModule { get; } = botModule;
    public string Command { get; } = command;
    public string[] Parameters { get; } = parameters;
    public Privilege PrivilegeLevel { get; } = privilege; 
}

