using System.Text;
using System.ComponentModel;

using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Objects;
using Plugins.Tokenizer;
using FChatApi.Attributes;

using Plugins;
using Plugins.Interfaces;

using FGlobals.Enums;
using Plugins.Attributes;
using System.Text.RegularExpressions;
using Plugins.Core;

namespace FGlobals;

public partial class FChatGlobalCommands : FChatPlugin<GlobalCommand>, IFChatPlugin
{


#if DEBUG
	internal FChatMessageBuilder MostRecentMessage = null!;
#endif

	public FChatGlobalCommands(ApiConnection api, TimeSpan updateInterval) : base(api, updateInterval)
	{ }

	private static void PreProcessEnumAttributes()
	{
		AttributeExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(GlobalCommand));

		AttributeExtensions.ProcessEnumForAttribute<MinimumPrivilegeAttribute>(typeof(GlobalCommand));
	}

	public override void HandleRecievedMessage(CommandTokens commandTokens)
	{
//////////////
		
		if (!commandTokens.TryGetParameters(out GlobalCommand command))
			return;
		
		if (commandTokens.Source.Author.PrivilegeLevel < command.GetEnumAttribute<GlobalCommand, MinimumPrivilegeAttribute>().Privilege)
			return;
			
//////////////

		bool respondWithMessage = true;
		
		FChatMessageBuilder messageBuilder = new FChatMessageBuilder()
			.WithAuthor(ApiConnection.CharacterName)
			.WithRecipient(commandTokens.Source.Author.Name)
			.WithChannel(commandTokens.Source.Channel)
			.WithMessageType(commandTokens.Source.Channel is not null ? FChatMessageType.Basic : FChatMessageType.Whisper);

////////////// DO STUFF HERE
		
		switch (command)
		{
#region Register
			case GlobalCommand.Register:
				if (commandTokens.Source.Author is not null && !commandTokens.Source.Author.IsRegistered)
				{
					commandTokens.Source.Author.Register();
					if (GlobalOperators.TryGetValue(commandTokens.Source.Author.Name, out Privilege globalPrivilege))
					{
						commandTokens.Source.Author.PrivilegeLevel = globalPrivilege;
					}
					else if (Operators.TryGetValue(commandTokens.Source.Author.Name, out Privilege opPrivilege))
					{
						commandTokens.Source.Author.PrivilegeLevel = opPrivilege;
					}
				}
				break;
#endregion

#region UnRegister
			case GlobalCommand.UnRegister:
				break;
#endregion

#region Whoami
			case GlobalCommand.Whoami:
				break;
#endregion

#region ChInvite
			case GlobalCommand.ChInvite:
				ApiConnection.Mod_SetChannelUserStatus(
					ApiConnection.Channels.GetList(ChannelType.Private).Values
						.FirstOrDefault(c => c.CreatedByApi),
					ApiConnection.Users.SingleByName(commandTokens.Parameters["User"]),
					UserRoomStatus.Invited
				);
				respondWithMessage = false;
				break;
#endregion

#region ChCreate
			case GlobalCommand.ChCreate:
				ApiConnection.User_CreateChannel(commandTokens.Parameters["ChannelName"]);
				respondWithMessage = false;
				break;
#endregion

#region Shutdown
			case GlobalCommand.Shutdown:
				ApiConnection.SetShutdownFlag();
				ApiConnection.SerializeUsers();
				break;
#endregion

			default:
				break;
		}


////////////////////////////

		if (respondWithMessage)
		{
#if DEBUG
			MostRecentMessage = messageBuilder;
#endif
			FChatApi.EnqueueMessage(messageBuilder);
		}
	}

	public override void Update()
	{
		base.Update();
	}

	public override void HandleJoinedChannel(ChannelEventArgs @event)
	{
		ActiveChannels.TryAdd(@event.Channel.Code, @event.Channel);
	}

	static FChatGlobalCommands()
	{
		PreProcessEnumAttributes();
	}
}