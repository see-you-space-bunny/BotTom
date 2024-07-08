using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Interfaces;
using System.Linq;
using FChatApi.Attributes;
using System.Collections.Specialized;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (+) JoinChannel
	/// <summary>
	/// <b>Channel Action</b><br/>
	/// joins the selected channel
	/// </summary>
	/// <param name="channelcode">channel code</param>
	public static Task User_JoinChannel(string channelcode)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.JCH.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channelcode);
		Console.WriteLine($"Attempting to join: {channelcode}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}

	/// <summary>
	/// <b>Channel Action</b><br/>
	/// joins the selected channel
	/// </summary>
	/// <param name="channel">the channel we wish to join</param>
	public static Task User_JoinChannel(Channel value) =>
		User_JoinChannel(value.Code);
#endregion


#region (+) LeaveChannel
	/// <summary>
	/// <b>Channel Action</b><br/>
	/// leaves the selected channel
	/// </summary>
	/// <param name="channelcode">channel code</param>
	public static Task User_LeaveChannel(string channelcode)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.LCH.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channelcode);
		Console.WriteLine($"Attempting to leave channel: {channelcode}");
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}

	/// <summary>
	/// <b>Channel Action</b><br/>
	/// leaves the selected channel
	/// </summary>
	/// <param name="channel">the channel we wish to leave</param>
	public static Task User_LeaveChannel(Channel value) =>
		User_LeaveChannel(value.Code);
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
		string toSend = string.Format(MessageCode.CCR.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channelname);
		Console.WriteLine($"Attempting to create channel: {channelname}");
		ChannelTracker.StartChannelCreation(channelname);
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
	/// <param name="channelcode">channel code of the channel in which to change the description</param>
	/// <param name="description">description as <c>JavaScript</c> Encoded String</param>
	public static Task Mod_SetChannelDescription(string channelcode, string description)
	{
		ConnectionCheck();
		string toSend = string.Format(MessageCode.CDS.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,channelcode);
		Console.WriteLine(toSend);
#if DEBUG
		return DebugSendAsync(toSend);
#else
		return Client.SendAsync(toSend);
#endif
	}
	
	/// <summary>
	/// <b>Channel Action</b><br/>
	/// sets the channel's description
	/// </summary>
	/// <param name="channel">channel in which to change the description</param>
	/// <param name="description">description as <c>JavaScript</c> Encoded String</param>
	public static Task Mod_SetChannelDescription(Channel channel, string description) =>
		Mod_SetChannelDescription(channel.Code, description);
#endregion


////////////////////////////////////////////////


#region (-) RequestInternalChannelList
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
	/// <param name="channelcode"></param>
	/// <param name="status"></param>
	/// <param name="duration">duration of timeout, has no effect otherwise</param>
	/// <param name="channelname">channel name, used only for logging</param>
	internal static Task Mod_SetChannelUserStatus(string channelcode, string username, UserRoomStatus status, int duration = -1, string channelname = null)
	{
		ConnectionCheck();
        string toSend;
        switch (status)
		{
#region Banned
			case UserRoomStatus.Banned:
			{
				toSend = string.Format(MessageCode.CDS.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,username,channelcode);
				Console.WriteLine($"Attempting to ban {username} from {channelname ?? channelcode}.");
			}
			break;
#endregion

#region Timeout
			case UserRoomStatus.Timeout:
			{
				if (duration < MinimumChannelUserTimeoutValue || duration > MaximumChannelUserTimeoutValue)
					throw new ArgumentException($"Cannot timeout a user for {duration} seconds. Minimum is {MinimumChannelUserTimeoutValue} second{(MinimumChannelUserTimeoutValue==1?'s':string.Empty)}, maximum is {MaximumChannelUserTimeoutValue} second{(MaximumChannelUserTimeoutValue==1?'s':string.Empty)}.",nameof(duration));
				toSend = string.Format(MessageCode.CTU.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,username,channelcode,duration);
				Console.WriteLine($"Attempting to timeout {username} from {channelname ?? channelcode} for {duration} second{(duration==1?'s':string.Empty)}.");
			}
			break;
#endregion

#region Kicked
			case UserRoomStatus.Kicked:
			{
				toSend = string.Format(MessageCode.CKU.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,username,channelcode);
				Console.WriteLine($"Attempting to kick {username} out of {channelname ?? channelcode}.");
			}
			break;
#endregion

#region Demoted
			case UserRoomStatus.Demoted:
			{
				toSend = string.Format(MessageCode.COR.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,username,channelcode);
				Console.WriteLine($"Attempting to demote {username} from Channel Operator to basic User in {channelname ?? channelcode}.");
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
				toSend = string.Format(MessageCode.CIU.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,username,channelcode);
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
				toSend = string.Format(MessageCode.COA.GetEnumAttribute<MessageCode,OutgoingMessageFormatAttribute>().Format,username,channelcode);
				Console.WriteLine($"Attempting to promote {username} to Channel Operator in {channelname ?? channelcode}.");
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
	
	internal static Task Mod_SetChannelUserStatus(Channel channel, string username, UserRoomStatus status, int duration = -1) =>
		Mod_SetChannelUserStatus(channel.Code,username,status,duration,channel.Name);

	internal static Task Mod_SetChannelUserStatus(Channel channel, User user, UserRoomStatus status, int duration = -1) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,status,duration,channel.Name);

	internal static Task Mod_SetChannelUserStatus(string channelcode, User user, UserRoomStatus status, int duration = -1) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,status,duration);
#endregion


////////////////////////////////////////////////
///

#region (+) ChBanUser
	public static Task Mod_ChannelBanUser(Channel channel, string username, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.Banned,channelname:channel.Name);

	public static Task Mod_ChannelBanUser(string channelcode, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.Banned);

	public static Task Mod_ChannelBanUser(Channel channel, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.Banned,channelname:channel.Name);
#endregion

#region (+) ChTimeoutUser
	public static Task Mod_ChannelTimeoutUser(Channel channel, string username, UserRoomStatus status, int duration = -1) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.Timeout,duration,channel.Name);

	public static Task Mod_ChannelTimeoutUser(string channelcode, User user, UserRoomStatus status, int duration = -1) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.Timeout,duration);

	public static Task Mod_ChannelTimeoutUser(Channel channel, User user, UserRoomStatus status, int duration = -1) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.Timeout,duration,channel.Name);
