using System;
using System.Text;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Runtime.CompilerServices;
using System.Reactive.Concurrency;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using FChatApi.Core;
using FChatApi.Objects;
using FChatApi.Enums;
using Widget.CardGame.Enums;
using Widget.CardGame.MatchEntities;
using Widget.CardGame.Commands;
using Widget.CardGame.PersistentEntities;
using Widget.CardGame.DataStructures;
using Widget.CardGame.Interfaces;
using Widget.CardGame.Exceptions;
using Engine.ModuleHost;
using Engine.ModuleHost.CardiApi;
using Engine.ModuleHost.Attributes;
using Engine.ModuleHost.Enums;
using Engine.ModuleHost.Plugins;
using Engine.ModuleHost.CommandHandling;
using Widget.CardGame.Attributes;

namespace Widget.CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
    internal Dictionary<string,(RegisteredUser FChatUser,PlayerCharacter ModuleCharacter)> RegisteredCharacters { get; }

    internal Dictionary<string,MatchChallenge> IncomingChallenges { get; }
    internal Dictionary<string,MatchChallenge> OutgoingChallenges => IncomingChallenges.ToOutgoing();

    internal List<BoardState> OngoingMatches { get; }

#if DEBUG
    internal ChatMessageBuilder MostRecentMessage = null!; 
