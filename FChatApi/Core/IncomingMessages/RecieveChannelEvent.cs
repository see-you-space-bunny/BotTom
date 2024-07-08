using System;
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
		if (!TryGetChannelByCode(json["channel"].ToString(),out Channel channel) && !ChannelTracker.IsChannelBeingCreated)
		{
			return Task.CompletedTask;
		}

		if (TryGetUserByName(json["character"]["identity"].ToString(),out User user) &&
			channel.Status == ChannelStatus.Creating &&
			user.Name.Equals(CharacterName))
		{

			return Task.Run(() =>
				{
					channel = ChannelTracker.FinalizeChannelCreation(json["title"].ToString(), json["channel"].ToString(), user);
					Console.WriteLine($"Created Channel: {json["channel"]}");
					CreatedChannelHandler?.Invoke(this, new ChannelEventArgs() { name = channel.Name, status = ChannelStatus.Joined, code = channel.Code, type = channel.Type });
				}
			);
		}
		else if (user is not null && channel is not null)
		{
			return Task.Run(() =>
				{
					if (user.Name.Equals(CharacterName))
					{
						ChannelTracker.JoinedChannels.Add(channel.Code,channel);
					}
					JoinedChannelHandler?.Invoke(this, new ChannelEventArgs() {
						name = json["title"].ToString(),
						status = ChannelStatus.Joined,
						code = channel.Code,
						type = channel.Type,
						userJoining = user.Name
					});
					channel.AddUser(user);
					Console.WriteLine($"{user.Name} joined Channel: {channel.Name}. {channel.Users.Count} total users in channel.");
				}
			);
		}
		//return Task.Run(() => ChannelTracker.AddManualChannel(json["title"].ToString(), ChannelStatus.Available, json["channel"].ToString()));
		throw new ArgumentException($"Incoming message was malformed: {json}",nameof(json));
	}
#endregion

#region (-) Handler_LCH
	private Task Handler_LCH(JObject json)
	{
		if (!TryGetUserByName(json["character"].ToString(),out User user) && user.Name.Equals(CharacterName))
		{
		}
		
		if (TryGetChannelByCode(json["channel"].ToString(),out Channel channel) &&
			user is not null && user.Name.Equals(CharacterName))
		{
			return Task.Run(() =>
				{
					LeaveChannel(channel);
					LeftChannelHandler?.Invoke(this, new ChannelEventArgs() { name = json["channel"].ToString(), status = ChannelStatus.Left });
					Console.WriteLine($"{user.Name} left Channel: {json["channel"]}. {channel.Users.Count} total users in channel.");
				}
			);
		}
		throw new ArgumentException($"Incoming message was malformed: {json}",nameof(json));
	}
#endregion
}
