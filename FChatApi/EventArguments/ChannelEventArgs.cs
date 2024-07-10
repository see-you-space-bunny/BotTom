using System;
using FChatApi.Enums;
using FChatApi.Objects;

namespace FChatApi.EventArguments;

public class ChannelEventArgs : EventArgs
{
	/// <summary>
	/// the channel where the event happened
	/// </summary>
	public Channel Channel;

	/// <summary>
	/// user that triggered the event channel
	/// </summary>
	public User User;

	/// <summary>
	/// number of current users
	/// </summary>
	/// 
	public int UserCount;
	/// <summary>
	/// what kind of event has happened
	/// </summary>
	public UserRelationshipWithChannel ChannelStatus;
}
