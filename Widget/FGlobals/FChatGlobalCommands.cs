using System.Text;
using System.ComponentModel;
using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Attributes;
using Engine.ModuleHost;
using Engine.ModuleHost.Plugins;
using Engine.ModuleHost.CommandHandling;
using Widget.FGlobals.Enums;
using FChatApi.Enums;

namespace Widget.FGlobals;

public partial class FChatGlobalCommands : FChatPlugin
{


#if DEBUG
	internal FChatMessageBuilder MostRecentMessage = null!; 
#endif

	public FChatGlobalCommands(TimeSpan? updateInterval = null) : this(null!,updateInterval)
	{ }

	public FChatGlobalCommands(ApiConnection api,TimeSpan? updateInterval = null) : base(api,updateInterval)
	{
		ModuleType = BotModule.System;
	}

	private static void PreProcessEnumAttributes()
	{
		AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(GlobalCommand));

		AttributeEnumExtensions.ProcessEnumForAttribute<MinimumPrivilegeAttribute>(typeof(GlobalCommand));

		AttributeEnumExtensions.ProcessEnumForAttribute<SuccessResponseAttribute>(typeof(GlobalCommand));
		AttributeEnumExtensions.ProcessEnumForAttribute<FailureResponseAttribute>(typeof(GlobalCommand));
		AttributeEnumExtensions.ProcessEnumForAttribute<AccessDeniedResponseAttribute>(typeof(GlobalCommand));
	}

	public override void HandleRecievedMessage(BotCommand command)
	{
		FChatMessageBuilder messageBuilder = new FChatMessageBuilder()
			.WithAuthor(ApiConnection.CharacterName)
			.WithRecipient(command.UserName)
			.WithChannel(command.Channel);

		////// DO STUFF HERE
		
		if (Enum.TryParse(command.Command,true,out GlobalCommand botCommand))
		{
			var commandPrivilege = AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,MinimumPrivilegeAttribute>(botCommand).Privilege;
			var tempUser = command.UserName.ToRegisteredUser();
			switch (botCommand)
			{
#region Register
				case GlobalCommand.Register:
					if (!(command.User! ?? new RegisteredUser()).IsLinked)
					{
						if (GlobalOperators.TryGetValue(command.UserName, out Privilege globalPrivilege))
						{
							tempUser.PrivilegeLevel = globalPrivilege;
						}
						else if (Operators.TryGetValue(command.UserName, out Privilege opPrivilege))
						{
							tempUser.PrivilegeLevel = opPrivilege;
						}
						else
						{
							tempUser.PrivilegeLevel = Privilege.RegisteredUser;
						}
						tempUser.WhenRegistered = DateTime.Now;
						if (ChatBot.Users.TryAdd(command.UserName.ToLower(),tempUser))
						{
							messageBuilder
								.WithMessage(
									!string.IsNullOrWhiteSpace(botCommand.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>().Message)
									?	botCommand.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>().Message
									:	FailureResponseAttribute.Generic);
						}
						else
						{
							messageBuilder
								.WithMessage(
									!string.IsNullOrWhiteSpace(GlobalCommand.Whoami.GetEnumAttribute<GlobalCommand,FailureResponseAttribute>().Message)
									?	GlobalCommand.Whoami.GetEnumAttribute<GlobalCommand,FailureResponseAttribute>().Message
									:	FailureResponseAttribute.Generic);
						}
					}
					else
					{
						messageBuilder
							.WithMessage(
								!string.IsNullOrWhiteSpace(botCommand.GetEnumAttribute<GlobalCommand,AccessDeniedResponseAttribute>().Message)
								?	botCommand.GetEnumAttribute<GlobalCommand,AccessDeniedResponseAttribute>().Message
								:	AccessDeniedResponseAttribute.Generic);
					}
					break;
#endregion

#region UnRegister
				case GlobalCommand.UnRegister:
					if (command.PrivilegeLevel >= commandPrivilege && (command.User! ?? new RegisteredUser()).IsLinked)
					{
						ChatBot.Users.Remove(command.UserName.ToLower());
						messageBuilder
							.WithMessage(
								!string.IsNullOrWhiteSpace(botCommand.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>().Message)
								?	botCommand.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>().Message
								:	FailureResponseAttribute.Generic);
					}
					else
					{
						messageBuilder
							.WithMessage(
								!string.IsNullOrWhiteSpace(botCommand.GetEnumAttribute<GlobalCommand,AccessDeniedResponseAttribute>().Message)
								?	botCommand.GetEnumAttribute<GlobalCommand,AccessDeniedResponseAttribute>().Message
								:	AccessDeniedResponseAttribute.Generic);
					}
					break;
#endregion

#region Whoami
				case GlobalCommand.Whoami:
					if (command.PrivilegeLevel >= commandPrivilege && (command.User! ?? new RegisteredUser()).IsLinked)
					{
						var sb = new StringBuilder();
						sb.Append(!string.IsNullOrWhiteSpace(botCommand.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>().Message)
								?	botCommand.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>().Message
								:	FailureResponseAttribute.Generic);
						sb.Append($"You are {command.User!.Mention.NameAndNickname.Basic}. ");
						sb.Append($"You are a {command.PrivilegeLevel}. ");
						sb.Append($"You are registered since {command.User.WhenRegistered.ToShortDateString()}.");
						messageBuilder.WithMessage(sb.ToString());
					}
					else
					{
						messageBuilder
							.WithMessage(
								!string.IsNullOrWhiteSpace(GlobalCommand.Whoami.GetEnumAttribute<GlobalCommand,FailureResponseAttribute>().Message)
								?	GlobalCommand.Whoami.GetEnumAttribute<GlobalCommand,FailureResponseAttribute>().Message
								:	FailureResponseAttribute.Generic);
					}
					break;
#endregion

#region Shutdown
				case GlobalCommand.Shutdown:
					if (command.PrivilegeLevel >= commandPrivilege && (command.User! ?? new RegisteredUser()).IsLinked)
					{
						ChatBot.SetShutdownFlag();
						messageBuilder
							.WithMessage(
								!string.IsNullOrWhiteSpace(botCommand.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>().Message)
								?	botCommand.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>().Message
								:	FailureResponseAttribute.Generic);
					}
					else
					{
						messageBuilder
							.WithMessage(
								!string.IsNullOrWhiteSpace(botCommand.GetEnumAttribute<GlobalCommand,AccessDeniedResponseAttribute>().Message)
								?	botCommand.GetEnumAttribute<GlobalCommand,AccessDeniedResponseAttribute>().Message
								:	AccessDeniedResponseAttribute.Generic);
					}
					break;
#endregion

				default:
					break;
			}
		}

		////////////////////

#if DEBUG
		MostRecentMessage = messageBuilder;
#endif
		FChatApi.EnqueueMessage(messageBuilder);
	}

	public override async Task Update()
	{
		Task t = base.Update();
		//tempUser.Update(command.User);
		await t;
	}

	public override void HandleJoinedChannel(Channel channel)
	{
		ActiveChannels.TryAdd(channel.Code,channel);
	}

	static FChatGlobalCommands()
	{
		PreProcessEnumAttributes();
	}
}