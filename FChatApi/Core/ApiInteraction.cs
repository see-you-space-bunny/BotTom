using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Interfaces;

namespace FChatApi.Core;

public partial class ApiConnection
{
	private static void ConnectionCheck()
	{
		if (!IsConnected())
			throw new Exception("You must be connected to chat to do this.");
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channelname"></param>
	public static async Task JoinChannel(string channelname)
	{
		ConnectionCheck();
		string toSend = $"{MessageCode.JCH} {{\"channel\": \"{channelname}\"}}";
		Console.WriteLine($"Attempting to join: {channelname}");
		await Client.SendAsync(toSend);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channelType"></param>
	private async Task RequestInternalChannelList(ChannelType channelType)
	{
		ConnectionCheck();
		string toSend = channelType == ChannelType.Private ? MessageCode.ORS.ToString() : MessageCode.CHA.ToString();
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
		ConnectionCheck();
		string toSend = $"{MessageCode.LCH} {{\"channel\": \"{channelname}\"}}";
		Console.WriteLine($"Attempting to leave channel: {channelname}");
		await Client.SendAsync(toSend);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channelName"></param>
	public static async void CreateChannel(string channelName)
	{
		ConnectionCheck();
		string toSend = $"{MessageCode.CCR} {{\"channel\": \"{channelName}\"}}";
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
		ConnectionCheck();
		string toSend = $"{MessageCode.CIU} {{\"channel\": \"{channelname}\", \"character\": \"{username}\"}}";
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
		ConnectionCheck();
		string toSend = $"{MessageCode.CDS} {{\"channel\": \"{channel}\", \"description\": \"{description}\"}}";
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
		ConnectionCheck();
        string toSend;
        switch (status)
		{
			case UserRoomStatus.Banned:
				{
					toSend = $"{MessageCode.CDS} {{\"character\": \"{user}\", \"channel\": \"{channel}\"}}";
					Console.WriteLine($"Attempting to ban {user} from {channel}.");
				}
				break;
			case UserRoomStatus.Moderator:
				{
					toSend = $"{MessageCode.COA} {{\"character\": \"{user}\", \"channel\": \"{channel}\"}}";
					Console.WriteLine($"Attempting to promote {user} to Channel Op in {channel}.");
				}
				break;
			case UserRoomStatus.User:
				{
					toSend = $"{MessageCode.COR} {{\"character\": \"{user}\", \"channel\": \"{channel}\"}}";
					Console.WriteLine($"Attempting to demote {user} from Channel Op to basic User in {channel}.");
				}
				break;
			case UserRoomStatus.Kicked:
				{
					toSend = $"{MessageCode.CKU} {{\"character\": \"{user}\", \"channel\": \"{channel}\"}}";
					Console.WriteLine($"Attempting to kick {user} out of {channel}.");
				}
				break;
			case UserRoomStatus.Timeout:
				{
					if (duration < 1) duration = 1;
					if (duration > 90) duration = 90;
					toSend = $"{MessageCode.CTU} {{\"character\": \"{user}\", \"channel\": \"{channel}\", \"length\": \"{duration}\"}}";
					Console.WriteLine($"Attempting to timeout {user} from {channel} for {duration} seconds.");
				}
				break;
			default:
				return;
		}

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
		else
		{
			throw new Exception($"Invalid ChatStatus: {statusType}");
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="statusType"></param>
	/// <param name="statusMessage"></param>
	/// <param name="user"></param>
	public static async Task SetStatus(ChatStatus statusType, string statusMessage, string user)
	{
		ConnectionCheck();
		string toSend = $"{MessageCode.STA} {{\"status\": \"{statusType.ToString().ToLowerInvariant()}\", \"statusmsg\": \"{statusMessage ?? string.Empty}\", \"character\": \"{user}\"}}";
		Console.WriteLine(toSend);
		await Client.SendAsync(toSend);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="messageBuilder"></param>
	public void EnqueueMessage(FChatMessageBuilder messageBuilder)
	{
		lock (SocketLocker)
		{
			if (messageBuilder.HasRecipient)
			{
				MessageQueue.TryAdd(messageBuilder.MessageRecipient,new Queue<FChatMessageBuilder>());
				MessageQueue[messageBuilder.MessageRecipient].Enqueue(messageBuilder);
			}
			else
			{
				throw new InvalidOperationException("The message you are trying to send has no valid MessageRecipient.");
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="message"></param>
	/// <param name="recipient"></param>
	/// <param name="messageType"></param>
	public void EnqueueMessage(string channel, string message, string recipient, FChatMessageType messageType = FChatMessageType.Basic)
	{
		EnqueueMessage(new FChatMessageBuilder()
				.WithRecipient(recipient)
				.WithChannel(ChannelTracker.GetChannelByNameOrCode(channel))
				.WithMessage(message)
				.WithMessageType(messageType)
		);
	}
}