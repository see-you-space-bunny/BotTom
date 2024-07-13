using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Objects;
using FChatApi.EventArguments;
using FChatApi.Tokenizer;
using FChatApi.Objects;
using Engine.ModuleHost.Enums;

namespace BotTom;

public partial class Program
{
	/// <summary>
	/// Takes care of pushing messages to the bot
	/// </summary>
	/// <param name="sender">our sending object</param>
	/// <param name="e">our event args</param>
	static void HandleMessageReceived(object sender, FChatMessage @event)
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
			ApiConnection.Mod_SetChannelUserStatus(@event.Channel,ApiConnection.Users.SingleByName(FCHAT_OWNER),UserRoomStatus.Moderator);
		}
#endif
	}
	
	static void HandleCreatedChannel(object sender, ChannelEventArgs @event)
	{
		ApiConnection.Mod_SetChannelUserStatus(@event.Channel,ApiConnection.Users.SingleByName(FCHAT_OWNER),UserRoomStatus.Invited);
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
		var privateChannels = ApiConnection.Channels.GetList(ChannelType.Private);

		// check and join starting channel here
		if (privateChannels.Values.Any(ch => ch.Code.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)))
		{
			await ApiConnection.User_JoinChannel(ApiConnection.Channels.SingleByNameOrCode(F_StartingChannel));
		}
		else if (privateChannels.Values.Any(ch => ch.Name.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)))
		{
			await ApiConnection.User_JoinChannel(privateChannels.Values.First(ch => ch.Name.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)));
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