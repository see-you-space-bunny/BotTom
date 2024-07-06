using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Runtime.CompilerServices;
using System.Reactive.Concurrency;
using Discord;
using FChatApi.Core;
using FChatApi.Objects;
using Engine.ModuleHost;
using Engine.ModuleHost.CardiApi;
using Engine.ModuleHost.Enums;
using Engine.ModuleHost.Attributes;
using Engine.ModuleHost.Plugins;
using Engine.ModuleHost.CommandHandling;
using Widget.FGlobals.Enums;

namespace Widget.FGlobals;

public partial class FChatGlobalCommands : FChatPlugin
{


#if DEBUG
	internal ChatMessageBuilder MostRecentMessage = null!; 
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
		ChatMessageBuilder messageBuilder = new ChatMessageBuilder()
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
						if (ChatBot.RegisteredUsers.TryAdd(command.UserName.ToLower(),tempUser))
						{
							messageBuilder.WithMessage(AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>(botCommand).Message);
						}
						else
						{
							messageBuilder.WithMessage(AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,FailureResponseAttribute>(botCommand).Message);
						}
					}
					else
					{
						messageBuilder.WithMessage(AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,AccessDeniedResponseAttribute>(botCommand).Message);
					}
					break;

				case GlobalCommand.UnRegister:
					if (command.PrivilegeLevel >= commandPrivilege && (command.User! ?? new RegisteredUser()).IsLinked)
					{
						ChatBot.RegisteredUsers.Remove(command.UserName.ToLower());
						messageBuilder.WithMessage(AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>(botCommand).Message);
					}
					else
					{
						messageBuilder.WithMessage(AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,AccessDeniedResponseAttribute>(botCommand).Message);
					}
					break;

				case GlobalCommand.Whoami:
					if (command.PrivilegeLevel >= commandPrivilege && (command.User! ?? new RegisteredUser()).IsLinked)
					{
						var sb = new StringBuilder();
						sb.Append(AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>(botCommand).Message);
						sb.Append($"You are {command.User!.Mention.NameAndNickname.Basic}. ");
						sb.Append($"You are a {command.PrivilegeLevel}. ");
						sb.Append($"You are registered since {command.User.WhenRegistered.ToShortDateString()}.");
						messageBuilder.WithMessage(sb.ToString());
					}
					else
					{
						messageBuilder.WithMessage(AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,FailureResponseAttribute>(botCommand).Message);
					}
					break;

				case GlobalCommand.Shutdown:
					if (command.PrivilegeLevel >= commandPrivilege && (command.User! ?? new RegisteredUser()).IsLinked)
					{
						ChatBot.SetShutdownFlag();
						messageBuilder.WithMessage(AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,SuccessResponseAttribute>(botCommand).Message);
					}
					else
					{
						messageBuilder.WithMessage(AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,AccessDeniedResponseAttribute>(botCommand).Message);
					}
					break;

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