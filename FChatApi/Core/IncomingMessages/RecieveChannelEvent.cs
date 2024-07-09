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
#region (-) Handler_JCH
	private Task Handler_JCH(JObject json)
	{
		if (TryGetChannelByCode(json["channel"].ToString(),out Channel channel) || ChannelTracker.IsChannelBeingCreated)
		{
			if (TryGetUserByName(json["character"]["identity"].ToString(),out User user) &&
				ChannelTracker.IsChannelBeingCreated &&
				channel is null &&
				user.Name.Equals(CharacterName))
			{
				return Task.Run(() =>
					{
						channel = ChannelTracker.FinalizeChannelCreation(json["title"].ToString(), json["channel"].ToString(), user);
						Console.WriteLine($"Created Channel: {json["channel"]}");
						CreatedChannelHandler?.Invoke(this, new ChannelEventArgs() { Channel = channel, User = user, ChannelStatus = ChannelStatus.Left });
					}
				);
			}
			
			if (user is not null)
			{
				return Task.Run(() =>
					{
						if (user.Name.Equals(CharacterName))
						{
							ChannelTracker.JoinedChannels.Add(channel.Code,channel);
						}
						JoinedChannelHandler?.Invoke(this, new ChannelEventArgs() { Channel = channel, User = user, ChannelStatus = ChannelStatus.Left });
						channel.AddUser(user);
						Console.WriteLine($"{user.Name} joined Channel: {channel.Name}. {channel.Users.Count} total users in channel.");
					}
				);
			}
		}
		throw new ArgumentException($"Incoming message was malformed: {json}",nameof(json));
	}
#endregion

#region (-) Handler_LCH
	private Task Handler_LCH(JObject json)
	{		
		if (TryGetChannelByCode(json["channel"].ToString(),out Channel channel) &&
			TryGetUserByName(json["character"].ToString(),out User user))
		{
			return Task.Run(() =>
				{
					if (user.Name.Equals(CharacterName))
					{
						LeaveChannel(channel);
					}
					else
					{
						channel.RemoveUser(user.Name);
					}
					LeftChannelHandler?.Invoke(this, new ChannelEventArgs() { Channel = channel, User = user, ChannelStatus = ChannelStatus.Left });
					Console.WriteLine($"{user.Name} left Channel: {json["channel"]}. {channel.Users.Count} total users in channel.");
				}
			);
		}
		throw new ArgumentException($"Incoming message was malformed: {json}",nameof(json));
	}
#endregion

#region (-) Handler_COL
	private Task Handler_COL(JObject json)
	{
		while (ChannelTracker.IsChannelBeingCreated);
		Channel channel		= GetChannelByNameOrCode(json["channel"].ToString());
		List<Task> tasks	= [];
		bool ownerAdded		= false;
		foreach (User user in json["oplist"].Select(user => GetUserByName(user.ToString())))
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
		return Task.Run(() => Task.WaitAll([.. tasks]));
	}
#endregion

#region (-) Handler_ICH
	private Task Handler_ICH(JObject json)
	{
		while (ChannelTracker.IsChannelBeingCreated);
		// joining channel
		Channel channel = ChannelTracker.ChangeChannelStatus(json["channel"].ToString(), ChannelStatus.Joined);
		List<Task> tasks = [];
		foreach (User user in json["users"].Select(u => UserTracker.GetUserByName(u["identity"].ToString())))
		{
			tasks.Add(Task.Run(() =>
				{
					if (user is null)
					{
						Console.WriteLine($"Error attempting to add user {user.Name} to {channel.Name} channel's userlist.");
					}

					channel.AddUser(user);
					channel.AdEnabled = !json["mode"].ToString().Equals("chat");
				}
			));
		}
#if DEBUG
		Console.WriteLine($"Adding {json["users"].Count()} users to {channel.Name} channel's userlist successful.");
#endif
		return Task.Run(() => Task.WaitAll([.. tasks]));
	}
#endregion

#region (-) Handler_CHA
	private Task Handler_CHA(JObject json)
	{
		List<Channel> publicChannelList = [];
		foreach (string channelName in json["channels"].Select(ch => ch["name"].ToString()))
		{					
			publicChannelList.Add(new Channel(channelName,channelName,ChannelType.Public));
		}

		return Task.Run(() =>
		{
			ChannelTracker.RefreshAvailableChannels(publicChannelList, ChannelType.Public);
			PublicChannelsReceivedHandler?.Invoke(this, new ChannelEventArgs() { });
			Console.WriteLine($"Public Channels Recieved... {publicChannelList.Count} total Public Channels.");
		});
	}
#endregion

#region (-) Handler_ORS
	private Task Handler_ORS(JObject json)
	{
		List<Channel> privateChannelList = [];
		foreach (var channel in json["channels"])
		{
			privateChannelList.Add(new Channel(channel["title"].ToString(), channel["name"].ToString(), ChannelType.Private));
		}

		return Task.Run(() =>
		{
			ChannelTracker.RefreshAvailableChannels(privateChannelList, ChannelType.Private);
			PrivateChannelsReceivedHandler?.Invoke(this, new ChannelEventArgs() { });
			Console.WriteLine($"Private Channels Recieved... {privateChannelList.Count} total Private Channels.");
		});
	}
#endregion
}
