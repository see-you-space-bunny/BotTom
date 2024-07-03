using ChatApi.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi
{
    public partial class ApiConnection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelname"></param>
        public async Task JoinChannel(string channelname)
        {
            if (!IsConnected()) throw new Exception("You must be connected to chat to do this.");

            string toSend = Hycybh.JCH.ToString() + string.Format(" {{\"channel\": \"{0}\"}}", channelname);
            Console.WriteLine($"Attempting to join: {channelname}");
            await Client.SendAsync(toSend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelType"></param>
        private async Task RequestInternalChannelList(ChannelType channelType)
        {
            if (!IsConnected()) throw new Exception("You must be connected to chat to do this.");

            string toSend = channelType == ChannelType.Private ? Hycybh.ORS.ToString() : Hycybh.CHA.ToString();
            Console.WriteLine($"Attempting to retrieve all {channelType} channels from server.");
            await Client.SendAsync(toSend);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userStatus"></param>
        /// <returns></returns>
        public IEnumerable<User> RequestUserList(UserStatus userStatus)
        {
            return userTracker.GetUsersByStatus(userStatus).Select(u=>u.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public User GetUserByName(string name)
        {
            return userTracker.GetUserByName(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelType"></param>
        /// <returns></returns>
        public IEnumerable<Channel> RequestChannelList(ChannelType channelType)
        {
            return channelTracker.GetChannelList(channelType).Select(ch=>ch.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameorcode"></param>
        /// <returns></returns>
        public Channel GetChannelByNameOrCode(string nameorcode)
        {
            return channelTracker.GetChannelByNameOrCode(nameorcode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelname"></param>
        public async void LeaveChannel(string channelname)
        {
            if (!IsConnected()) throw new Exception("You must be connected to chat to do this.");

            string toSend = Hycybh.LCH.ToString() + string.Format(" {{\"channel\": \"{0}\"}}", channelname);
            Console.WriteLine($"Attempting to leave channel: {channelname}");
            await Client.SendAsync(toSend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channelName"></param>
        public async void CreateChannel(string channelName)
        {
            if (!IsConnected()) throw new Exception("You must be connected to chat to do this.");

            string toSend = Hycybh.CCR.ToString() + string.Format(" {{\"channel\": \"{0}\"}}", channelName);
            Console.WriteLine($"Attempting to create channel: {channelName}");
            channelTracker.StartChannelCreation(channelName);
            await Client.SendAsync(toSend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="channelname"></param>
        public async void Mod_InviteUserToChannel(string username, string channelname)
        {
            if (!IsConnected()) throw new Exception("You must be connected to chat to do this.");

            string toSend = Hycybh.CIU.ToString() + string.Format(" {{\"channel\": \"{0}\", \"character\": \"{1}\"}}", channelname, username);
            Console.WriteLine(toSend);
            await Client.SendAsync(toSend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="description"></param>
        public async void Mod_SetChannelDescription(string channel, string description)
        {
            if (!IsConnected()) throw new Exception("You must be connected to chat to do this.");

            string toSend = Hycybh.CDS.ToString() + string.Format(" {{\"channel\": \"{0}\", \"description\": \"{1}\"}}", channel, description);
            Console.WriteLine(toSend);
            await Client.SendAsync(toSend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="status"></param>
        /// <param name="duration"></param>
        public async void Mod_SetChannelUserStatus(string channel, string user, UserRoomStatus status, int duration = -1)
        {
            if (!IsConnected()) throw new Exception("You must be connected to chat to do this.");
            string toSend = string.Empty;
            switch (status)
            {
                case UserRoomStatus.Banned:
                    {
                        toSend = Hycybh.CDS.ToString() + string.Format(" {{\"character\": \"{0}\", \"channel\": \"{1}\"}}", user, channel);
                        Console.WriteLine($"Attempting to ban {user} from {channel}.");
                    }
                    break;
                case UserRoomStatus.Moderator:
                    {
                        toSend = Hycybh.COA.ToString() + string.Format(" {{\"character\": \"{0}\", \"channel\": \"{1}\"}}", user, channel);
                        Console.WriteLine($"Attempting to promote {user} to Channel Op in {channel}.");
                    }
                    break;
                case UserRoomStatus.User:
                    {
                        toSend = Hycybh.COR.ToString() + string.Format(" {{\"character\": \"{0}\", \"channel\": \"{1}\"}}", user, channel);
                        Console.WriteLine($"Attempting to demote {user} from Channel Op to basic User in {channel}.");
                    }
                    break;
                case UserRoomStatus.Kicked:
                    {
                        toSend = Hycybh.CKU.ToString() + string.Format(" {{\"character\": \"{0}\", \"channel\": \"{1}\"}}", user, channel);
                        Console.WriteLine($"Attempting to kick {user} out of {channel}.");
                    }
                    break;
                case UserRoomStatus.Timeout:
                    {
                        if (duration < 1) duration = 1;
                        if (duration > 90) duration = 90;
                        toSend = Hycybh.CTU.ToString() + string.Format(" {{\"character\": \"{0}\", \"channel\": \"{1}\", \"length\": \"{2}\"}}", user, channel, duration);
                        Console.WriteLine($"Attempting to timeout {user} from {channel} for {duration} seconds.");
                    }
                    break;
            }

            if (string.IsNullOrWhiteSpace(toSend))
                return;

            await Client.SendAsync(toSend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusType"></param>
        /// <param name="statusMessage"></param>
        /// <param name="user"></param>
        public async Task SetStatus(string statusType, string statusMessage, string user)
        {
            if (Enum.GetNames(typeof(ChatStatus)).Any(st => st == statusType))
            {
                await SetStatus((ChatStatus)Enum.Parse(typeof(ChatStatus), statusType), statusMessage, user);
            }

            throw new Exception($"Invalid ChatStatus: {statusType}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusType"></param>
        /// <param name="statusMessage"></param>
        /// <param name="user"></param>
        public async Task SetStatus(ChatStatus statusType, string statusMessage, string user)
        {
            int LENGTH_MAX = 255;

            statusMessage ??= string.Empty;
            if (statusMessage.Length > LENGTH_MAX /* STATUS MAX LENGTH */)
                throw new Exception($"Max message length of: {LENGTH_MAX} characters.");

            if (!IsConnected()) throw new Exception("You must be connected to chat to do this.");

            string toSend = Hycybh.STA.ToString() + string.Format(" {{\"status\": \"{0}\", \"statusmsg\": \"{1}\", \"character\": \"{2}\"}}", statusType.ToString().ToLowerInvariant(), statusMessage, user);
            Console.WriteLine(toSend);
            await Client.SendAsync(toSend);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="recipient"></param>
        /// <param name="messageType"></param>
        public void SendMessage(string channel, string message, string recipient, MessageType messageType = MessageType.Basic)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new Exception("Attempting to send an empty message.");

            if (string.IsNullOrWhiteSpace(recipient) && string.IsNullOrWhiteSpace(channel))
                throw new Exception("Attempting to send an empty user and channel.");

            lock (SocketLocker)
            {
                // extra whisper check, in case channel target has no valid value
                if (string.IsNullOrWhiteSpace(channel) && messageType != MessageType.Whisper)
                    messageType = MessageType.Whisper;

                // extra whisper check, in case recipient is invalid on a whisper
                if (messageType == MessageType.Whisper)
                {
                    if (string.IsNullOrWhiteSpace(recipient)) throw new Exception("Attempting to send a whisper with no valid target.");
                }

                if (!string.IsNullOrWhiteSpace(channel) && !channelTracker.GetChannelByNameOrCode(channel).AdEnabled && messageType == MessageType.Advertisement)
                {
                    Console.WriteLine("Error: Attempting to post an ad in a channel that doesn't support it.");
                }

                MessageQueue.Enqueue(new MessageContents(channel, message, recipient, messageType));
            }
        }
    }
}