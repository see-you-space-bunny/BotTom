using System;
using System.Collections.Generic;
using System.Linq;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Core;
using System.Collections.Concurrent;

namespace FChatApi.Systems;

public class UserTracker
{
#region (-) RegisteredUsers
	public readonly ConcurrentDictionary<string,User> RegisteredUsers;
#endregion


#region (-) KnownUsers
	internal readonly ConcurrentDictionary<string,User> KnownUsers;
#endregion


#region (-) OnlineUsers
	internal readonly ConcurrentDictionary<string,User> OnlineUsers;
#endregion


#region (~) Constructor
	internal UserTracker()
	{
		RegisteredUsers	= new ConcurrentDictionary<string,User>(StringComparer.InvariantCultureIgnoreCase);
		KnownUsers		= new ConcurrentDictionary<string,User>(StringComparer.InvariantCultureIgnoreCase);
		OnlineUsers		= new ConcurrentDictionary<string,User>(StringComparer.InvariantCultureIgnoreCase);
	}
#endregion


#region (~) Add
	internal void Add(User value)
	{
		if (!TryAdd(value))
			KnownUsers[value.Name] = value;
	}
#endregion


#region (-) TryAdd
	internal bool TryAdd(User value)
	{
		if (KnownUsers.TryAdd(value.Name, value))
		{
			if (value.IsRegistered)
				if (!RegisteredUsers.TryAdd(value.Name,value))
					RegisteredUsers[value.Name] = value;
			return true;
		}
		return false;
	}
#endregion


#region (~) ByStatus
	/// <summary>
	/// tries to get a character by that character's full name
	/// </summary>
	/// <param name="status">name of the requested character</param>
	/// <param name="user">Instance of the requested character.<br/>
	/// the method will still attempt to find the user if they are not online.</param>
	/// <returns><c>false</c> if the character is offline</returns>
	public Dictionary<string,User> FilterByStatus(ChatStatus status) =>
		(status switch {
			ChatStatus.Any			=> KnownUsers,
			ChatStatus.AnyOnline	=> OnlineUsers,
			_ => KnownUsers.Where(user => user.Value.ChatStatus.Equals(status)),
		}).ToDictionary();		
#endregion


#region (~) ByRelationship
	public Dictionary<string,User> FilterByRelationship(RelationshipToApiUser relationship) =>
		KnownUsers.Where(u => u.Value.UserStatus.Equals(relationship)).ToDictionary();
#endregion


#region (+) TryUserByName
	/// <summary>
	/// tries to get a character by that character's full name
	/// </summary>
	/// <param name="value">name of the requested character</param>
	/// <param name="user">instance of the requested character</param>
	/// <returns><c>false</c> if the character was not found</returns>
	public bool TrySingleByName(string value, out User user)
	{
		value = value.ToLowerInvariant();
		if (!OnlineUsers.TryGetValue(value, out user))
			if (!RegisteredUsers.TryGetValue(value, out user))
				if (!KnownUsers.TryGetValue(value, out user))
					return false;
		return user is not null;
	}
#endregion


#region (+) ByName
	/// <summary>
	/// gets a character by that character's full name
	/// </summary>
	/// <param name="value">character name</param>
	/// <returns>the selected character</returns>
	public User SingleByName(string username) 
		=> SingleByKey(username);
#endregion


#region (~) UserByKey
	internal User SingleByKey(string username) =>
		KnownUsers.TryGetValue(username, out User result) ? result : null;
#endregion


#region (~) SetChatStatus
	internal void Character_SetChatStatus(User user, ChatStatus status, bool logging = true)
	{
		UserSanityCheck(user);
		user.ChatStatus = status;

		if (status > ChatStatus.AnyOnline)
			OnlineUsers.TryAdd(user.Name,user);
		else
			OnlineUsers.Remove(user.Name, out _);

		if (logging)
			Console.WriteLine($"{user.Name}'s chat status changed to: {status}");
	}
#endregion


#region (~) SetUserStatus
	internal void Character_SetRelationship(User user, RelationshipToApiUser status, bool logging = true)
	{
		UserSanityCheck(user);
		user.UserStatus = status;

		if (logging)
			Console.WriteLine($"{user.Name}'s user status changed to: {status}");
	}
#endregion


#region (-) UserSanityCheck
	private void UserSanityCheck(User user)
	{
		if (!KnownUsers.ContainsKey(user.Name))
			Add(user);
	}
#endregion
}