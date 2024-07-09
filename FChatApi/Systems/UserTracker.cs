using System;
using System.Collections.Generic;
using System.Linq;
using FChatApi.Objects;
using FChatApi.Enums;

namespace FChatApi.Systems;

internal class UserTracker
{
#region (-) KnownUsers
	private readonly Dictionary<string,User> KnownUsers;
#endregion


#region (~) Constructor
	internal UserTracker()
	{
		KnownUsers = [];
	}
#endregion


#region (~) Count
	internal int Count => KnownUsers.Count;
#endregion


#region (~) AddUser
	internal void AddUser(User value) =>
		TryAddUser(value);
#endregion


#region (-) Try: AddUser
	internal bool TryAddUser(User value) =>
		KnownUsers.TryAdd(value.Key, value);
#endregion


#region (~) SetUserStatus
	internal IEnumerable<KeyValuePair<string,User>> GetUsersByStatus(UserStatus status)
	{
		return KnownUsers.Where(user => user.Value.UserStatus == status);
	}
#endregion


#region (~) UserByName
	internal User GetUserByName(string value) 
		=> GetUserByKey(value.ToLowerInvariant());
#endregion


#region (~) UserByKey
	internal User GetUserByKey(string value) =>
		KnownUsers.TryGetValue(value, out User user) ? user : null;
#endregion


#region (~) SetChatStatus
	internal void Character_SetChatStatus(User user, ChatStatus status, bool logging = true)
	{
		user.ChatStatus = status;
		AddUser(user);

		if (logging)
			Console.WriteLine($"{user.Name}'s chat status changed to: {status}");
	}
#endregion


#region (~) SetUserStatus
	internal void Character_SetUserStatus(User user, UserStatus status, bool logging = true)
	{
		user.UserStatus = status;
		if (GetUserByName(user.Key) == null)
			AddUser(user);

		User thisUser = KnownUsers[user.Key];
		if (null == thisUser)
			throw new Exception($"Error attempting to resolve user: {user.Name}.");

		thisUser.UserStatus = status;
		if (logging) Console.WriteLine($"{thisUser.Name}'s user status changed to: {status}");
	}
#endregion
}