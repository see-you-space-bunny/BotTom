using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.EventArguments;
using FChatApi.Tokenizer;
using Engine.ModuleHost.Enums;

namespace BotTom;

public partial class Program
{
	/// <summary>
	/// Takes care of pushing messages to the bot
	/// </summary>
	/// <param name="sender">our sending object</param>
	/// <param name="e">our event args</param>
	static void HandleMessageReceived(object sender, MessageEventArgs @event)
	{
		if (F_CommandParser.TryConvertCommand(@event,out BotCommand command))
		{
			F_Bot!.HandleMessage(command);
		}
	}
	
	static void ConnectedToChat(object sender, ChannelEventArgs @event)
	{
#if DEBUG
		ApiConnection.User_CreateChannel("Tom's Test Kitchen");
#endif
	}

	/// <summary>
	/// We've joined a channel
	/// </summary>
	/// <param name="sender">our sending object</param>
	/// <param name="event">our event args</param>
	static void HandleJoinedChannel(object sender, ChannelEventArgs @event)
	{
		if (@event.User.Name.Equals(F_CharacterName))
		{
			F_Bot!.HandleJoinedChannel(@event);
		}
#if DEBUG
		else if (@event.User.Name.Equals(FCHAT_OWNER,StringComparison.InvariantCultureIgnoreCase))
		{
			ApiConnection.Mod_ChannelPromoteUser(@event.Channel,FCHAT_OWNER);
		}
#endif
	}
	
	static void HandleCreatedChannel(object sender, ChannelEventArgs @event)
	{
		ApiConnection.Mod_ChannelInviteUser(@event.Channel,FCHAT_OWNER);
#if DEBUG
		ApiConnection.User_SetStatus(ChatStatus.DND,$"[session={@event.Channel.Name}]{@event.Channel.Code}[/session]");
#endif
	}

	/// <summary>
	/// We've left a channel
	/// </summary>
	/// <param name="sender">our sending object</param>
	/// <param name="event">our event args</param>
	static void HandleLeftChannel(object sender, ChannelEventArgs @event)
	{

	}

	/// <summary>
	/// We got a private list of channels
	/// </summary>
	/// <param name="sender">our sending object</param>
	/// <param name="event">our event args</param>
	static async void HandlePrivateChannelsReceived(object sender, ChannelEventArgs @event)
	{
		var privateChannels = ApiConnection.GetChannelListByType(ChannelType.Private);

		// check and join starting channel here
		if (privateChannels.Any(x => x.Code.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)))
		{
			await ApiConnection.User_JoinChannel(F_StartingChannel);
		}
		else if (privateChannels.Any(x => x.Name.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)))
		{
			await ApiConnection.User_JoinChannel(privateChannels.First(x => x.Name.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)).Code);
		}

#if DEBUG
		// string roomname = "Aelia's Secret Testing Ground";
		// if (!F_Chat.RequestChannelList(ChannelType.Private).Any(x => x.Code.Equals("adh-1a7c52c105ef5420b73b", StringComparison.InvariantCultureIgnoreCase)))
		// {
		//     F_Chat.CreateChannel(roomname);
		// }
		// else
		// {
		//     F_Chat.JoinChannel(roomname);
		// }
#endif
	}

	/// <summary>
	/// We got a public list of channels
	/// </summary>
	/// <param name="sender">our sending object</param>
	/// <param name="event">our event args</param>
	static void HandlePublicChannelsReceived(object sender, ChannelEventArgs @event)
	{
	}
}