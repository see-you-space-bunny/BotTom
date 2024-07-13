using System.Collections.Generic;
using System.Linq;
using FChatApi.Objects;
using FChatApi.Enums;
using System.Threading.Tasks;
using System;
using FChatApi.Core;
using System.Collections.Concurrent;

namespace FChatApi.Systems;

public class ChannelTracker
{
	private readonly ConcurrentDictionary<string,Channel>[] Channels;

	/// <summary>all known channels</summary>
	internal ConcurrentDictionary<string,Channel> All => Channels[(int)ChannelType.All];

	/// <summary>all joined channels</summary>
	internal ConcurrentDictionary<string,Channel> Joined => Channels[0];

	private Channel ChannelBeingCreated { get; set; }

	internal bool IsChannelBeingCreated => ChannelBeingCreated is not null;

	/// <summary>
	/// 
	/// </summary>
	internal ChannelTracker()
	{
		Channels = new ConcurrentDictionary<string,Channel>[5];
		Channels[0]							= new ConcurrentDictionary<string,Channel>(StringComparer.InvariantCultureIgnoreCase);
		Channels[(int)ChannelType.All]		= new ConcurrentDictionary<string,Channel>(StringComparer.InvariantCultureIgnoreCase);
		Channels[(int)ChannelType.Public]	= new ConcurrentDictionary<string,Channel>(StringComparer.InvariantCultureIgnoreCase);
		Channels[(int)ChannelType.Private]	= new ConcurrentDictionary<string,Channel>(StringComparer.InvariantCultureIgnoreCase);
		Channels[(int)ChannelType.Hidden]	= new ConcurrentDictionary<string,Channel>(StringComparer.InvariantCultureIgnoreCase);
	}

	public Channel AddManually(string channelname, UserRelationshipWithChannel status, string channelcode)
	{
		if (All.ContainsKey(channelname) && !All.Values.Any(ch => ch.Name.Equals(channelname)))
		{
			Channel ch = new (channelname, channelcode, ChannelType.Private)
			{
				Status = status
			};
			All.AddOrUpdate(channelcode,(key) => ch, (key,value) => ch);
			return ch;
		}

		return null;
	}

#region (~) StartCreation
	/// <summary>
	/// begins the creation of a channel<br/>it must be later finalized with <c>FinalizeChannelCreation</c>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="status"></param>
	internal void StartChannelCreation(string name)
	{
		if (ChannelBeingCreated is null)
		{
			Channel channel = new()
			{
				Name		= name,
				Code		= string.Empty,
				Status		= UserRelationshipWithChannel.Creating,
				Type		= ChannelType.Private,
				AdEnabled	= false,
			};
			ChannelBeingCreated = channel;
		}
		else
		{
			throw new InvalidOperationException($"Cannot start creating channel \"{name}\" while another channel (\"{ChannelBeingCreated.Name}\") is still pending finalization.");
		}
	}
#endregion

#region (~) FinalizeCreation
	/// <summary>
	/// finalize creating a channel<br/>must be called after <c>StartChannelCreation</c>
	/// </summary>
	/// <param name="name">name of the channel we wish to create</param>
	/// <param name="code">channel code we recieved after requesting channel creation</param>
	/// <param name="owner">the owner of the channel (us)</param>
	/// <returns>the channel we just created</returns>
	internal Channel FinalizeChannelCreation(string name, string code, User owner = null)
	{
		owner ??= ApiConnection.ApiUser;
		if (ChannelBeingCreated is null)
			throw new NullReferenceException($"Tried to create  \"{name}\" (code: {code}, owner: {owner.Name}), ChannelBeingCreated was null. Most likely caused by not calling StartChannelCreation first.");

		if (ChannelBeingCreated.Status == UserRelationshipWithChannel.Creating && ChannelBeingCreated.Name.Equals(name))
		{
			ChannelBeingCreated.Code			= code;
			ChannelBeingCreated.Status			= UserRelationshipWithChannel.Created;
			ChannelBeingCreated.CreatedByApi	= true;
			ChannelBeingCreated.Owner			= owner;

			All.AddOrUpdate(code,(key) => ChannelBeingCreated,(key,value) => ChannelBeingCreated);
			Joined.AddOrUpdate(code,(key) => ChannelBeingCreated,(key,value) => ChannelBeingCreated);

			ChannelBeingCreated					= null;
			return All[code.ToLowerInvariant()];
		}
		throw new InvalidOperationException($"Could not create \"{name}\" (code: {code}, owner: {owner.Name}). No channel matching that name was pending creation.");
	}
#endregion

#region (~) ChangeChStatus
	/// <summary>
	/// changes the status of a channel
	/// </summary>
	/// <param name="channelnameorcode">channel name or code</param>
	/// <param name="value">the channel's new status</param>
	/// <returns>the updated channel</returns>
	internal Channel Channel_ChangeStatus(string channelnameorcode, UserRelationshipWithChannel value) =>
        Channel_ChangeStatus(SingleByNameOrCode(channelnameorcode), value);

