using ChatApi.Core;
using ChatApi.Objects;
using ModuleHost.CardiApi;
using ModuleHost.CommandHandling;
using ModuleHost.Enums;

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
public class FChatPlugin(ApiConnection api,TimeSpan? updateInterval = null) : PluginBase(updateInterval)
{
    /// <summary>our api connection</summary>
    public ApiConnection FChatApi { get; } = api;

    /// <summary>the channels in which this module is active</summary>
    public Dictionary<string, Channel> ActiveChannels { get; } = [];

    /// <summary>the operators </summary>
    public Dictionary<string, Privilege> Operators { get; } = [];

    /// <summary>the operators </summary>
    public static Dictionary<string, Privilege> GlobalOperators { get; } = [];

    /// <summary>
    /// how we should handle a recieved message
    /// </summary>
    /// <param name="command">command sent</param>
    public virtual void HandleRecievedMessage(BotCommand command) { }
      
    /// <summary>
    /// how we should handle a successful channel joining
    /// </summary>
    /// <param name="channel">channel joined</param>
    public virtual void HandleJoinedChannel(Channel channel) { }


    /// <summary>
    /// Add Operators to the module
    /// </summary>
    public FChatPlugin SetOperators(IEnumerable<string> values,Privilege privilege)
    {
        if (privilege >= Privilege.GlobalOperator)
        {
            SetGlobalOperators(values,privilege);
        }
        else
        {
            foreach(var user in values)
            {
                Operators.TryAdd(user,privilege);
            }
        }
        return this;
    }

    /// <summary>
    /// Add global operators across all modules
    /// </summary>
    public static void SetGlobalOperators(IEnumerable<string> values,Privilege privilege)
    {
        foreach(var user in values)
        {
            GlobalOperators.TryAdd(user,privilege);
        }
    }
}