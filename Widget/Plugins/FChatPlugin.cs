using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Enums;
using Plugins.Tokenizer;

using ModularPlugins.Interfaces;
using FChatApi.Attributes;
using Plugins.Attributes;
using Plugins.Enums;

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
public class FChatPlugin<TCommand>(ApiConnection api, TimeSpan updateInterval) : PluginBase(updateInterval), IFChatPlugin where TCommand : struct, Enum
{
	/// <summary>our api connection</summary>
	public ApiConnection FChatApi { get; }									= api;

	/// <summary>the channels in which this module is active</summary>
	public Dictionary<string, Channel> ActiveChannels { get; }				= [];

	/// <summary>the operators </summary>
	public Dictionary<string, Privilege> Operators { get; }					= [];

	/// <summary>the operators </summary>
	public static Dictionary<string, Privilege> GlobalOperators { get; }	= [];

	/// <summary>
	/// how we should handle a recieved message
	/// </summary>
	/// <param name="command">command sent</param>
	public virtual void HandleRecievedMessage(CommandTokens command) { }
	  
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
	public FChatPlugin<TCommand> SetOperators(IEnumerable<string> values,Privilege privilege)
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

	public override void Initialize()=> base.Initialize();

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

    public override void Shutdown() => base.Shutdown();

    void IFChatPlugin.HandleRecievedMessage(CommandTokens commandTokens)
    {
		if (Enum.TryParse(commandTokens.Command,true,out TCommand command))
		{
			if (
				command.GetEnumAttribute<TCommand,UsageScopeAttribute>().Scope == CommandScope.Anywhere ||
				(command.GetEnumAttribute<TCommand,UsageScopeAttribute>().Scope == CommandScope.Whisper &&
				commandTokens.Source.MessageType == FChatMessageType.Whisper) ||
				(command.GetEnumAttribute<TCommand,UsageScopeAttribute>().Scope == CommandScope.AnyChannel &&
				commandTokens.Source.MessageType == FChatMessageType.Basic) ||
				(command.GetEnumAttribute<TCommand,UsageScopeAttribute>().Scope == CommandScope.ActiveChannel &&
				ActiveChannels.ContainsValue(commandTokens.Source.Channel))
			)
        		HandleRecievedMessage(commandTokens);
		}
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