#endif

    public FChatTournamentOrganiser(TimeSpan? updateInterval = null) : this(null!,updateInterval)
    { }

    public FChatTournamentOrganiser(ApiConnection api,TimeSpan? updateInterval = null) : base(api,updateInterval)
    {
        ModuleType              = BotModule.XCG;
        RegisteredCharacters    = [];
        IncomingChallenges      = [];
        OngoingMatches          = [];
    }

	private static void PreProcessEnumAttributes()
	{
        AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(CharacterStat));
        AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(CharacterStatGroup));
        AttributeEnumExtensions.ProcessEnumForAttribute<DescriptionAttribute>(typeof(Command));

        AttributeEnumExtensions.ProcessEnumForAttribute<StatAliasAttribute  >(typeof(CharacterStat));
        
        AttributeEnumExtensions.ProcessEnumForAttribute<StatGroupAttribute  >(typeof(CharacterStat));
	}

    public override void HandleRecievedMessage(BotCommand command)
    {
        ChatMessageBuilder messageBuilder = new ChatMessageBuilder()
            .WithAuthor(ApiConnection.CharacterName)
            .WithRecipient(command.User!.Name)
            .WithMention(command.User!.Mention.Name.WithPronouns)
            .WithChannel(command.Channel);
        
        string message = ValidateCommandUse(command);

        if (!string.IsNullOrWhiteSpace(message))
            FChatApi.EnqueueMessage(messageBuilder.WithMessage(message));

        ////// DO STUFF HERE
        
        HandleValidatedCommand(messageBuilder,command);

        ////////////////////

#if DEBUG
        MostRecentMessage = messageBuilder;
#endif
        FChatApi.EnqueueMessage(messageBuilder);
    }


    private string ValidateCommandUse(BotCommand command)
    {
        try
        {
            // User may not be null
            if (command.User == null)
                throw new ArgumentNullException(nameof(command),$"The (BotCommand) User property may not be null when handling a {command.Command} command.");

            return string.Empty;
        }
        catch (DisallowedCommandException disallowedCommand)
        {
            return disallowedCommand.Reason switch
            {
                CommandPermission.InsufficientPermission    => "You don't have permission to do that!",
                CommandPermission.ResponseRequired          => $"You need to respond to {IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention.Name.Basic}'s challenge first!",
                _ => throw new ArgumentOutOfRangeException(command.ToString(), $"Unexpected {typeof(CommandPermission)} value: {disallowedCommand.Reason}")
            };
        }
    }

    public void HandleValidatedCommand(ChatMessageBuilder messageBuilder,BotCommand command)
    {
        if (Enum.TryParse(value: command.Parameters[0],ignoreCase: true,result: out Command ModuleCommand))
        try
        {
            var alertTargetMessage = new ChatMessageBuilder()
                .WithAuthor(ApiConnection.CharacterName)
                .WithoutMention();
            switch (ModuleCommand)
            {
                case Command.Challenge:
                    if (IssueChallenge(command,messageBuilder.WithoutMention(),alertTargetMessage))
                        FChatApi.EnqueueMessage(alertTargetMessage);
                    break;

                case Command.Accept:
                    if (AcceptChallenge(command,messageBuilder.WithoutMention(),alertTargetMessage))
                        FChatApi.EnqueueMessage(alertTargetMessage);
                    break;
                
                default:
                    messageBuilder
                        .WithRecipient(command.User!.Name)
                        .WithMessage("That is not a valid command!")
                        .WithMention(RegisteredCharacters[command.User.Name.ToLower()].FChatUser.Mention.Name.WithPronouns);
                    break;
            }
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.InnerException);
            Console.WriteLine(e.StackTrace);
            Console.WriteLine();
            Console.WriteLine(e.Message);
        }
        catch (InvalidCommandSyntaxException e)
        {
            messageBuilder.WithMessage(e.Message);
        }
        catch (DisallowedCommandException disallowedCommand)
        {
            messageBuilder.WithMessage(
                disallowedCommand.Reason switch
                {
                    CommandPermission.InsufficientPermission    => $"{RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns}, you don't have permission to do that!",
                    CommandPermission.AwaitingResponse          => $"{RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns}, you still have a challenge!",
                    CommandPermission.ResponseRequired          => $"{RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns}, you need to respond to {IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention.Name.Basic}'s challenge first!",
                    _ => throw new ArgumentOutOfRangeException(nameof(disallowedCommand.Reason), $"Unexpected {typeof(CommandPermission)} value: {disallowedCommand.Reason}")
                }
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
    private bool IssueChallenge(BotCommand command,ChatMessageBuilder commandResponse,ChatMessageBuilder targetAlertResponse)
    {
        StringBuilder responseBuilder   = new();
        StringBuilder alertBuilder      = new();
        
        CharacterStat stat1 = default;
        if (command.Parameters.Length < 2)
        {
            commandResponse
                .WithMessage("You need to specify at least one stat with which to build your deck.");
            return false;
        }
        else if (!Enum.TryParse(command.Parameters[1],true,out stat1))
        {
            commandResponse
                .WithMessage($"{command.Parameters[1].ToUpper()} is not a recognised stat.");
            return false;
        }

        CharacterStat stat2 = default;
        if (command.Parameters.Length > 2)
        {
            if (!Enum.TryParse(command.Parameters[2],true,out stat2) && command.Parameters.Length > 3)
            {
                commandResponse
                    .WithMessage($"{command.Parameters[2].ToUpper()} is not a recognised stat.");
                return false;
            }
        }
        else
        {
            commandResponse
                .WithMessage($"Too few arguments! You need to specify who your are challenging.");
            return false;
        }

        string target;
        if (stat2 == default)
        {
            target = string.Join(' ',command.Parameters[2..]);
        }
        else if (command.Parameters.Length > 3)
        {
            target = string.Join(' ',command.Parameters[3..]);
        }
        else
        {
            commandResponse
                .WithMessage($"You have to specify a user to challenge.");
            return false;
        }

        var (exitEarly, errMessage, player) = ValidateIssueChallenge(command.User!.Name,target);

        if (exitEarly)
        {
            commandResponse
                .WithMessage(errMessage);
            return false;
        }
        
        //////////

        responseBuilder
            .Append(RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns)
            .Append(" has challenged ")
            .Append(player.Name)
            .Append(" to a [b]Duel[/b]! ")
            .Append("[i]Hint:[/i] To accept this challenge, use the command \"tom!xcg accept [i]stat1[/i] [i]stat2[/i]\"");

        alertBuilder
            .Append(RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns)
            .Append(" has challenged you to a [b]Duel[/b]! ")
            .Append("[i]Hint:[/i] To accept this challenge, use the command \"tom!xcg accept [i]stat1[/i] [i]stat2[/i]\"");
        
        //////////

        commandResponse
            .WithMessage(responseBuilder.ToString());

        targetAlertResponse
            .WithRecipient(player.Name)
            .WithMessage(alertBuilder.ToString());
        
        //////////

        IncomingChallenges.Add(
            player.Name.ToLower(),
            new MatchChallenge(
                command.User!,
                RegisteredCharacters[command.User!.Name.ToLower()].ModuleCharacter.CreateMatchPlayer(stat1,stat2),
                RegisteredCharacters[player.Name.ToLower()].ModuleCharacter
            )
        );
        IncomingChallenges[player.Name.ToLower()].AdvanceState(MatchChallenge.Event.Initiate);
        return true;
    }

    private bool AcceptChallenge(BotCommand command,ChatMessageBuilder commandResponse,ChatMessageBuilder challengerAlertResponse)
    {
        CharacterStat stat1 = default;
        if (command.Parameters.Length < 2)
        {
            commandResponse
                .WithMessage("You need to specify at least one stat with which to build your deck.");
            return false;
        }
        else if (!Enum.TryParse(command.Parameters[1],true,out stat1))
        {
            commandResponse
                .WithMessage($"{command.Parameters[1].ToUpper()} is not a recognised stat.");
            return false;
        }

        CharacterStat stat2 = default;
        if (command.Parameters.Length > 2)
        {
            if (!Enum.TryParse(command.Parameters[2],true,out stat2))
            {
                commandResponse
                    .WithMessage($"{command.Parameters[2].ToUpper()} is not a recognised stat.");
                return false;
            }
        }

        //////////
            
        var responseBuilder = new StringBuilder()
            .Append(command.User!.Mention.Name.Basic)
            .Append(" has accepted ")
            .Append(IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention.Name.Basic)
            .Append("'s challenge!");

        var alertBuilder    = new StringBuilder()
            .Append(command.User!.Mention.Name.Basic)
            .Append(" has accepted your challenge!");
        
        //////////

        commandResponse
            .WithMessage(responseBuilder.ToString())
            .WithMessageType(ChatMessageType.Whisper);

        challengerAlertResponse
            .WithMessage(alertBuilder.ToString())
            .WithRecipient(IncomingChallenges[command.User!.Name.ToLower()].Challenger.Name)
            .WithMessageType(ChatMessageType.Whisper);
        
        //////////

        IncomingChallenges[command.User!.Name.ToLower()].AdvanceState(MatchChallenge.Event.Confirm);

        OngoingMatches.Add(IncomingChallenges[command.User!.Name.ToLower()].AcceptWithDeckArchetype(stat1,stat2));
        return true;
    }

    private (bool exitEarly,string message,RegisteredUser target) ValidateIssueChallenge(string challenger,string target)
    {
        RegisteredUser player;
        if (!RegisteredCharacters.TryGetValue(target.ToLower(), out (RegisteredUser FChatUser,PlayerCharacter ModuleCharacter) userAndPlayer))
        {
            return (true,"You can't challenge a user that is not registered.",new RegisteredUser());
        }
        else
        {
            player = userAndPlayer.FChatUser;
        }

        if (target == challenger)
        {
            return (true,"You can't challenge yourself.",new RegisteredUser());
        }
        else if (OutgoingChallenges.ContainsKey(challenger.ToLower()))
        {
            return (true,"You can't have more than one pending challenge.",new RegisteredUser());
        }
        else if (!RegisteredCharacters.ContainsKey(player.Name.ToLower()))
        {
            return (true,"That player is already being challenged by you.",new RegisteredUser());
        }
        else if (IncomingChallenges.ContainsKey(player.Name.ToLower()))
        {
            if (IncomingChallenges[player.Name.ToLower()].Challenger.Name == challenger)
            {
                return (true,"That player is already being challenged by you.",new RegisteredUser());
            }
            else
            {
                return (true,"That player is already being challenged by someone.",new RegisteredUser());
            }
        }
        return (false,string.Empty,player);
    }



    public async override Task Update()
    {
        Task t = base.Update();
        foreach(string key in OutgoingChallenges.Where((kvp)=>kvp.Value.AtTerminalStage).Select((kvp)=>kvp.Key))
            OutgoingChallenges.Remove(key);
        await t;
    }

    public override void HandleJoinedChannel(Channel channel)
    {
        ActiveChannels.TryAdd(channel.Code,channel);
    }

    static FChatTournamentOrganiser()
    {
        PreProcessEnumAttributes();
    }
}