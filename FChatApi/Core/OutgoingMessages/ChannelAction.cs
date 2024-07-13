using System;
using System.Threading.Tasks;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Attributes;
using System.Text.Encodings.Web;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (+) JoinChannel
	/// <summary>
	/// <b>Channel Action</b><br/>
	/// joins the selected channel
	/// </summary>
	/// <param name="channel">channel code</param>
	public static Task User_JoinChannel(Channel channel)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.JCH.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channel);
		Console.WriteLine($"Attempting to join: {channel.Code}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
#endregion


#region (+) LeaveChannel
	/// <summary>
	/// <b>Channel Action</b><br/>
	/// leaves the selected channel
	/// </summary>
	/// <param name="channel">the channel we wish to leave</param>
	public static Task User_LeaveChannel(Channel channel)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.LCH.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channel);
		Console.WriteLine($"Attempting to leave channel: {channel.Code}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
#endregion


////////////////////////////////////////////////


#region (+) CreateChannel
	/// <summary>
	/// <b>Channel Action</b><br/>
	/// attempts to create a private channel with the provided name
	/// </summary>
	/// <param name="channelname">channel name as <c>JavaScript</c> Encoded String</param>
	public static Task User_CreateChannel(string channelname)
	{
		ConnectionCheck();
		string toSend = string.Format(
			MessageCode.CCR.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,
			System.Web.HttpUtility.JavaScriptStringEncode(channelname)
		);
		Console.WriteLine($"Attempting to create channel: {channelname}");
		Channels.StartChannelCreation(channelname);
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
#endregion


////////////////////////////////////////////////


#region (+) SetChDescription
	/// <summary>
	/// <b>Channel Action</b><br/>
	/// sets the channel's description
	/// </summary>
	/// <param name="channel">channel in which to change the description</param>
	/// <param name="description">description as <c>JavaScript</c> Encoded String</param>
	public static Task Mod_SetChannelDescription(Channel channel, string description)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.CDS.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channel.Code);
		Console.WriteLine(toSend);
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
#endregion


////////////////////////////////////////////////
////////////////////////////////////////////////
////////////////////////////////////////////////


#region (-) RequestChannelList
	/// <summary>
	/// 
	/// </summary>
	/// <param name="channelType"></param>
	private Task RequestChannelListFromServer(ChannelType channelType)
	{
		ConnectionCheck();
		Console.WriteLine($"Attempting to retrieve all {channelType} channels from server.");
#if DEBUG
		return DebugSendAsync(channelType == ChannelType.Private ? MessageCode.ORS.ToString() : MessageCode.CHA.ToString());
#else
		return Client.SendAsync(channelType == ChannelType.Private ? MessageCode.ORS.ToString() : MessageCode.CHA.ToString());
#endif
	}
#endregion


////////////////////////////////////////////////
////////////////////////////////////////////////
////////////////////////////////////////////////


#region (+) SetChUserStatus
	/// <summary>
	/// <b>Channel Action</b><br/>
	/// 
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="status"></param>
	/// <param name="duration">duration of timeout, has no effect otherwise</param>
	/// <param name="channelname">channel name, used only for logging</param>
	public static Task Mod_SetChannelUserStatus(Channel channel, User user, UserRoomStatus status, int duration = -1)
	{
		ConnectionCheck();
        string toSend;
        switch (status)
		{
#region Banned
			case UserRoomStatus.Banned:
			{
				toSend = string.Format(MessageCode.CDS.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,user.Name,channel);
				Console.WriteLine($"Attempting to ban {user.Name} from {channel.Name}.");
			}
			break;
#endregion

#region Timeout
			case UserRoomStatus.Timeout:
			{
				if (duration < MinimumChannelUserTimeoutValue || duration > MaximumChannelUserTimeoutValue)
					throw new ArgumentException($"Cannot timeout a user for {duration} seconds. Minimum is {MinimumChannelUserTimeoutValue} second{(MinimumChannelUserTimeoutValue==1?'s':string.Empty)}, maximum is {MaximumChannelUserTimeoutValue} second{(MaximumChannelUserTimeoutValue==1?'s':string.Empty)}.",nameof(duration));
				toSend = string.Format(MessageCode.CTU.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,user.Name,channel,duration);
				Console.WriteLine($"Attempting to timeout {user.Name} from {channel.Name} for {duration} second{(duration==1?'s':string.Empty)}.");
			}
			break;
#endregion

#region Kicked
			case UserRoomStatus.Kicked:
			{
				toSend = string.Format(MessageCode.CKU.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,user.Name,channel);
				Console.WriteLine($"Attempting to kick {user.Name} out of {channel.Name}.");
			}
			break;
#endregion

#region Demoted
			case UserRoomStatus.Demoted:
			{
				toSend = string.Format(MessageCode.COR.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,user.Name,channel);
				Console.WriteLine($"Attempting to demote {user.Name} from Channel Operator to basic User in {channel.Name}.");
			}
			break;
#endregion

#region UnBanned
			case UserRoomStatus.UnBanned:
			{
				throw new NotImplementedException();
			}
			//break;
#endregion

#region Invited
			case UserRoomStatus.Invited:
			{
				toSend = string.Format(MessageCode.CIU.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,user.Name,channel);
				Console.WriteLine(toSend);
			}
			break;
#endregion

#region User
			case UserRoomStatus.User:
			{
				throw new NotImplementedException();
			}
			//break;
#endregion

#region TrustedUser
			case UserRoomStatus.TrustedUser:
			{
				throw new NotImplementedException();
			}
			//break;
#endregion

#region Moderator
			case UserRoomStatus.Moderator:
			{
				toSend = string.Format(MessageCode.COA.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,user.Name,channel);
				Console.WriteLine($"Attempting to promote {user.Name} to Channel Operator in {channel.Name}.");
			}
			break;
#endregion

#region Owner
			case UserRoomStatus.Owner:
			{
				throw new NotImplementedException();
			}
			//break;
#endregion

			default:
				throw new ArgumentException($"Cannot set a user's status to {status}.",nameof(status));
		}
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
#endregion
}
