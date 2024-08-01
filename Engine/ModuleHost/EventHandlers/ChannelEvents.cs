using System.Reactive.Concurrency;
using Discord.WebSocket;
using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Enums;
using Plugins.Tokenizer;

namespace Engine.ModuleHost;

/// <summary>
/// This is our main bot interface
/// </summary>
public partial class ChatBot
{
	/// <summary>
	/// Called when a channel is joined
	/// </summary>
	/// <param name="@event">Channel that was joined</param>
	public async void HandleJoinedChannel(ChannelEventArgs @event)
	{
		List<Task> tasks = [];
		foreach (var plugin in FChatPlugins.Values)
		{
			tasks.Add(Task.Run(() => plugin.HandleJoinedChannel(@event)));
		}
		await Task.WhenAll([.. tasks]);
	}

	/// <summary>
	/// Called when a channel is joined
	/// </summary>
	/// <param name="@event">Channel that was created</param>
	public async void HandleCreatedChannel(ChannelEventArgs @event)
	{
		List<Task> tasks = [];
		foreach (var plugin in FChatPlugins.Values)
		{
			tasks.Add(Task.Run(() => plugin.HandleCreatedChannel(@event)));
		}
		await Task.WhenAll([.. tasks]);
	}
}