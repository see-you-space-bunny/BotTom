using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatApi.Objects;
using ModuleHost.CommandHandling;
using Discord;
using Discord.WebSocket;
using ChatApi.Core;

namespace ModuleHost
{
    /// <summary>
    /// This is our main bot interface
    /// </summary>
    public class ChatBot
    {
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

        /// <summary>
        /// constructor, inits plugins
        /// </summary>
        public ChatBot()
        {
            Plugins = [];
        }

        /// <summary>
        /// Adds a plugin to the list of active plugins
        /// </summary>
        /// <param name="plugin"></param>
        public void AddPlugin<TPlugin>(PluginBase plugin) where TPlugin : PluginBase
        {
            var key = (typeof(TPlugin) switch
                {
                    Type t when t == typeof(FChatPlugin)        => Platform.FChat,
                    Type t when t == typeof(DiscordSlashPlugin) => Platform.Discord,
                    _ => throw new ArgumentException(typeof(TPlugin).ToString()),
                },
                plugin.GetType()
            );
            if (Plugins.ContainsKey(key))
            {
                Console.WriteLine("Unable to add same plugin twice at this time. Sorry!");
                return;
            }

            Plugins.Add(key,plugin);
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
        /// Clean up our plugins if we're closing things down
        /// </summary>
        public async Task Update()
        {
            foreach (var plugin in Plugins.Values.Where(p => p.NextUpdate >= DateTime.Now))
            {
                await plugin.Update();
            }
        }

        /// <summary>
        /// Clean up our plugins if we're closing things down
        /// </summary>
        public async Task Shutdown()
        {
            foreach (var plugin in Plugins.Values)
            {
                await plugin.Shutdown();
            }
        }

        /// <summary>
        /// Handles when we receive a message from the chat server
        /// </summary>
        /// <param name="channel">originating channel</param>
        /// <param name="message">cleaned up message</param>
        /// <param name="sendingUser">user that send the message</param>
        /// <param name="command">command being sent, if any</param>
        /// <param name="isOp">if the sending user is an op</param>
        public async void HandleMessage(string channelCode, string message, string sendingUser, BotCommand command, bool isOp)
        {
            await FChatPlugins.Single(p => p.ModuleType == command.BotModule)
                .HandleRecievedMessage(
                    command,
                    ApiConnection.GetChannelByNameOrCode(channelCode),
                    message,
                    ApiConnection.GetUserByName(sendingUser),
                    isOp
                );
        }

        /// <summary>
        /// Handles when we receive a slash command
        /// </summary>
        /// <param name="command">the slash command context</param>
        public async void HandleMessage(SocketSlashCommand command)
        {
            foreach (DiscordSlashPlugin plugin in DiscordSlashPlugins)
            {
                await plugin.HandleRecievedMessage(command);
            }
        }

        /// <summary>
        /// Called when a channel is joined
        /// </summary>
        /// <param name="channel">Channel that was joined</param>
        public async void HandleJoinedChannel(string channelCode)
        {
            Channel channel = ApiConnection.GetChannelByNameOrCode(channelCode);
            foreach (FChatPlugin plugin in FChatPlugins)
            {
                await plugin.HandleJoinedChannel(channel);
            }
        }
    }
}