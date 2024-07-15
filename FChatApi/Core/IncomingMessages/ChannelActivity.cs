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
#region (-) Handler_COL
	/// <summary><b>Channel Operator List</b><br/>
	/// incoming with a list of channel operators<br/>
	/// (INC) &lt;&lt; COL {"channel": "ChannelCode", "oplist": Array["OperatorName"]}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_COL(JObject json,bool logging = true)
	{
		List<Task> tasks	= [];
		tasks.Add(Task.Run(() => {
			Channels.CreationSemaphore.Wait();
			Channels.CreationSemaphore.Release();
			Channel channel		= Channels.SingleByNameOrCode(json["channel"].ToString());
			bool ownerAdded		= false;
			foreach (User user in json["oplist"].Select(u => Users.SingleByName(u.ToString())))
			{
				if (user is null)
				{
					ownerAdded		= true;
					continue;
				}
				else if (!ownerAdded)
				{
					channel.Owner	= user;
					ownerAdded		= true;
				}
				else
				{
					tasks.Add(Task.Run(() => channel.AddMod(user)));
				}
			}
#if DEBUG
			Console.WriteLine($"Found {channel.Mods.Count} mods for channel: {channel.Name}");
#endif
		}));
		return Task.Run(() => Task.WaitAll([.. tasks]));
	}
#endregion


#region (-) Handler_ICH
	/// <summary><b>Chat Login</b><br/>
	/// incoming when api-user has successfully identified with the server<br/>
	/// (INC) &lt;&lt; IDN {"character": "ApiUserName"}<br/>
	/// <i><b>DO NOT</b> issue chat commands until you have also recieved the NLN command with the current character's name!!<br/>
	/// Doing so will cause desynchronization with the server.</i><br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_ICH(JObject json,bool logging = true)
	{
		List<Task> tasks = [];
		tasks.Add(Task.Run(() =>
		{
			Channels.CreationSemaphore.Wait();
			Channels.CreationSemaphore.Release();
			Channel channel = Channels.Channel_ChangeStatus(json["channel"].ToString(), UserRelationshipWithChannel.Joined);
			foreach (User user in json["users"].Select(u => Users.SingleByName(u["identity"].ToString())))
			{
				if (user is null)
				{
					Console.WriteLine($"Error attempting to add user {user.Name} to {channel.Name} channel's userlist.");
				}

				channel.AddUser(user);
				channel.AdEnabled = !json["mode"].ToString().Equals("chat");
				
			}
#if DEBUG
			Console.WriteLine($"Adding {json["users"].Count()} users to {channel.Name} channel's userlist successful.");
#endif
		}));
		return Task.Run(() => Task.WaitAll([.. tasks]));
	}
#endregion


#region (-) Handler_JCH
	/// <summary><b>Join Channel</b><br/>
	/// incoming when api-user has joined a channel<br/>
	/// (INC) &lt;&lt; JCH {"character": ChannelCharacter, "channel": "ChannelCode", "title": "ChannelName"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_JCH(JObject json,bool logging = true)
	{
		if (!Users.TrySingleByName(json["character"]["identity"].ToString(),out User user))
			throw new ArgumentException($"Incoming message was malformed: {json}",nameof(json));

		if (!Channels.TrySingleByNameOrCode(json["channel"].ToString(),out Channel channel) && Channels.IsChannelBeingCreated && user.Name.Equals(ApiUser.Name))
		{
			return Task.Run(() =>
			{
				channel = Channels.FinalizeChannelCreation(json["title"].ToString(), json["channel"].ToString(), user);
				Console.WriteLine($"Created Channel: {json["channel"]}");
				user.JoinChannel(channel);
				CreatedChannelHandler?.Invoke(this, new ChannelEventArgs() { Channel = channel, User = user, ChannelStatus = UserRelationshipWithChannel.Left });
			});
		}

		return Task.Run(() =>
		{
			user.JoinChannel(channel);
			JoinedChannelHandler?.Invoke(this, new ChannelEventArgs() { Channel = channel, User = user, ChannelStatus = UserRelationshipWithChannel.Left });
			Console.WriteLine($"{user.Name} joined Channel: {channel.Name}. {channel.Users.Count} total users in channel.");
		});
	}
#endregion


#region (-) Handler_LCH
	/// <summary><b>Leave Channel</b><br/>
	/// incoming when a character leaves a channel<br/>
	/// (INC) &lt;&lt; LCH {"character": "CharacterName", "channel": "ChannelCode"}<br/>
	/// </summary>
	/// <param name="json">the incoming message's contents</param>
	/// <returns>the task we initiated</returns>
	private Task Handler_LCH(JObject json,bool logging = true)
	{
		return Task.Run(() => {
			Channels.CreationSemaphore.Wait();
			Channels.CreationSemaphore.Release();
			if (Channels.TrySingleByNameOrCode(json["channel"].ToString(),out Channel channel) &&
				Users.TrySingleByName(json["character"].ToString(),out User user))
			{
					user.LeaveChannel(channel);
					LeftChannelHandler?.Invoke(this, new ChannelEventArgs() { Channel = channel, User = user, ChannelStatus = UserRelationshipWithChannel.Left });
					Console.WriteLine($"{user.Name} left Channel: {json["channel"]}. {channel.Users.Count} total users in channel.");
				
			}
			//throw new ArgumentException($"Incoming message was malformed: {json}",nameof(json));
		});
	}
#endregion
}
