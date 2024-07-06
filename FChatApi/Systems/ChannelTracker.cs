using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FChatApi.Objects;
using FChatApi.Enums;

namespace FChatApi.Systems;

public class ChannelTracker
{

	/// <summary>
	/// 
	/// </summary>
	Dictionary<string,Channel> AvailableChannels;

	/// <summary>
	/// 
	/// </summary>
	IEnumerable<KeyValuePair<string,Channel>> AvailablePublicChannels => AvailableChannels.Where((ch)=>ch.Value.Type == ChannelType.Public);

	/// <summary>
	/// 
	/// </summary>
	IEnumerable<KeyValuePair<string,Channel>> AvailablePrivateChannels => AvailableChannels.Where((ch)=>ch.Value.Type == ChannelType.Private);

	/// <summary>
	/// 
	/// </summary>
	public Dictionary<string,Channel> WatchChannels;

	private Channel ChannelBeingCreated;

	/// <summary>
	/// 
	/// </summary>
	public ChannelTracker()
	{
		AvailableChannels = [];
		WatchChannels = [];
	}

	public Channel AddManualChannel(string channelname, ChannelStatus status, string channelcode)
	{
		if (AvailableChannels.ContainsKey(channelname) && !AvailableChannels.Values.Any(ch => ch.Name.Equals(channelname)))
		{
			Channel ch = new(channelname, channelcode, ChannelType.Private)
			{
				Status = status
			};
			AvailableChannels.Add(channelcode,ch);
			return ch;
		}

		return null;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channelname"></param>
	/// <param name="status"></param>
	public void StartChannelCreation(string channelname, ChannelStatus status = ChannelStatus.Creating)
	{
		if (ChannelBeingCreated is null)
		{
			Channel toAdd = new Channel(channelname, string.Empty, ChannelType.Private)
			{
				Status = status
			};
			ChannelBeingCreated = toAdd;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channelname"></param>
	/// <param name="channelcode"></param>
	/// <param name="owner"></param>
	/// <returns></returns>
	public Channel FinalizeChannelCreation(string channelname, string channelcode, User owner)
	{
		if (ChannelBeingCreated.Status == ChannelStatus.Creating && ChannelBeingCreated.Name.Equals(channelname))
		{
			ChannelBeingCreated.Code = channelcode;
			ChannelBeingCreated.Status = ChannelStatus.Created;
			ChannelBeingCreated.CreatedByApi = true;
			AvailableChannels.Add(channelcode,ChannelBeingCreated);
			ChannelBeingCreated = null;
			return AvailableChannels[channelcode];
		}

		return null;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channelname"></param>
	/// <returns></returns>
	public Channel GetChannelByNameOrCode(string channelnameorcode)
	{
		if (AvailableChannels.TryGetValue(channelnameorcode, out Channel channel))
			return channel;

		channel = AvailableChannels.Values.FirstOrDefault(ch => ch.Name.Equals(channelnameorcode, System.StringComparison.InvariantCultureIgnoreCase));
		
		return channel ?? throw new System.Exception($"No channel found: {channelnameorcode}");
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="newState"></param>
	/// <returns></returns>
	public Channel ChangeChannelState(string channelnameorcode, ChannelStatus newState)
	{
		Channel channel = GetChannelByNameOrCode(channelnameorcode);

		if (AvailableChannels.Values.Any(ch => ch.Name.Equals(channel.Name) && (channel.Type == ChannelType.Public || ch.Code == channel.Code)))
		{
			var tChannel = AvailableChannels.Values.First(ch => ch.Name.Equals(channel.Name) && (channel.Type == ChannelType.Public || ch.Code == channel.Code));
			tChannel.Status = newState;
			return tChannel;
		}

		return null;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="status"></param>
	/// <returns></returns>
	public IEnumerable<KeyValuePair<string,Channel>> GetCombinedChannelList(ChannelStatus status = ChannelStatus.AllValid)
	{
		if (status == ChannelStatus.AllValid)
		{
			return AvailableChannels.Where(ch =>  ch.Value.Status == ChannelStatus.Available || 
												  ch.Value.Status == ChannelStatus.Invited   || 
												  ch.Value.Status == ChannelStatus.Joined    || 
												  ch.Value.Status == ChannelStatus.Kicked    || 
												  ch.Value.Status == ChannelStatus.Left      || 
												  ch.Value.Status == ChannelStatus.Pending   ||
												  ch.Value.Status == ChannelStatus.Created);
		}
		else if (status == ChannelStatus.All)
		{ 
			return AvailableChannels;
		}
		else
		{
			return AvailableChannels.Where(ch => ch.Value.Status == status);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public IEnumerable<KeyValuePair<string,Channel>> GetChannelList(ChannelType type)
	{
		if (type == ChannelType.All)
			return GetCombinedChannelList(ChannelStatus.All);

		return type == ChannelType.Private ? AvailablePrivateChannels : AvailablePublicChannels;
	}

	public IEnumerable<KeyValuePair<string,Channel>> GetChannelList(ChannelStatus status = ChannelStatus.AllValid)
	{
		if (status == ChannelStatus.All)
			return AvailableChannels;
		else if (status == ChannelStatus.AllValid)
		{
			return AvailableChannels.Where(ch => 
				ch.Value.Status.Equals(ChannelStatus.Available) ||
				ch.Value.Status.Equals(ChannelStatus.Created)   ||
				ch.Value.Status.Equals(ChannelStatus.Joined)    ||
				ch.Value.Status.Equals(ChannelStatus.Left)      ||
				ch.Value.Status.Equals(ChannelStatus.Kicked)    ||
				ch.Value.Status.Equals(ChannelStatus.Invited));
		}
		else
			return AvailablePrivateChannels.Concat(AvailablePublicChannels).ToList().Where(ch => ch.Value.Status.Equals(status));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="newList"></param>
	/// <param name="type"></param>
	public void RefreshAvailableChannels(List<Channel> newList, ChannelType type)
	{

		if (type == ChannelType.Private)
		{
			foreach(string code in AvailablePrivateChannels.Select(ch=>ch.Value.Code))
				AvailableChannels.Remove(code);

			AvailableChannels = AvailableChannels.Concat(newList.ToDictionary(li => li.Code, li => li)).ToDictionary();
		}
		else if (type == ChannelType.Public)
		{
			foreach(string code in AvailablePrivateChannels.Select(ch=>ch.Value.Code))
				AvailableChannels.Remove(code);

			AvailableChannels = AvailableChannels.Concat(newList.ToDictionary(li => li.Code, li => li)).ToDictionary();
		}
	}
}
