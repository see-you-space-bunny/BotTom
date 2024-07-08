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
#region (+) UserListByStatus
	/// <summary>
	/// filters the list of known users by status
	/// </summary>
	/// <param name="value">the status we are filtering for</param>
	/// <returns>enumerable of users with matching status</returns>
	public static IEnumerable<User> GetUserListByStatus(UserStatus value) =>
		UserTracker.GetUsersByStatus(value).Select(u=>u.Value);
#endregion


#region (+) UserByName
	/// <summary>
	/// gets a character by that character's full name
	/// </summary>
	/// <param name="value">character name</param>
	/// <returns>the selected character</returns>
	public static User GetUserByName(string value) =>
		UserTracker.GetUserByName(value);
#endregion


#region (+) Try: OnlineUserByName
	/// <summary>
	/// tries to get a character by that character's full name
	/// </summary>
	/// <param name="value">name of the requested character</param>
	/// <param name="user">instance of the requested character</param>
	/// <returns><c>false</c> if the character is offline</returns>
	public static bool TryGetOnlineUserByName(string value, out User user)
	{
		user = UserTracker.GetUserByName(value);
		return user is not null && !(user.ChatStatus == ChatStatus.Offline);
	}
#endregion


#region (+) Try: UserByName
	/// <summary>
	/// tries to get a character by that character's full name
	/// </summary>
	/// <param name="value">name of the requested character</param>
	/// <param name="user">instance of the requested character</param>
	/// <returns><c>false</c> if the character was not found</returns>
	public static bool TryGetUserByName(string value, out User user)
	{
		user = UserTracker.GetUserByName(value);
		return user is not null;
	}
#endregion
}