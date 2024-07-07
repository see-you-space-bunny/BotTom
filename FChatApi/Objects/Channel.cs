using System;
using System.Collections.Generic;
using System.Linq;
using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Interfaces;

namespace FChatApi.Objects;

public class Channel : IMessageRecipient
{
#region Properties (+)
	/// <summary>does the channel permit ads to be sent</summary>
	public bool AdEnabled { get; set; }

	/// <summary>the channel</summary>
	public string Name { get; set; }

	/// <summary>what is our status with the channel</summary>
	public ChannelStatus Status { get; set; }

	/// <summary>the channel's channel-code</summary>
	public string Code { get; set; }

	/// <summary>(accessibility) type of channel</summary>
	public ChannelType Type { get; set; }

	/// <summary>the channel's moderators</summary>
	internal Dictionary<string, User> Mods { get; set; }

	/// <summary>the channel's users</summary>
	internal Dictionary<string, User> Users { get; set; }

	/// <summary>did this api create the channel</summary>
	public bool CreatedByApi { get; set; }

	/// <summary>the channel's description</summary>
	public string Description { get; set; }

	/// <summary>the channel's owner</summary>
	public User Owner { get; set; }
#endregion


////////////////////////////////////////////////


#region Message Timers (+)
	/// <summary>the amount of miliseconds to wait before the next message may be sent</summary>
	public TimeSpan SleepInterval { get; }

	/// <summary>the next earliest point at which a message may be sent</summary>
	public DateTime Next { get; set; }

	/// <summary>sets the earliest time the next message can be sent</summary>
    void IMessageRecipient.MessageSent() => Next = DateTime.Now + SleepInterval;
#endregion


////////////////////////////////////////////////


#region Constructor (-)
	/// <summary>assigns default/empty values to <c>Status</c>,<c>Mods</c>,<c>Users</c>,<c>CreatedByApi</c> and <c>Description</c></summary>
	private Channel()
	{
		Status			= ChannelStatus.Available;
		Mods			= [];
		Users			= [];
		CreatedByApi	= false;
		Description		= string.Empty;
		SleepInterval	= new TimeSpan(0,0,0,0,milliseconds: 1001);
	}
#endregion


#region Constructor (+)
	/// <summary>
	/// this constructor does not assign <c>Description</c>,<c>Mods</c> or <c>Users</c>
	/// </summary>
	/// <param name="name">channel name</param>
	/// <param name="code">channel code</param>
	/// <param name="type">(accessibility) type of channel</param>
	/// <param name="adenabled">does the channel permit ads to be sent</param>
	public Channel(string name, string code, ChannelType type, bool adEnabled = false) : this()
	{
		Name		= name;
		Code		= code;
		Type		= type;
		AdEnabled	= adEnabled;
	}
#endregion


////////////////////////////////////////////////


#region Update (+)
	public void Update(Channel value)
	{
		Name			= value.Name;
		Type			= value.Type;
		AdEnabled		= value.AdEnabled;
		Status			= value.Status;
		Mods			= value.Mods;
		Users			= value.Users;
		Description		= value.Description;
	}
#endregion


////////////////////////////////////////////////


#region User Management (+)
	/// <summary>
	/// adds the user to the channel's user list<br/> this does invite/add the actual user form the channel
	/// </summary>
	/// <param name="value">the user we want to add</param>
	/// <returns>this channel</returns>
	public Channel AddUser(string value)
	{
		if (!Users.TryAdd(value, ApiConnection.GetUserByName(value)))
			Console.WriteLine($"Skipping duplicate entry: {value}");
		return this;
	}

	/// <summary>
	/// adds the user to the channel's user list<br/> this does invite/add the actual user form the channel
	/// </summary>
	/// <param name="value">the user we want to add</param>
	/// <returns>this channel</returns>
	public Channel AddUser(User value)
	{
		if (!Users.TryAdd(value.Name, value))
			Console.WriteLine($"Skipping duplicate entry: {value.Name}");
		return this;
	}

	/// <summary>
	/// removes the user from the channel's user list<br/> this does NOT kick the actual user form the channel
	/// </summary>
	/// <param name="value">the user we want to remove</param>
	/// <returns>this channel</returns>
	public Channel RemoveUser(string value)
	{
		Users.Remove(value);
		return this;
	}

	/// <summary>
	/// removes the user from the channel's user list<br/> this does NOT kick the actual user form the channel
	/// </summary>
	/// <param name="value">the user we want to remove</param>
	/// <returns>this channel</returns>
	public Channel RemoveUser(User value)
	{
		Users.Remove(value.Name);
		return this;
	}
#endregion


////////////////////////////////////////////////


#region Moderation
	/// <summary>
	/// adds channel moderator/op status to a user<br/>this has NO effect on bot-privileges/permissions
	/// </summary>
	/// <param name="value">the user we wish to Op</param>
	/// <returns>this channel</returns>
	public Channel AddMod(string value)
	{
		if (!Users.TryAdd(value, ApiConnection.GetUserByName(value)))
			Console.WriteLine($"Skipping duplicate mod entry: {value}");
		return this;
	}

	/// <summary>
	/// adds channel moderator/op status to a user<br/>this has NO effect on bot-privileges/permissions
	/// </summary>
	/// <param name="value">the user we wish to Op</param>
	/// <returns>this channel</returns>
	public Channel AddMod(User value)
	{
		if (!Users.TryAdd(value.Name, value))
			Console.WriteLine($"Skipping duplicate mod entry: {value.Name}");
		return this;
	}

	/// <summary>
	/// removes channel moderator/op status from a user<br/>this has NO effect on bot-privileges/permissions
	/// </summary>
	/// <param name="value">the user we wish to de-Op</param>
	/// <returns>this channel</returns>
	public Channel RemoveMod(string value)
	{
		Mods.Remove(value);
		return this;
	}

	/// <summary>
	/// removes channel moderator/op status from a user<br/>this has NO effect on bot-privileges/permissions
	/// </summary>
	/// <param name="value">the user we wish to de-Op</param>
	/// <returns>this channel</returns>
	public Channel RemoveMod(User value)
	{
		Mods.Remove(value.Name);
		return this;
	}
#endregion
}