using ChatApi;
using ChatApi.Objects;
using ModuleHost.CommandHandling;
using Discord;
using Discord.WebSocket;

namespace ModuleHost;

/// <summary>
/// our discord plugin for others to derive off of
/// </summary>
/// <param name="updateInterval">how often this module runs Update().<br/>defaults to: Never</param>
public class DiscordSlashPlugin(TimeSpan? updateInterval = null) : PluginBase(updateInterval)
{
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    /// <summary>
    /// how we should handle a recieved command
    /// </summary>
    /// <param name="command">the slash command context</param>
    public async virtual Task HandleRecievedMessage(SocketSlashCommand command) { }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}