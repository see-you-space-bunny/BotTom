using System.Reactive.Concurrency;
using FChatApi.Objects;
using ModularPlugins.Interfaces;
using System.ComponentModel;
using FChatApi.Attributes;
using FChatApi.Core;
using Plugins.Tokenizer;

namespace Engine.ModuleHost;

/// <summary>
/// This is our main bot interface
/// </summary>
public partial class ChatBot
{
	/// <summary>
	/// our list of active plugins
	/// </summary>
	private Dictionary<Type,IFChatPlugin> FChatPlugins { get; }

	/// <summary>
	/// constructor, inits plugins
	/// </summary>
	public ChatBot()
	{
		FChatPlugins = [];
	}

	/// <summary>
	/// Adds a plugin to the list of active plugins
	/// </summary>
	/// <param name="plugin"></param>
	public ChatBot AddPlugin(Type type,IFChatPlugin plugin)
	{
		if (FChatPlugins.ContainsKey(type))
		{
			Console.WriteLine("You cannot add same plugin twice!");
			return this;
		}
		FChatPlugins.Add(type,plugin);
		return this;
	}

	/// <summary>
	/// Returns plugin of a specific type if available
	/// </summary>
	/// <param name="type">type to return</param>
	/// <returns>plugin if found, otherwise null</returns>
	public IFChatPlugin GetPlugin(Type value)
	{
		return FChatPlugins.FirstOrDefault(p => p.Key == value).Value;
	}

	/// <summary>
	/// update the bot's and its modules
	/// </summary>
	public async Task Update()
	{
		//if (_nextUserRefresh > DateTime.Now)
		//	foreach (var regUser in Users.Keys)
		//		if (Users.TryGetValue(regUser, out User? User) && ApiConnection.TryGetOnlineUserByName(regUser,out User user))
		//			User.Update(user);

		List<Task> tasks = [];
		foreach (IFChatPlugin plugin in FChatPlugins.Values.Where(p => DateTime.Now >= p.NextUpdate))
		{
			tasks.Add(Task.Run(() => plugin.Update()));
		}
		await Task.WhenAll([.. tasks]);
	}

	/// <summary>
	/// Clean up our plugins if we're closing things down
	/// </summary>
	public async Task Shutdown()
	{
		List<Task> tasks = [];
		foreach (IFChatPlugin plugin in FChatPlugins.Values)
		{
			tasks.Add(Task.Run(() => plugin.Shutdown()));
		}
		await Task.WhenAll([.. tasks]);
	}
}