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
#region (-) Handler_STA
	private Task Handler_STA(JObject json) => 
		Task.Run(() =>
		{
			if (json["character"].ToString().Equals(CharacterName, StringComparison.InvariantCultureIgnoreCase))
			{
				Console.WriteLine("Status Changed");
			}
		});
#endregion

#region (-) Handler_MSG
	private Task Handler_MSG(JObject json,string message)
	{
		Task t = Task.Run(() => MessageHandler?.Invoke(MessageCode.MSG, new MessageEventArgs() { Channel = GetChannelByCode(json["channel"].ToString()), Message = json["message"].ToString(), User = GetUserByName(json["character"].ToString()) }));
		Console.WriteLine(message);
		return t;
	}
#endregion

#region (-) Handler_PRI
	private Task Handler_PRI(JObject json,string message)
	{
		Task t = Task.Run(() => MessageHandler?.Invoke(MessageCode.PRI, new MessageEventArgs() { User = GetUserByName(json["character"].ToString()), Message = json["message"].ToString() }));
		Console.WriteLine(message);
		return t;
	}
#endregion

#region (-) Handler_LIS
	private Task Handler_LIS(JObject json)
	{
		List<Task> tasks = [];
		foreach(var userinfo in json["characters"])
		{
			User tempUser = new()
			{
				Name		= userinfo[0].ToString(),
				Gender		= userinfo[1].ToString(),
				ChatStatus	= (ChatStatus)Enum.Parse(typeof(ChatStatus), userinfo[2].ToString().ToLowerInvariant(), true)
			};
			if (!UserTracker.TryAddUser(tempUser))
			{
				tasks.Add(Task.Run(
					() => UserTracker
						.GetUserByName(tempUser.Name)
						.Update(tempUser)
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

#region (-) Handler_FLN
	private Task Handler_FLN(JObject json)
	{
		if (TryGetUserByName(json["character"].ToString(),out User user))
			return Task.CompletedTask;

		UserTracker.Character_SetChatStatus(user, ChatStatus.Offline, false);

		return Task.Run(() =>
		{
			foreach (var channel in ChannelTracker.JoinedChannels.Values)
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
	private Task Handler_NLN(JObject json)
	{
		return Task.Run(() =>
		{
			User user = new ()
			{
				Name		= json["identity"].ToString(),
				UserStatus	= Enum.Parse<UserStatus>(json["status"].ToString(), true),
				Gender		= json["gender"].ToString(),
			};
			UserTracker.AddUser(user);
			UserTracker.Character_SetChatStatus(user, ChatStatus.Online, false);
		});
	}
#endregion

#region (-) Handler_IDN
	private Task Handler_IDN(JObject json)
	{
		Task[] tasks = new Task[2];
		ConnectedToChat?.Invoke(this, null);
#if !UNIT_TEST
		tasks[0] = RequestChannelListFromServer(ChannelType.Private);
		tasks[1] = RequestChannelListFromServer(ChannelType.Public);
#endif
		Console.WriteLine("Connected to Chat");
		return Task.Run(() => Task.WaitAll([.. tasks]));
	}
#endregion
}