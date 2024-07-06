using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using FChatApi.Enums;

namespace FChatApi.Core;

public partial class ApiConnection
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="channelname"></param>
    public static async Task JoinChannel(string channelname)
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
    public static IEnumerable<User> RequestUserList(UserStatus userStatus)
    {
        return UserTracker.GetUsersByStatus(userStatus).Select(u=>u.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static User GetUserByName(string name)
    {
        return UserTracker.GetUserByName(name);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool TryGetUserByName(string name, out User user)
    {
        user = UserTracker.GetUserByName(name);
        return  !(user.ChatStatus == ChatStatus.Offline);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channelType"></param>
    /// <returns></returns>
    public static IEnumerable<Channel> RequestChannelList(ChannelType channelType)
    {
        return ChannelTracker.GetChannelList(channelType).Select(ch=>ch.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nameorcode"></param>
    /// <returns></returns>
    public static Channel GetChannelByNameOrCode(string nameorcode)
    {
        return ChannelTracker.GetChannelByNameOrCode(nameorcode);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channelname"></param>
    public static async void LeaveChannel(string channelname)
    {
        if (!IsConnected())
            throw new Exception("You must be connected to chat to do this.");

        string toSend = Hycybh.LCH.ToString() + string.Format(" {{\"channel\": \"{0}\"}}", channelname);
        Console.WriteLine($"Attempting to leave channel: {channelname}");
        await Client.SendAsync(toSend);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channelName"></param>
    public static async void CreateChannel(string channelName)
    {
        if (!IsConnected())
            throw new Exception("You must be connected to chat to do this.");

        string toSend = Hycybh.CCR.ToString() + string.Format(" {{\"channel\": \"{0}\"}}", channelName);
        Console.WriteLine($"Attempting to create channel: {channelName}");
        ChannelTracker.StartChannelCreation(channelName);
        await Client.SendAsync(toSend);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="channelname"></param>
    public static async void Mod_InviteUserToChannel(string username, string channelname)
    {
        if (!IsConnected())
            throw new Exception("You must be connected to chat to do this.");

        string toSend = Hycybh.CIU.ToString() + string.Format(" {{\"channel\": \"{0}\", \"character\": \"{1}\"}}", channelname, username);
        Console.WriteLine(toSend);
        await Client.SendAsync(toSend);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="description"></param>
    public static async void Mod_SetChannelDescription(string channel, string description)
    {
        if (!IsConnected())
            throw new Exception("You must be connected to chat to do this.");

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
    public static async void Mod_SetChannelUserStatus(string channel, string user, UserRoomStatus status, int duration = -1)
    {
        if (!IsConnected())
            throw new Exception("You must be connected to chat to do this.");

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
    public static async Task SetStatus(string statusType, string statusMessage, string user)
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
    public static async Task SetStatus(ChatStatus statusType, string statusMessage, string user)
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
    public void EnqueueMessage(ChatMessageBuilder messageBuilder)
    {
        lock (SocketLocker)
        {
            MessageQueue.Enqueue(messageBuilder);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="message"></param>
    /// <param name="recipient"></param>
    /// <param name="messageType"></param>
    public void EnqueueMessage(string channel, string message, string recipient, ChatMessageType messageType = ChatMessageType.Basic)
    {
        EnqueueMessage(new ChatMessageBuilder()
            .WithRecipient(recipient)
            .WithChannel(ChannelTracker.GetChannelByNameOrCode(channel))
            .WithMessage(message)
            .WithMessageType(messageType)
        );
    }
}