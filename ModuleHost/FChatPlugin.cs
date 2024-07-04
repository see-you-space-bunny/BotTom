using ChatApi.Core;
using ChatApi.Objects;
using ModuleHost.CommandHandling;

namespace ModuleHost;

/// <summary>
/// our fchat plugin for others to derive off of
/// </summary>
/// <remarks>
/// sets our api connection in the constructor
/// </remarks>
/// <param name="api"></param>
/// <param name="commandChar">the symbol that wakes the module up</param>
/// <param name="updateInterval">how often this module runs Update().<br/>defaults to: Never</param>
public class FChatPlugin(ApiConnection api, Channel[] activeChannels, string commandChar, string? floatingCommandChar = null, TimeSpan? updateInterval = null) : PluginBase(commandChar,floatingCommandChar,updateInterval)
{
    /// <summary>our api connection</summary>
    public ApiConnection FChatApi { get; } = api;

    /// <summary>the channels in which this module is active</summary>
    public Dictionary<string, Channel> ActiveChannels { get; } = activeChannels.ToDictionary(li => li.Code, li => li);

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    /// <summary>
    /// how we should handle a recieved message
    /// </summary>
    /// <param name="command">command sent</param>
    /// <param name="channel">source channel, if any</param>
    /// <param name="message">base message, if any</param>
    /// <param name="sendingUser">sending user</param>
    /// <param name="isOp">if the user is an op</param>
    public async virtual Task HandleRecievedMessage(BotCommand command, Channel? channel, string? message, User sendingUser, bool isOp) { }
      
    public async virtual Task HandleJoinedChannel(Channel channel) { }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}