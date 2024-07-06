using System;
using System.Text;
using WatsonWebsocket;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FChatApi.Systems;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.EventArguments;

namespace FChatApi.Core
{
    public partial class ApiConnection
    {
        public ApiConnection()
        { }

        #region Client Events
        async void Client_MessageReceived(object sender, MessageReceivedEventArgs @event)
        {
            string message = Encoding.UTF8.GetString(@event.Data.ToArray());
            //Console.WriteLine($"Message from server: {message}");
            await ParseMessage(Enum.Parse<Hycybh>(message.Split(' ').First()), message.Split(" ".ToCharArray(), 2).Last());
        }

        void Client_ChatDisConnected(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Disconnected from F-Chat servers!");
        }

        async void Client_ChatConnected(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Connected to F-Chat servers! Sending identification...");
            await IdentifySelf(UserName, TicketInformation.Ticket, CharacterName, ClientId, ClientVersion);
            StartReplyThread();
        }
        #endregion

        async static Task IdentifySelf(string accountName, string ticket, string botName, string botClientID, string botClientVersion)
        {
            string toSend = $"{Hycybh.IDN}  {{ \"method\": \"ticket\", \"account\": \"{accountName}\", \"ticket\": \"{ticket}\", \"character\": \"{botName}\", \"cname\": \"{botClientID}\", \"cversion\": \"{botClientVersion}\" }}";
            await Client.SendAsync(toSend);
        }

        static JObject ParseToJObject(string message, Hycybh hycybh)
        {
            JObject returnCarrier;

            try
            {
                if (message.Split(' ').Length <= 1)
                {
                    if (string.Equals(hycybh.ToString(), message))
                    {
                        return null;
                    }

                    returnCarrier = JObject.Parse(message);
                }
                else
                {
                    returnCarrier = JObject.Parse(message.Replace(hycybh.ToString(), "").TrimStart());
                }
            }
            catch
            {
                throw new Exception($"Failure to parse message: {message}");
            }

            return returnCarrier;
        }

        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////
        ///////////////////////////////////////////////////