    /// <summary>
    /// changes the status of a channel
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="value">the channel's new status</param>
    /// <returns>the updated channel</returns>
    internal Channel Channel_ChangeStatus(Channel channel, UserRelationshipWithChannel value)
    {
		channel.Status = value;
		return channel;
	}
#endregion

#region (+) TryNameOrCode
	/// <summary>
	/// retrieves a channel, first attempting to find it by its channel-code and then by name
	/// </summary>
	/// <param name="channelcode">the name or channel-code of the channel we seek to find</param>
	/// <returns>the channel that matches our <c>value</c></returns>
	public bool TrySingleByNameOrCode(string channelcode, out Channel channel)
	{
		if (!Joined.TryGetValue(channelcode, out channel))
			if (!All.TryGetValue(channelcode, out channel))
				channel = All.Values.FirstOrDefault(ch => ch.Name.Equals(channelcode, StringComparison.InvariantCultureIgnoreCase));
		return channel is not null;
	}
#endregion

#region (+) NameOrCode
	/// <summary>
	/// retrieves a channel, first attempting to find it by its channel-code and then by name
	/// </summary>
	/// <param name="channelcode">the name or channel-code of the channel we seek to find</param>
	/// <returns>the channel that matches our <c>value</c></returns>
	public Channel SingleByNameOrCode(string channelcode)
	{
		if (!Joined.TryGetValue(channelcode, out Channel channel))
			if (!All.TryGetValue(channelcode, out channel))
				channel = All.Values.FirstOrDefault(ch => ch.Name.Equals(channelcode, StringComparison.InvariantCultureIgnoreCase));
		return channel;
	}
#endregion

#region (~) GetList
	/// <summary>
	/// 
	/// </summary>
	/// <param name="channeltype"></param>
	/// <returns>gets a list of channels by type</returns>
	public IDictionary<string,Channel> GetList(ChannelType channeltype) =>
		Channels[(int)channeltype];

	public IDictionary<string,Channel> GetList(UserRelationshipWithChannel relationship = UserRelationshipWithChannel.AllValid) =>
		relationship switch {
			UserRelationshipWithChannel.All			=> All,
			UserRelationshipWithChannel.AllValid	=> All.Where(ch => ch.Value.Status >= UserRelationshipWithChannel.AllValid).ToDictionary(),
			_ 										=> All.Where(ch => ch.Value.Status.Equals(relationship)).ToDictionary(),
		};
#endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="channels">the list of channels we need to update</param>
    /// <param name="channeltype">the type of channel that's being put into the list</param>
    internal void RefreshAvailableChannels(List<Channel> channels, ChannelType channeltype)
    {
        if (!(channeltype > ChannelType.All))
			throw new ArgumentException($"Attempted to refresh channels of an invalid type: {channeltype}",nameof(channeltype));
		
		Channels[(int)channeltype] = new ConcurrentDictionary<string,Channel>(channels.ToDictionary(c => c.Code, c => c));
		
		Channels[(int)ChannelType.All ] = new ConcurrentDictionary<string,Channel>(All
			.Where(kv => !Channels[(int)channeltype].ContainsKey(kv.Key))
			.Concat(Channels[(int)channeltype]));
    }
}
