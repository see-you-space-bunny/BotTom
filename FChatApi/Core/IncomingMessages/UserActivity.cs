using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Enums;
using FChatApi.EventArguments;
using FChatApi.Objects;
using Newtonsoft.Json.Linq;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region (-) Handler_FLN
	/// <summary><b>Offline User Notification</b><br/>
	/// incoming when a character has gone offline<br/>
	/// (INC) &lt;&lt; FLN {"character": "CharacterName"}
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_FLN(JObject json)
	{
		if (TryGetUser(json["character"].ToString(),out User user))
			return Task.CompletedTask;

		Users.Character_SetChatStatus(user, ChatStatus.Offline, false);

		return Task.Run(() =>
		{
			foreach (var channel in Channels.Joined.Values)
			{
				bool needsRemoved = false;

				if (channel.Users.ContainsKey(user.Name))
				{
					needsRemoved = true;
				}

				if (needsRemoved)
				{
					channel.RemoveUser(user);
				}
			}
		}
		);
	}
#endregion


#region (-) Handler_NLN
	/// <summary><b>Channel Message</b><br/>
	/// incoming when a character has come online<br/>
	/// (INC) &lt;&lt; NLN {"gender": "A String", "status": "A String", "identity": "CharacterName"}<br/>
	/// <i>This includes the api-user! Wait to recieve this message before issuing any chat commands!!</i>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_NLN(JObject json)
	{
		return Task.Run(() =>
		{
			User user = new ()
			{
				Name		= json["identity"].ToString(),
				UserStatus	= Enum.Parse<RelationshipToApiUser>(json["status"].ToString(), true),
				Gender		= json["gender"].ToString(),
			};
			Users.Add(user);
			Users.Character_SetChatStatus(user, ChatStatus.Online, false);
		});
	}
#endregion


#region (-) Handler_STA
	/// <summary><b>Status Update</b><br/>
	/// incoming with a status update alert<br/>
	/// (INC) &lt;&lt; STA {"character": "CharacterName", "status": "UserStatusType", "statusmsg": "A String"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_STA(JObject json) => 
		Task.Run(() =>
		{
			if (json["character"].ToString().Equals(CharacterName, StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("Status Changed");
			}
		});
#endregion
}