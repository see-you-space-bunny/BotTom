using System.Collections.Generic;
using System.Linq;
using FChatApi.Objects;
using FChatApi.Enums;
using System.Threading.Tasks;
using System;

namespace FChatApi.Systems;

#pragma warning disable CA1822 // Mark members as static
internal class ChannelTracker
{

	/// <summary>all known channels</summary>
	internal Dictionary<string,Channel> AllAvailableChannels;

	/// <summary>
	/// 
	/// </summary>
	internal Dictionary<string,Channel> AvailablePublicChannels => AllAvailableChannels.Where((ch)=>ch.Value.Type == ChannelType.Public).ToDictionary();

	/// <summary>
	/// 
	/// </summary>
	internal Dictionary<string,Channel> AvailablePrivateChannels => AllAvailableChannels.Where((ch)=>ch.Value.Type == ChannelType.Private).ToDictionary();

	/// <summary>
	/// 
	/// </summary>
	internal Dictionary<string,Channel> JoinedChannels;

	private Channel ChannelBeingCreated { get; set; }

	internal bool IsChannelBeingCreated => ChannelBeingCreated is not null;

	/// <summary>
	/// 
	/// </summary>
	internal ChannelTracker()
	{
		AllAvailableChannels	= [];
		JoinedChannels			= [];
	}

	internal Channel AddChannelManually(string channelname, ChannelStatus status, string channelcode)
	{
		if (AllAvailableChannels.ContainsKey(channelname.ToLowerInvariant()) && !AllAvailableChannels.Values.Any(ch => ch.Name.Equals(channelname)))
		{
			Channel ch = new (channelname, channelcode, ChannelType.Private)
			{
				Status = status
			};
			AllAvailableChannels.Add(channelcode.ToLowerInvariant(),ch);
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
	internal void StartChannelCreation(string name, ChannelStatus status = ChannelStatus.Creating)
	{
		if (ChannelBeingCreated is null)
		{
			Channel toAdd = new(name, string.Empty, ChannelType.Private)
			{
				Status = status
			};
			ChannelBeingCreated = toAdd;
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
	internal Channel FinalizeChannelCreation(string name, string code, User owner)
	{
		if (ChannelBeingCreated is null)
			throw new NullReferenceException($"Tried to create  \"{name}\" (code: {code}, owner: {owner.Name}), ChannelBeingCreated was null. Most likely caused by not calling StartChannelCreation first.");

		if (ChannelBeingCreated.Status == ChannelStatus.Creating && ChannelBeingCreated.Name.Equals(name))
		{
			ChannelBeingCreated.Code			= code;
			ChannelBeingCreated.Status			= ChannelStatus.Created;
			ChannelBeingCreated.CreatedByApi	= true;
			ChannelBeingCreated.Owner			= owner;

			AllAvailableChannels.Add(code.ToLowerInvariant(),ChannelBeingCreated);
			JoinedChannels.Add(code.ToLowerInvariant(),ChannelBeingCreated);

			ChannelBeingCreated					= null;
			return AllAvailableChannels[code.ToLowerInvariant()];
		}
		throw new InvalidOperationException($"Could not create \"{name}\" (code: {code}, owner: {owner.Name}). No channel matching that name was pending creation.");
	}
#endregion

#region (~) LeaveChannel

	internal void LeaveChannel(Channel value) =>
        JoinedChannels.Remove(value.Code);

	internal void LeaveChannel(string channelnameorcode) =>
		LeaveChannel(GetChannelByNameOrCode(channelnameorcode).Code);
#endregion

#region (~) ChangeChStatus
	/// <summary>
	/// changes the status of a channel
	/// </summary>
	/// <param name="channelnameorcode">channel name or code</param>
	/// <param name="value">the channel's new status</param>
	/// <returns>the updated channel</returns>
	internal Channel ChangeChannelStatus(string channelnameorcode, ChannelStatus value) =>
        ChangeChannelStatus(GetChannelByNameOrCode(channelnameorcode), value);

    /// <summary>
    /// changes the status of a channel
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="value">the channel's new status</param>
    /// <returns>the updated channel</returns>
    internal Channel ChangeChannelStatus(Channel channel, ChannelStatus value)
    {
		channel.Status = value;
		return channel;
	}
#endregion

#region (~) ChByNameOrCode
	/// <summary>
	/// retrieves a channel, first attempting to find it by its channel-code and then by name
	/// </summary>
	/// <param name="value">the name or channel-code of the channel we seek to find</param>
	/// <returns>the channel that matches our <c>value</c></returns>
	internal Channel GetChannelByNameOrCode(string value)
	{
		if (!JoinedChannels.TryGetValue(value.ToLowerInvariant(), out Channel channel))
			if (!AllAvailableChannels.TryGetValue(value.ToLowerInvariant(), out channel))
				channel = AllAvailableChannels.Values.FirstOrDefault(ch => ch.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase));
		
		return channel;
	}
#endregion

#region (~) CombinedChList
	/// <summary>
	/// retrieves a combined list of all currently channels<br/>may optionally be filtered to a specific <c>ChannelStatus</c>
	/// </summary>
	/// <param name="status">by what <c>ChannelStatus</c> we filter the return value</param>
	/// <returns>a range of channels filtered to our argument</returns>
	internal Dictionary<string,Channel> GetCombinedChannelList(ChannelStatus status = ChannelStatus.AllValid)
	{
		if (status == ChannelStatus.AllValid)
		{
			return AllAvailableChannels.Where(ch => ch.Value.Status > ChannelStatus.AllValid).ToDictionary();
		}
		else if (status == ChannelStatus.All)
		{ 
			return AllAvailableChannels;
		}
		else if (status > ChannelStatus.Invalid)
		{
			return AllAvailableChannels.Where(ch => ch.Value.Status == status).ToDictionary();
		}
		throw new ArgumentException("Attempted to filter channel list by invalid status.",nameof(status));
	}
#endregion

#region (~) GetChList
	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <returns>gets a list of channels by type</returns>
	internal IDictionary<string,Channel> GetChannelList(ChannelType value) =>
		value switch {
			ChannelType.All		=> GetCombinedChannelList(ChannelStatus.All),
			ChannelType.Private	=> AvailablePrivateChannels,
			_ 					=> AvailablePublicChannels,
		};

	internal IDictionary<string,Channel> GetChannelList(ChannelStatus value = ChannelStatus.AllValid) =>
		value switch {
			ChannelStatus.All		=> AllAvailableChannels,
			ChannelStatus.AllValid	=> AllAvailableChannels.Where(ch => ch.Value.Status >= ChannelStatus.AllValid).ToDictionary(),
			_ 						=> AllAvailableChannels.Where(ch => ch.Value.Status.Equals(value)).ToDictionary(),
		};
#endregion


    /// <summary>
    /// 
    /// </summary>
    /// <param name="incomingChannels">the list of channels we need to update</param>
    /// <param name="value">the type of channel that's being put into the list</param>
    internal void RefreshAvailableChannels(List<Channel> incomingChannels, ChannelType value)
    {
        if (!(value > ChannelType.Invalid))
			throw new ArgumentException($"Attempted to refresh channels of an invalid type: {value}",nameof(value));

		AllAvailableChannels = AllAvailableChannels.Where(kv => kv.Value.Type != value).ToDictionary();
		AllAvailableChannels = AllAvailableChannels.Concat(incomingChannels.ToDictionary(li => li.Code, li => li)).ToDictionary();
    }
}
#pragma warning restore CA1822 // Mark members as static