#endregion

#region (+) ChKickUser
	public static Task Mod_ChannelKickUser(Channel channel, string username, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.Kicked,channelname:channel.Name);

	public static Task Mod_ChannelKickUser(string channelcode, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.Kicked);

	public static Task Mod_ChannelKickUser(Channel channel, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.Kicked,channelname:channel.Name);
#endregion

#region (+) ChDemoteUser
	public static Task Mod_ChannelDemoteUser(Channel channel, string username, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.Demoted,channelname:channel.Name);

	public static Task Mod_ChannelDemoteUser(string channelcode, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.Demoted);

	public static Task Mod_ChannelDemoteUser(Channel channel, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.Demoted,channelname:channel.Name);
#endregion


#region (+) ChUnbanUser
	public static Task Mod_ChannelUnbanUser(Channel channel, string username, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.UnBanned,channelname:channel.Name);

	public static Task Mod_ChannelUnbanUser(string channelcode, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.UnBanned);

	public static Task Mod_ChannelUnbanUser(Channel channel, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.UnBanned,channelname:channel.Name);
#endregion


#region (+) ChInviteUser
	public static Task Mod_ChannelInviteUser(Channel channel, string username, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.Invited,channelname:channel.Name);

	public static Task Mod_ChannelInviteUser(string channelcode, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.Invited);

	public static Task Mod_ChannelInviteUser(Channel channel, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.Invited,channelname:channel.Name);
#endregion


#region (+) ChResetUser
	public static Task Mod_ChannelResetUser(Channel channel, string username, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.User,channelname:channel.Name);

	public static Task Mod_ChannelResetUser(string channelcode, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.User);

	public static Task Mod_ChannelResetUser(Channel channel, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.User,channelname:channel.Name);
#endregion


#region (+) ChTrustUser
	public static Task Mod_ChannelTrustUser(Channel channel, string username, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.TrustedUser,channelname:channel.Name);

	public static Task Mod_ChannelTrustUser(string channelcode, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.TrustedUser);

	public static Task Mod_ChannelTrustUser(Channel channel, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.TrustedUser,channelname:channel.Name);
#endregion


#region (+) ChPromoteUser
	public static Task Mod_ChannelPromoteUser(Channel channel, string username, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.Moderator,channelname:channel.Name);

	public static Task Mod_ChannelPromoteUser(string channelcode, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.Moderator);

	public static Task Mod_ChannelPromoteUser(Channel channel, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.Moderator,channelname:channel.Name);
#endregion


#region (+) ChTransferOwner
	public static Task Mod_ChannelTransferOwner(Channel channel, string username, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,username,UserRoomStatus.Owner,channelname:channel.Name);

	public static Task Mod_ChannelTransferOwner(string channelcode, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channelcode,user.Name,UserRoomStatus.Owner);

	public static Task Mod_ChannelTransferOwner(Channel channel, User user, UserRoomStatus status) =>
		Mod_SetChannelUserStatus(channel.Code,user.Name,UserRoomStatus.Owner,channelname:channel.Name);
#endregion
}
