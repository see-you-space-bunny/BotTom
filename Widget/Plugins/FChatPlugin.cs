using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Tokenizer;
using FChatApi.EventArguments;

using ModularPlugins.Interfaces;

namespace ModularPlugins;

/// <summary>
/// our fchat plugin for others to derive off of
/// </summary>
/// <remarks>
/// sets our api connection in the constructor
/// </remarks>
/// <param name="api"></param>
/// <param name="commandChar">the symbol that wakes the module up</param>
/// <param name="updateInterval">how often this module runs Update().<br/>defaults to: Never</param>
public class FChatPlugin(ApiConnection api, TimeSpan updateInterval) : PluginBase(updateInterval), IFChatPlugin
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
	public virtual void HandleJoinedChannel(ChannelEventArgs @event) { }
	  
	/// <summary>
	/// how we should handle a successful channel creation
	/// </summary>
	/// <param name="channel">channel created</param>
	public virtual void HandleCreatedChannel(ChannelEventArgs @event) { }

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

	/// <summary>
	/// do periodically executed update stuff here
	/// </summary>
	/// <remarks>
	/// you <b><u>must</u></b> call this base method in the override
	/// </remarks>
	public override void Update()
	{
		base.Update();
	}

    public override void Shutdown() { }

    void IFChatPlugin.HandleRecievedMessage(BotCommand command)
    {
        HandleRecievedMessage(command);
    }

    void IFChatPlugin.HandleJoinedChannel(ChannelEventArgs @event)
    {
        HandleJoinedChannel(@event);
    }

    void IFChatPlugin.HandleCreatedChannel(ChannelEventArgs @event)
    {
        HandleCreatedChannel(@event);
    }
}