using System.Reactive.Concurrency;
using FChatApi.Objects;
using Engine.ModuleHost.Enums;
using ModularPlugins.Interfaces;
using System.ComponentModel;
using FChatApi.Attributes;
using FChatApi.Core;

namespace Engine.ModuleHost;

/// <summary>
/// This is our main bot interface
/// </summary>
public partial class ChatBot
{
	public static Dictionary<string,User> RegisteredUsers { get => ApiConnection.RegisteredUsers; }

	/// <summary>
	/// our list of active plugins
	/// </summary>
	private Dictionary<BotModule,IFChatPlugin> FChatPlugins { get; }

	private AsyncLock _UsersLock = new();

	/// <summary>
	/// constructor, inits plugins
	/// </summary>
	public ChatBot()
	{
		FChatPlugins = [];
		AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(BotModule));

		DeserializeUsers();
		ApiConnection.ImportRegisteredUsers();
	}

	private static void DeserializeUsers(string? path = null)
	{
		path ??= Path.Combine(Environment.CurrentDirectory,"sessiondata","KnownUsers");
		if (!File.Exists(path))
			return;

		using (var stream = File.OpenRead(path))
		{
			var reader  = new BinaryReader(stream);
			uint count  = reader.ReadUInt32();
			if (count == 0)
				return;

			for (uint i=0;i<count;i++)
			{
				var user = User.Deserialize(reader);
				if (RegisteredUsers.TryAdd(user.Name.ToLower(),user))
					continue;
			}
		}
	}

	/// <summary>
	/// Adds a plugin to the list of active plugins
	/// </summary>
	/// <param name="plugin"></param>
	public ChatBot AddPlugin(BotModule type,IFChatPlugin plugin)
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
	public IFChatPlugin GetPlugin<TPlugin>(BotModule value)
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

		var tasks = new Task[FChatPlugins.Count];
		int i = 0;
		foreach (IFChatPlugin plugin in FChatPlugins.Values.Where(p => p.NextUpdate >= DateTime.Now))
		{
			tasks[i] = Task.Run(() => plugin.Update());
			++i;
		}
		await Task.WhenAll(tasks.Where(t => t != null).ToArray());
		if (_shutdown)
			await Shutdown();
	}

	/// <summary>
	/// Clean up our plugins if we're closing things down
	/// </summary>
	public async Task Shutdown()
	{
		lock (_UsersLock)
		{
			// TODO get list filtered by registered users
			// TODO get method for adding registered users back into this list
			var userlist = ApiConnection.GetUserListByStatus(FChatApi.Enums.UserStatus.Online);
			if (userlist.Any())
			using (var stream = File.Create(Path.Combine(Environment.CurrentDirectory, "sessiondata", "KnownUsers")))
			{
				var writer = new BinaryWriter(stream);
				writer.Write((uint)RegisteredUsers.Count);
				foreach (User user in RegisteredUsers.Values)
				{
					user.Serialize(writer);
				}
			}
		}

		var tasks = new Task[FChatPlugins.Count];
		int i = 0;
		foreach (IFChatPlugin plugin in FChatPlugins.Values)
		{
			tasks[i] = Task.Run(() => plugin.Shutdown());
			++i;
		}
		await Task.WhenAll(tasks.Where(t => t != null).ToArray());
	}

	private static bool _shutdown = false;
	public bool ShutdownFlag => _shutdown;
	public static void SetShutdownFlag()
	{
		_shutdown = true;
	}
}