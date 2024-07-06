using System.Linq;
using Engine.ModuleHost.CommandHandling;
using Engine.ModuleHost;
using FChatApi.Core;
using FChatApi.Systems;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.EventArguments;

namespace BotTom
{
    public partial class Program
    {
        /// <summary>
        /// Takes care of pushing messages to the bot
        /// </summary>
        /// <param name="sender">our sending object</param>
        /// <param name="e">our event args</param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async void HandleMessageReceived(object sender, MessageEventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (F_CommandParser.TryConvertCommand(
                e.user,
                ChatBot.RegisteredUsers.TryGetValue(e.user.ToLower(),out var regUser) ? regUser : null,
                e.channel   != null ? ApiConnection.GetChannelByNameOrCode(e.channel)           : null,
                e.message,
                out BotCommand ?command
            ))
            {
                F_Bot!.HandleMessage(command!);
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async void ConnectedToChat(object sender, ChannelEventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {

        }

        /// <summary>
        /// We've joined a channel
        /// </summary>
        /// <param name="sender">our sending object</param>
        /// <param name="e">our event args</param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async void HandleJoinedChannel(object sender, ChannelEventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (e.userJoining.Equals(F_CharacterName))
            {
                //F_Chat.SetStatus(ChatStatus.DND, $"[session={e.name}]{(string.IsNullOrWhiteSpace(e.code) ? e.name : e.code)}[/session] [color=pink]DM me with {F_CommandChar}{"help"} to get started![/color]", F_CharacterName);
                F_Bot!.HandleJoinedChannel(string.IsNullOrWhiteSpace(e.code) ? e.name : e.code);
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async void HandleCreatedChannel(object sender, ChannelEventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            //F_Chat.Mod_InviteUserToChannel("Astral Mage", e.code);
            //F_Chat.SetStatus(ChatStatus.DND, $"[session={e.name}]{(string.IsNullOrWhiteSpace(e.code) ? e.name : e.code)}[/session] [color=green]Welcome to the testing grounds.[/color]", F_CharacterName);

        }

        /// <summary>
        /// We've left a channel
        /// </summary>
        /// <param name="sender">our sending object</param>
        /// <param name="e">our event args</param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        static async void HandleLeftChannel(object sender, ChannelEventArgs e)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {

        }

        /// <summary>
        /// We got a private list of channels
        /// </summary>
        /// <param name="sender">our sending object</param>
        /// <param name="e">our event args</param>
        static async void HandlePrivateChannelsReceived(object sender, ChannelEventArgs e)
        {
            var privateChannels = ApiConnection.RequestChannelList(ChannelType.Private);

            // check and join starting channel here
            if (privateChannels.Any(x => x.Code.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)))
            {
                await ApiConnection.JoinChannel(F_StartingChannel);
            }
            else if (privateChannels.Any(x => x.Name.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)))
            {
                await ApiConnection.JoinChannel(privateChannels.First(x => x.Name.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)).Code);
            }

#if DEBUG
            //string roomname = "Aelia's Secret Testing Ground";
            //if (!F_Chat.RequestChannelList(ChannelType.Private).Any(x => x.Code.Equals("adh-1a7c52c105ef5420b73b", System.StringComparison.InvariantCultureIgnoreCase)))
            //{
            //    F_Chat.CreateChannel(roomname);
            //}
            //else
            //{
            //    F_Chat.JoinChannel(roomname);
            //}
#endif
        }

        /// <summary>
        /// We got a public list of channels
        /// </summary>
        /// <param name="sender">our sending object</param>
        /// <param name="e">our event args</param>
        static void HandlePublicChannelsReceived(object sender, ChannelEventArgs e)
        {
        }
    }
}