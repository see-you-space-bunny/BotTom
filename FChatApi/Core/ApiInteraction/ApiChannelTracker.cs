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
#region (+) LeaveChannel
	/// <summary>
	/// leaves the selected channel
	/// </summary>
	/// <param name="value">the channel we wish to leave</param>
	public static void LeaveChannel(Channel value)
	{
		ChannelTracker.LeaveChannel(value);
		value.RemoveUser(GetUserByName(CharacterName));
		ChannelTracker.ChangeChannelStatus(value, ChannelStatus.Available);
	}
#endregion

#region (+) ChListByType
	/// <summary>
	/// filters the channel list by type
	/// </summary>
	/// <param name="value"></param>
	/// <returns>an enumerable of channels that match the specified type</returns>
	public static IEnumerable<Channel> GetChannelListByType(ChannelType value) =>
		ChannelTracker.GetChannelList(value).Select(ch=>ch.Value);
#endregion


#region (+) ChByNameOrCode
	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static Channel GetChannelByNameOrCode(string value) =>
		ChannelTracker.GetChannelByNameOrCode(value);
#endregion


#region (+) Try: ChByNameOrCode
	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
	public static bool TryGetChannelByNameOrCode(string value, out Channel channel)
	{
		channel = GetChannelByNameOrCode(value);
		return channel is not null;
	}
#endregion


#region (+) ChByCode
	/// <summary>
	/// 
	/// </summary>
	/// <param name="nameorcode"></param>
	/// <returns></returns>
	public static Channel GetChannelByCode(string value) =>
		ChannelTracker.JoinedChannels.TryGetValue(value, out Channel channel) ?
			channel :
			throw new ArgumentException($"The requested channel \"{value}\" could not be found.",nameof(value));
#endregion


#region (+) Try: ChByCode
	/// <summary>
	/// 
	/// </summary>
	/// <param name="nameorcode"></param>
	/// <returns></returns>
	public static bool TryGetChannelByCode(string value, out Channel channel)
	{
		channel = GetChannelByNameOrCode(value);
		return channel is not null;
	}
#endregion
}