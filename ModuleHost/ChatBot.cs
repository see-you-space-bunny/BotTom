using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.Objects;
using ModuleHost.CommandHandling;
using Discord;
using Discord.WebSocket;
using ChatApi.Core;
using ModuleHost.CardiApi;
using FileManip;
using ModuleHost.Attributes;
using System.Reactive.Concurrency;

namespace ModuleHost
{
    /// <summary>
    /// This is our main bot interface
    /// </summary>
    public class ChatBot
    {
        public static Dictionary<string,RegisteredUser> RegisteredUsers { get; } = [];

        /// <summary>
        /// our list of active plugins
        /// </summary>
        private Dictionary<(Platform Platform,Type Type),PluginBase> Plugins { get; }

        /// <summary>
        /// our list of active fchat plugins
        /// </summary>
        private IEnumerable<FChatPlugin> FChatPlugins => Plugins.Values.OfType<FChatPlugin>();

        /// <summary>
        /// our list of active discord plugins
        /// </summary>
        private IEnumerable<DiscordSlashPlugin> DiscordSlashPlugins => Plugins.Values.OfType<DiscordSlashPlugin>();

        private TimeSpan _userRefreshIneterval = new TimeSpan(0,0,15);
        private DateTime _nextUserRefresh;

        private AsyncLock _registeredUsersLock = new();

        /// <summary>
        /// constructor, inits plugins
        /// </summary>
        public ChatBot()
        {
            Plugins = [];
            _nextUserRefresh = DateTime.Now;

            DeserializeRegisteredUsers();
        }

        private static void DeserializeRegisteredUsers(string? path = null)
        {
            path ??= Path.Combine(Environment.CurrentDirectory,"sessiondata","RegisterdPlayers");
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
                    var tempRegUser = RegisteredUser.Deserialize(reader);
                    if (RegisteredUsers.TryAdd(tempRegUser.Name.ToLower(),tempRegUser))
                        continue;
                }
            }
        }

        /// <summary>
        /// Adds a plugin to the list of active plugins
        /// </summary>
        /// <param name="plugin"></param>
        public ChatBot AddPlugin<TPlugin>(PluginBase plugin) where TPlugin : PluginBase
        {
            var key = (typeof(TPlugin) switch
                {
                    Type t when t.IsSubclassOf(typeof(FChatPlugin))         => Platform.FChat,
                    Type t when t.IsSubclassOf(typeof(DiscordSlashPlugin))  => Platform.Discord,
                    _ => throw new ArgumentException(typeof(TPlugin).ToString()),
                },
                plugin.GetType()
            );
            if (Plugins.ContainsKey(key))
            {
                Console.WriteLine("You cannot add same plugin twice!");
                return this;
            }

            Plugins.Add(key,plugin);
            return this;
        }

        /// <summary>
        /// Returns plugin of a specific type if available
        /// </summary>
        /// <param name="type">type to return</param>
        /// <returns>plugin if found, otherwise null</returns>
        public PluginBase? GetPlugin(Platform platform, Type type)
        {
            if (Plugins.TryGetValue((platform,type),out PluginBase? pluginBase))
                return pluginBase;
            return null;
        }

        /// <summary>
        /// update the bot's and its modules
        /// </summary>
        public async Task Update()
        {
            if (_nextUserRefresh > DateTime.Now)
                foreach (var regUser in RegisteredUsers.Keys)
                    if (RegisteredUsers.TryGetValue(regUser, out RegisteredUser? registeredUser) && ApiConnection.TryGetUserByName(regUser,out User user))
                        registeredUser.Update(user);

            var tasks = new Task[Plugins.Count];
            int i = 0;
            foreach (var plugin in Plugins.Values.Where(p => p.NextUpdate >= DateTime.Now))
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
            lock (_registeredUsersLock)
            {
                using var stream = File.Create(Path.Combine(Environment.CurrentDirectory, "sessiondata", "RegisterdPlayers"));
                var writer = new BinaryWriter(stream);
                writer.Write((uint)RegisteredUsers.Count);
                foreach (RegisteredUser regUser in RegisteredUsers.Values)
                {
                    regUser.Serialize(writer);
                }
            }

            var tasks = new Task[Plugins.Count];
            int i = 0;
            foreach (var plugin in Plugins.Values)
            {
                tasks[i] = Task.Run(() => plugin.Shutdown());
                ++i;
            }
            await Task.WhenAll(tasks.Where(t => t != null).ToArray());
        }

        /// <summary>
        /// Handles when we receive a message from the chat server
        /// </summary>
        /// <param name="command">command being sent, if any</param>
        public void HandleMessage(BotCommand command)
        {
            FChatPlugins.Single(p => p.ModuleType == command.BotModule).HandleRecievedMessage(command);
        }


        /// <summary>
        /// Handles when we receive a slash command
        /// </summary>
        /// <param name="command">the slash command context</param>
        public async void HandleMessage(SocketSlashCommand command)
        {
            if (Enum.TryParse(command.Data.Name,true,out BotModule botModule))
            {
                await DiscordSlashPlugins.Single(p => p.ModuleType == botModule).HandleRecievedMessage(command);
            }
        }

        /// <summary>
        /// Called when a channel is joined
        /// </summary>
        /// <param name="channel">Channel that was joined</param>
        public async void HandleJoinedChannel(string channelCode)
        {
            Channel channel = ApiConnection.GetChannelByNameOrCode(channelCode);
            
            var tasks = new Task[FChatPlugins.Count()];
            int i = 0;
            foreach (var plugin in FChatPlugins)
            {
                tasks[i] = Task.Run(() => plugin.HandleJoinedChannel(channel));
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
}