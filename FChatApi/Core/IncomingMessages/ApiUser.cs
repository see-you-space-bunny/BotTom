using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.Objects;
using Newtonsoft.Json.Linq;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (-) Handler_FRL
	/// <summary><b>Friends List</b><br/>
	/// incoming with a list of api-user's friends<br/>
	/// (INC) &lt;&lt; FRL {"characters": Array["FriendName"]}
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_FRL(JObject json)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_LIS
	/// <summary><b>Character List</b><br/>
	/// incoming with api-user's character list and online status ?
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_LIS(JObject json)
	{
		List<Task> tasks = [];
		foreach(var userinfo in json["characters"])
		{
			User user = new()
			{
				Name		= userinfo[0].ToString(),
				Gender		= userinfo[1].ToString(),
				ChatStatus	= (ChatStatus)Enum.Parse(typeof(ChatStatus), userinfo[2].ToString().ToLowerInvariant(), true)
			};
			if (!UserTracker.TryAddUser(user))
			{
				tasks.Add(Task.Run(
					() => UserTracker
						.GetUserByName(user.Name)
						.Update(user)
				));
			}
			//UserTracker.SetChatStatus(tempUser, tempUser.ChatStatus, false);
		}
#if DEBUG
		Console.WriteLine($"Added {json["characters"].Count()} users. Total users: {UserTracker.Count}");
#endif
		return Task.Run(() => Task.WaitAll([.. tasks]));
	}
#endregion
}