        #region ParseMessage
        async Task ParseMessage(Hycybh hycybh, string message)
        {
            JObject json;
            try
            {
                json = ParseToJObject(message, hycybh);
            }
            catch(Exception)
            {
                json = null;
            }

            switch (hycybh)
            {
                case Hycybh.STA:
                    {
                        if (json["character"].ToString().Equals(CharacterName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            Console.WriteLine("Status Changed");
                        }
                    }
                    break;
                case Hycybh.IDN:
                    {
                        ConnectedToChat?.Invoke(this, null);

                        await RequestInternalChannelList(ChannelType.Private);
                        await RequestInternalChannelList(ChannelType.Public);

                        Console.WriteLine("Connected to Chat");
                    }
                    break;
                case Hycybh.ORS:
                    {
                        List<Channel> privateChannelList = new List<Channel>();
                        
                        foreach (var channel in json["channels"])
                        {
                            privateChannelList.Add(new Channel(channel["title"].ToString(), channel["name"].ToString(), ChannelType.Private));
                        }

                        ChannelTracker.RefreshAvailableChannels(privateChannelList, ChannelType.Private);
                        PrivateChannelsReceivedHandler?.Invoke(this, new ChannelEventArgs() { });
                        Console.WriteLine($"Private Channels Recieved... {privateChannelList.Count} total Private Channels.");
                    }
                    break;
                case Hycybh.CHA:
                    {
                        List<Channel> publicChannelList = new List<Channel>();

                        foreach (var channel in json["channels"])
                        {
                            publicChannelList.Add(new Channel(string.Empty, channel["name"].ToString(), ChannelType.Private));
                        }

                        ChannelTracker.RefreshAvailableChannels(publicChannelList, ChannelType.Public);
                        PublicChannelsReceivedHandler?.Invoke(this, new ChannelEventArgs() { });
                        Console.WriteLine($"Public Channels Recieved... {publicChannelList.Count} total Public Channels.");
                    }
                    break;
                case Hycybh.MSG:
                    {
                        MessageHandler?.Invoke(Hycybh.MSG, new MessageEventArgs() { channel = json["channel"].ToString(), message = json["message"].ToString(), user = json["character"].ToString() });
                        Console.WriteLine(message);
                    }
                    break;
                case Hycybh.PRI:
                    {
                        MessageHandler?.Invoke(Hycybh.PRI, new MessageEventArgs() { user = json["character"].ToString(), message = json["message"].ToString() });
                        Console.WriteLine(message);
                    }
                    break;
                case Hycybh.JCH:
                    {
                        User user = UserTracker.GetUserByName(json["character"]["identity"].ToString());
                        Channel tempChannel;
                        try
                        {
                            tempChannel = ChannelTracker.GetChannelByNameOrCode(json["title"].ToString());
                        }
                        catch
                        {
                            tempChannel = ChannelTracker.AddManualChannel(json["title"].ToString(), ChannelStatus.Available, json["channel"].ToString());
                        }

                        if (user.Name.Equals(CharacterName))
                        {
                            ChannelTracker.WatchChannels.Add(tempChannel.Code,tempChannel);
                        }

                        if (tempChannel == null)
                        {
                            return;
                        }

                        // creating channel
                        bool creating = tempChannel.Status == ChannelStatus.Creating && user.Name.Equals(CharacterName);
                        if (creating)
                        {
                            tempChannel = ChannelTracker.FinalizeChannelCreation(json["title"].ToString(), json["channel"].ToString(), user);
                            Console.WriteLine($"Created Channel: {json["channel"]}");
                            CreatedChannelHandler?.Invoke(this, new ChannelEventArgs() { name = tempChannel.Name, status = ChannelStatus.Joined, code = tempChannel.Code, type = tempChannel.Type });
                        }

                        // join channel
                        Channel channel = ChannelTracker.GetChannelByNameOrCode(json["channel"].ToString());
                        if (user != null && channel != null)
                        {
                            if (!creating)
                            {
                                JoinedChannelHandler?.Invoke(this, new ChannelEventArgs() { name = json["title"].ToString(), status = ChannelStatus.Joined, code = tempChannel.Code, type = tempChannel.Type, userJoining = user.Name });
                            }

                            channel.AddUser(user);
                            Console.WriteLine($"{user.Name} joined Channel: {channel.Name}. {channel.Users.Count} total users in channel.");
                        }
                    }
                    break;
                case Hycybh.LCH:
                    {
                        if (json["character"].ToString().Equals(CharacterName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            LeftChannelHandler?.Invoke(this, new ChannelEventArgs() { name = json["channel"].ToString(), status = ChannelStatus.Left });
                            ChannelTracker.ChangeChannelState(json["channel"].ToString(), ChannelStatus.Available);
                        }

                        User user = UserTracker.GetUserByName(json["character"].ToString());
                        Channel channel = ChannelTracker.GetChannelByNameOrCode(json["channel"].ToString());

                        if (user.Name.Equals(CharacterName))
                        {
                            ChannelTracker.WatchChannels.Remove(channel.Code);
                        }

                        if (user != null && channel != null)
                        {
                            channel.RemoveUser(user);
                            Console.WriteLine($"{user.Name} left Channel: {json["channel"]}. {channel.Users.Count} total users in channel.");
                        }

                    }
                    break;
                case Hycybh.PIN:
                    {
                        await Client.SendAsync(Hycybh.PIN.ToString());
                    }
                    break;
                case Hycybh.VAR:
                    {

                    }
                    break;
                case Hycybh.HLO:
                    {

                    }
                    break;
                case Hycybh.CON:
                    {
                        Console.WriteLine($"{json["count"]} connected users sent.");
                    }
                    break;
                case Hycybh.FRL:
                    {
                        // friends list
                    }
                    break;
                case Hycybh.IGN:
                    {
                        // ignore list
                    }
                    break;
                case Hycybh.ADL:
                    {

                    }
                    break;
                case Hycybh.LIS:
                    {
                        foreach(var userinfo in json["characters"])
                        {
                            User tempUser = new User()
                            {
                                Name = userinfo[0].ToString(),
                                Gender = userinfo[1].ToString(),
                                ChatStatus = (ChatStatus)Enum.Parse(typeof(ChatStatus), userinfo[2].ToString().ToLowerInvariant(), true)
                            };
                            UserTracker.SetChatStatus(tempUser, tempUser.ChatStatus, false);
                        }

                        Console.WriteLine($"Added {json["characters"].Count()} users. Total users: {UserTracker.GetNumberActiveUsers()}");
                    }
                    break;
                case Hycybh.NLN:
                    {
                        User tempUser = new User()
                        {
                            Name = json["identity"].ToString(),
                            UserStatus = (UserStatus)Enum.Parse(typeof(UserStatus), json["status"].ToString().ToLowerInvariant(), true),
                            Gender = json["gender"].ToString()
                        };
                        UserTracker.SetChatStatus(tempUser, ChatStatus.Online, false);
                    }
                    break;
                case Hycybh.COL:
                    {
                        Channel tempChannel = ChannelTracker.GetChannelByNameOrCode(json["channel"].ToString());

                        int counter = 0;
                        foreach (string username in json["oplist"].Select(user => user.ToString()))
                        {
                            if (string.IsNullOrWhiteSpace(username.ToString()))
                            {
                                continue;
                            }

                            User tempUser = UserTracker.GetUserByName(username);

                            if (counter == 0)
                            {
                                tempChannel.Owner = tempUser;
                            }

                            tempChannel.AddMod(tempUser);
                        }

                        Console.WriteLine($"Found {tempChannel.Mods.Count} mods for channel: {tempChannel.Name}");
                    }
                    break;
                case Hycybh.FLN:
                    {
                        User tempUser = new User()
                        {
                            Name = json["character"].ToString()
                        };

                        UserTracker.SetChatStatus(tempUser, ChatStatus.Offline, false);
                        foreach (var channel in ChannelTracker.WatchChannels.Values)
                        {
                            bool needsRemoved = false;

                            if (channel.Users.ContainsKey(tempUser.Name))
                            {
                                needsRemoved = true;
                            }

                            if (needsRemoved)
                            {
                                channel.RemoveUser(tempUser);
                            }
                        }
                    }
                    break;
                case Hycybh.ICH:
                    {
                        // joining channel
                        Channel tempChannel = ChannelTracker.ChangeChannelState(json["channel"].ToString(), ChannelStatus.Joined);
                        foreach (var user in json["users"])
                        {
                            User tempUser = UserTracker.GetUserByName(user["identity"].ToString());
                            if (null == tempUser)
                            {
                                Console.WriteLine($"Error attempting to add user {user["identity"]} to {json["channel"]} channel's userlist.");
                            }

                            tempChannel.AddUser(tempUser);
                            tempChannel.AdEnabled = !json["mode"].ToString().Equals("chat");
                        }

                        Console.WriteLine($"Adding {json["users"].Count()} users to {tempChannel.Name} channel's userlist successful.");
                    }
                    break;
                case Hycybh.CDS:
                    {
                        Channel tempChannel = ChannelTracker.GetChannelByNameOrCode(json["channel"].ToString());
                        tempChannel.Description = json["description"].ToString();
                    }
                    break;
                default:
                    {

                    }
                    break;
            }
        }
    }
    #endregion
}