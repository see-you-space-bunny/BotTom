using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text;
using ModuleHost;
using ModuleHost.CommandHandling;
using ChatApi.Objects;
using ChatApi.Core;
using Discord;
using ModuleHost.CardiApi;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Runtime.CompilerServices;
using System.Reactive.Concurrency;
using ModuleHost.Attributes;
using System.ComponentModel;
using Widget.FGlobals.Enums;
using ModuleHost.Enums;

namespace Widget.CardGame;

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

        AttributeEnumExtensions.ProcessEnumForAttribute<DefaultResponseAttribute>(typeof(GlobalCommand));
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
            var defaultResponse = AttributeEnumExtensions.GetEnumAttribute<GlobalCommand,DefaultResponseAttribute>(botCommand);
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
                            messageBuilder.WithMessage(defaultResponse.Success);
                        }
                        else
                        {
                            messageBuilder.WithMessage(defaultResponse.Failure);
                        }
                    }
                    else
                    {
                        messageBuilder.WithMessage(defaultResponse.Denied);
                    }
                    break;

                case GlobalCommand.UnRegister:
                    if (command.PrivilegeLevel >= commandPrivilege && (command.User! ?? new RegisteredUser()).IsLinked)
                    {
                        ChatBot.RegisteredUsers.Remove(command.UserName.ToLower());
                        messageBuilder.WithMessage(defaultResponse.Success);
                    }
                    else
                    {
                        messageBuilder.WithMessage(defaultResponse.Denied);
                    }
                    break;

                case GlobalCommand.Whoami:
                case GlobalCommand.Me:
                    if (command.PrivilegeLevel >= commandPrivilege && (command.User! ?? new RegisteredUser()).IsLinked)
                    {
                        var sb = new StringBuilder();
                        sb.Append(defaultResponse.Success);
                        sb.Append($"You are {command.User!.Mention.NameAndNickname.Basic}. ");
                        sb.Append($"You are a {command.PrivilegeLevel}. ");
                        sb.Append($"You are registered since {command.User.WhenRegistered.ToShortDateString()}.");
                        messageBuilder.WithMessage(sb.ToString());
                    }
                    else
                    {
                        messageBuilder.WithMessage(defaultResponse.Denied);
                    }
                    break;

                case GlobalCommand.Shutdown:
                    if (command.PrivilegeLevel >= commandPrivilege && (command.User! ?? new RegisteredUser()).IsLinked)
                    {
                        ChatBot.SetShutdownFlag();
                        messageBuilder.WithMessage(defaultResponse.Success);
                    }
                    else
                    {
                        messageBuilder.WithMessage(defaultResponse.Denied);
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