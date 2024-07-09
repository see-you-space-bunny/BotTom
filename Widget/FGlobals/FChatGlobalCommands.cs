using System.Text;
using System.ComponentModel;

using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Objects;
using FChatApi.Tokenizer;
using FChatApi.Attributes;
using FChatApi.EventArguments;

using ModularPlugins;
using ModularPlugins.Interfaces;

using FGlobals.Enums;

namespace FGlobals;

public partial class FChatGlobalCommands : FChatPlugin, IFChatPlugin
{


#if DEBUG
    internal FChatMessageBuilder MostRecentMessage = null!;
#endif

    public FChatGlobalCommands(ApiConnection api, TimeSpan updateInterval) : base(api, updateInterval)
    { }

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
        if (!command.TryParseCommand(out GlobalCommand moduleCommand))
            return;

        bool respondWithMessage = true;
        
        FChatMessageBuilder messageBuilder = new FChatMessageBuilder()
            .WithAuthor(ApiConnection.CharacterName)
            .WithRecipient(command.User.Name)
            .WithChannel(command.Channel)
            .WithMessageType(command.Channel is not null ? FChatMessageType.Basic : FChatMessageType.Whisper);


        var requiredPrivilege = moduleCommand
            .GetEnumAttribute<GlobalCommand, MinimumPrivilegeAttribute>()
            .Privilege;

        ////////////// DO STUFF HERE
        
        switch (Enum.GetValues<GlobalCommand>().FirstOrDefault(e => e.GetHashCode().Equals(command.ModuleCommand)))
        {
            #region Register
            case GlobalCommand.Register:
                if (command.User is not null)
                {
                    if (GlobalOperators.TryGetValue(command.User.Name, out Privilege globalPrivilege))
                    {
                        command.User.PrivilegeLevel = globalPrivilege;
                    }
                    else if (Operators.TryGetValue(command.User.Name, out Privilege opPrivilege))
                    {
                        command.User.PrivilegeLevel = opPrivilege;
                    }
                    else
                    {
                        command.User.PrivilegeLevel = Privilege.RegisteredUser;
                    }
                    command.User.WhenRegistered = DateTime.Now;
                    // if (ChatBot.RegisteredUsers.TryAdd(command.User.Name.ToLower(), command.User))
                    // {
                    //     messageBuilder
                    //         .WithMessage(
                    //             !string.IsNullOrWhiteSpace(command.ModuleCommand.GetEnumAttribute<GlobalCommand, SuccessResponseAttribute>().Message)
                    //             ? command.ModuleCommand.GetEnumAttribute<GlobalCommand, SuccessResponseAttribute>().Message
                    //             : FailureResponseAttribute.Generic);
                    // }
                    // else
                    // {
                    //     messageBuilder
                    //         .WithMessage(
                    //             !string.IsNullOrWhiteSpace(GlobalCommand.Whoami.GetEnumAttribute<GlobalCommand, FailureResponseAttribute>().Message)
                    //             ? GlobalCommand.Whoami.GetEnumAttribute<GlobalCommand, FailureResponseAttribute>().Message
                    //             : FailureResponseAttribute.Generic);
                    // }
                }
                else
                {
                    messageBuilder
                        .WithMessage(
                            !string.IsNullOrWhiteSpace(moduleCommand.GetEnumAttribute<GlobalCommand, AccessDeniedResponseAttribute>().Message)
                            ? moduleCommand.GetEnumAttribute<GlobalCommand, AccessDeniedResponseAttribute>().Message
                            : AccessDeniedResponseAttribute.Generic);
                }
                break;
            #endregion

            #region UnRegister
            case GlobalCommand.UnRegister:
                if (command.User.PrivilegeLevel >= requiredPrivilege)
                {
                    //ChatBot.RegisteredUsers.Remove(command.User.Name.ToLower());
                    messageBuilder
                        .WithMessage(
                            !string.IsNullOrWhiteSpace(moduleCommand.GetEnumAttribute<GlobalCommand, SuccessResponseAttribute>().Message)
                            ? moduleCommand.GetEnumAttribute<GlobalCommand, SuccessResponseAttribute>().Message
                            : FailureResponseAttribute.Generic);
                }
                else
                {
                    messageBuilder
                        .WithMessage(
                            !string.IsNullOrWhiteSpace(moduleCommand.GetEnumAttribute<GlobalCommand, AccessDeniedResponseAttribute>().Message)
                            ? moduleCommand.GetEnumAttribute<GlobalCommand, AccessDeniedResponseAttribute>().Message
                            : AccessDeniedResponseAttribute.Generic);
                }
                break;
            #endregion

            #region Whoami
            case GlobalCommand.Whoami:
                if (command.User.PrivilegeLevel >= requiredPrivilege)
                {
                    var sb = new StringBuilder();
                    sb.Append(moduleCommand.GetEnumAttribute<GlobalCommand, SuccessResponseAttribute>().Message);
                    sb.Append($"You are {command.User!.Mention}. ");
                    sb.Append($"You are a {command.User.PrivilegeLevel}. ");
                    sb.Append($"You are registered since {command.User.WhenRegistered.ToShortDateString()}.");
                    messageBuilder.WithMessage(sb.ToString());
                }
                else
                {
                    messageBuilder
                        .WithMessage(
                            !string.IsNullOrWhiteSpace(GlobalCommand.Whoami.GetEnumAttribute<GlobalCommand, FailureResponseAttribute>().Message)
                            ? GlobalCommand.Whoami.GetEnumAttribute<GlobalCommand, FailureResponseAttribute>().Message
                            : FailureResponseAttribute.Generic);
                }
                break;
            #endregion

            #region ChInvite
            case GlobalCommand.ChInvite:
                ApiConnection.Mod_ChannelInviteUser(
                    ApiConnection.GetChannelListByType(ChannelType.Private)
                        .FirstOrDefault(c => c.CreatedByApi),
                    command.Parameters[0]);
                respondWithMessage = false;
                break;
            #endregion

            #region ChCreate
            case GlobalCommand.ChCreate:
                ApiConnection.User_CreateChannel(command.Parameters[0]);
                respondWithMessage = false;
                break;
            #endregion

            #region Shutdown
            case GlobalCommand.Shutdown:
                if (command.User.PrivilegeLevel >= requiredPrivilege)
                {
                    //ChatBot.SetShutdownFlag();
                    messageBuilder
                        .WithMessage(
                            !string.IsNullOrWhiteSpace(moduleCommand.GetEnumAttribute<GlobalCommand, SuccessResponseAttribute>().Message)
                            ? moduleCommand.GetEnumAttribute<GlobalCommand, SuccessResponseAttribute>().Message
                            : FailureResponseAttribute.Generic);
                }
                else
                {
                    messageBuilder
                        .WithMessage(
                            !string.IsNullOrWhiteSpace(moduleCommand.GetEnumAttribute<GlobalCommand, AccessDeniedResponseAttribute>().Message)
                            ? moduleCommand.GetEnumAttribute<GlobalCommand, AccessDeniedResponseAttribute>().Message
                            : AccessDeniedResponseAttribute.Generic);
                }
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