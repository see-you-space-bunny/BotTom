using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using FChatApi.Objects;
using FChatApi.Enums;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;

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
	IDictionary<string,Channel> AvailablePublicChannels => AvailableChannels.Where((ch)=>ch.Value.Type == ChannelType.Public).ToDictionary();

	/// <summary>
	/// 
	/// </summary>
	IDictionary<string,Channel> AvailablePrivateChannels => AvailableChannels.Where((ch)=>ch.Value.Type == ChannelType.Private).ToDictionary();

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
		AvailableChannels	= [];
		WatchChannels		= [];
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
	/// begins the creation of a channel<br/>it must be later finalized with <c>FinalizeChannelCreation</c>
	/// </summary>
	/// <param name="name"></param>
	/// <param name="status"></param>
	public void StartChannelCreation(string name, ChannelStatus status = ChannelStatus.Creating)
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

	/// <summary>
	/// finalize creating a channel<br/>must be called after <c>StartChannelCreation</c>
	/// </summary>
	/// <param name="name">name of the channel we wish to create</param>
	/// <param name="code">channel code we recieved after requesting channel creation</param>
	/// <param name="owner">the owner of the channel (us)</param>
	/// <returns>the channel we just created</returns>
	public Channel FinalizeChannelCreation(string name, string code, User owner)
	{
		if (ChannelBeingCreated is null)
			throw new NullReferenceException($"Tried to create  \"{name}\" (code: {code}, owner: {owner.Name}), ChannelBeingCreated was null. Most likely caused by not calling StartChannelCreation first.");

		if (ChannelBeingCreated.Status == ChannelStatus.Creating && ChannelBeingCreated.Name.Equals(name))
		{
			ChannelBeingCreated.Code			= code;
			ChannelBeingCreated.Status			= ChannelStatus.Created;
			ChannelBeingCreated.CreatedByApi	= true;
			ChannelBeingCreated.Owner			= owner;

			AvailableChannels.Add(code,ChannelBeingCreated);

			ChannelBeingCreated					= null;
			return AvailableChannels[code];
		}
		throw new InvalidOperationException($"Could not create \"{name}\" (code: {code}, owner: {owner.Name}). No channel matching that name was pending creation.");
	}

	/// <summary>
	/// retrieves a channel, first attempting to find it by its channel-code and then by name
	/// </summary>
	/// <param name="value">the name or channel-code of the channel we seek to find</param>
	/// <returns>the channel that matches our <c>value</c></returns>
	public Channel GetChannelByNameOrCode(string value)
	{
		if (AvailableChannels.TryGetValue(value, out Channel channel))
			return channel;

		channel = AvailableChannels.Values.FirstOrDefault(ch => ch.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase));
		
		return channel ?? throw new ArgumentException($"No matching channel found for \"{value}\".",nameof(value));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channelnameorcode"></param>
	/// <param name="value"></param>
	/// <returns>the updated channel</returns>
	public Channel ChangeChannelStatus(string channelnameorcode, ChannelStatus value) =>
        ChangeChannelStatus(GetChannelByNameOrCode(channelnameorcode), value);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public static Channel ChangeChannelStatus(Channel channel, ChannelStatus value)
	{
		channel.Status = value;
		return channel;
	}

	/// <summary>
	/// retrieves a combined list of all currently channels<br/>may optionally be filtered to a specific <c>ChannelStatus</c>
	/// </summary>
	/// <param name="status">by what <c>ChannelStatus</c> we filter the return value</param>
	/// <returns>a range of channels filtered to our argument</returns>
	public Dictionary<string,Channel> GetCombinedChannelList(ChannelStatus status = ChannelStatus.AllValid)
	{
		if (status == ChannelStatus.AllValid)
		{
			return AvailableChannels.Where(ch => ch.Value.Status > ChannelStatus.AllValid).ToDictionary();
		}
		else if (status == ChannelStatus.All)
		{ 
			return AvailableChannels;
		}
		else if (status > ChannelStatus.Invalid)
		{
			return AvailableChannels.Where(ch => ch.Value.Status == status).ToDictionary();
		}
		throw new ArgumentException("Attempted to filter channel list by invalid status.",nameof(status));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="value"></param>
	/// <returns>gets a list of channels by type</returns>
	public IDictionary<string,Channel> GetChannelList(ChannelType value) =>
		value switch {
			ChannelType.All		=> GetCombinedChannelList(ChannelStatus.All),
			ChannelType.Private	=> AvailablePrivateChannels,
			_ 					=> AvailablePublicChannels,
		};

	public IDictionary<string,Channel> GetChannelList(ChannelStatus value = ChannelStatus.AllValid) =>
		value switch {
			ChannelStatus.All		=> AvailableChannels,
			ChannelStatus.AllValid	=> AvailableChannels.Where(ch => ch.Value.Status >= ChannelStatus.AllValid).ToDictionary(),
			_ 						=> AvailableChannels.Where(ch => ch.Value.Status.Equals(value)).ToDictionary(),
		};

    /// <summary>
    /// 
    /// </summary>
    /// <param name="incomingChannels">the list of channels we need to update</param>
    /// <param name="value">the type of channel that's being put into the list</param>
    public void RefreshAvailableChannels(List<Channel> incomingChannels, ChannelType value)
    {
        List<Task> tasks = [];
        switch (value)
        {
			case ChannelType.Private:
				foreach (string code in AvailablePrivateChannels.Select(ch => ch.Value.Code))
					tasks.Add(Task.Run(() => AvailableChannels.Remove(code)));

				AvailableChannels = AvailableChannels.Concat(incomingChannels.ToDictionary(li => li.Code, li => li)).ToDictionary();
				break;

			case ChannelType.Public:
				foreach (string code in AvailablePrivateChannels.Select(ch => ch.Value.Code))
					tasks.Add(Task.Run(() => AvailableChannels.Remove(code)));

				AvailableChannels = AvailableChannels.Concat(incomingChannels.ToDictionary(li => li.Code, li => li)).ToDictionary();
				break;
			
			default:
				throw new ArgumentException($"Attempted to refresh channels of an invalid type: {value}",nameof(value));
        }
        Task.WaitAll([.. tasks]);
    }
}
