using System.Reactive.Concurrency;
using Discord.WebSocket;
using FChatApi.Core;
using FChatApi.Objects;
using Engine.ModuleHost.Enums;
using FChatApi.Enums;
using FChatApi.Tokenizer;
using FChatApi.EventArguments;

namespace Engine.ModuleHost;

/// <summary>
/// This is our main bot interface
/// </summary>
public partial class ChatBot
{
	/// <summary>
	/// Called when a channel is joined
	/// </summary>
	/// <param name="channel">Channel that was joined</param>
	public async void HandleJoinedChannel(ChannelEventArgs @event)
	{
		Channel channel = ApiConnection.GetChannelByCode(@event.Channel.Code);
		
		List<Task> tasks = [];
		foreach (var plugin in FChatPlugins.Values)
		{
			tasks.Add(Task.Run(() => plugin.HandleJoinedChannel(@event)));
		}
		await Task.WhenAll([.. tasks]);
	}
}