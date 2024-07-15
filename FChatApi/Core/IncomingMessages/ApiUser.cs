using System;
using System.Collections.Generic;
using System.IO;
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
	private Task Handler_FRL(JObject json,bool logging = true)
	{
		return Task.CompletedTask;
	}
#endregion


#region (-) Handler_LIS
	/// <summary><b>Character List</b><br/>
	/// incoming list of characters, recieved on login<br/>
	/// (INC) &lt;&lt; LIS {"characters": [["CharacterName","Gender","RelationshipToApiUser"]] }
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_LIS(JObject json,bool logging = true)
	{
		List<Task> tasks = [];
		foreach(var userinfo in json["characters"])
		{
			tasks.Add(Task.Run(() => 
			{
				if (Users.TrySingleByName(userinfo[0].ToString(), out User user))
				{
					user.Gender		= userinfo[1].ToString();
					Users.Character_SetChatStatus(user,(ChatStatus)Enum.Parse(typeof(ChatStatus), userinfo[2].ToString(), true),false);
				}
				else
				{
					Users.AddOrUpdate(new User()
					{
						Name		= userinfo[0].ToString(),
						Gender		= userinfo[1].ToString(),
						ChatStatus	= (ChatStatus)Enum.Parse(typeof(ChatStatus), userinfo[2].ToString(), true)
					});
				}
			}));
		}
#if DEBUG
		Console.WriteLine($"Added {json["characters"].Count()} users. Total users: {Users.KnownUsers.Count}");
#endif
		return Task.Run(() => Task.WaitAll([.. tasks]));
	}
#endregion
}