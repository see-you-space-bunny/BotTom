using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Widget.CardGame.Enums;
using Widget.CardGame.MatchEntities;
using Widget.CardGame.Commands;
using Widget.CardGame.PersistentEntities;
using Widget.CardGame.DataStructures;
using Widget.CardGame.Interfaces;
using Widget.CardGame.Exceptions;
using System.Text;
using ModuleHost;
using BotTom.FChat;
using ModuleHost.CommandHandling;
using ChatApi.Objects;
using ChatApi.Core;
using Discord;
using ModuleHost.CardiApi;
using System.Diagnostics;

namespace Widget.CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
    internal Dictionary<string,(RegisteredUser FChatUser,PlayerCharacter ModuleCharacter)> RegisteredCharacters { get; }

    internal Dictionary<string,MatchChallenge> IncomingChallenges { get; }
    internal Dictionary<string,MatchChallenge> OutgoingChallenges => IncomingChallenges.ToOutgoing();

    internal List<BoardState> OngoingMatches { get; }

#if DEBUG
    internal ChatMessageBuilder MostRecentMessage = null; 
#endif

    public FChatTournamentOrganiser(Channel[]? activeChannels, string commandChar,string? floatingCommandChar = null,TimeSpan? updateInterval = null) : this(null,activeChannels ?? [],commandChar,floatingCommandChar,updateInterval)
    { }

    public FChatTournamentOrganiser(ApiConnection api, Channel[]? activeChannels, string commandChar,string? floatingCommandChar = null,TimeSpan? updateInterval = null) : base(api,activeChannels ?? [],commandChar,floatingCommandChar,updateInterval)
    {
#if DEBUG
        string player1 = "Daniel";
        string player2 = "The Cooler Daniel";
        RegisteredCharacters    = new Dictionary<string,(RegisteredUser,PlayerCharacter)>{
            [player1.ToLower()] = (new RegisteredUser(){ Name = player1 }, new PlayerCharacter(player1)),
            [player2.ToLower()] = (new RegisteredUser(){ Name = player2 }, new PlayerCharacter(player2)),
        };
#else
        RegisteredCharacters    = [];
#endif
        IncomingChallenges      = [];
        OngoingMatches          = [];
    }

    public async override Task HandleRecievedMessage(BotCommand command)
    {
        ChatMessageBuilder messageBuilder = new ChatMessageBuilder()
            .WithAuthor(ApiConnection.CharacterName)
            .WithRecipient(command.User!.Name)
            .WithMention(command.User!.Mention.Name.WithPronouns)
            .WithChannel(command.Channel);
        
        string message = ValidateCommandUse(command);
        if (!string.IsNullOrWhiteSpace(message))
            FChatApi.SendMessage(messageBuilder.WithMessage(message));
        
        ////// DO STUFF HERE
        
        await HandleValidatedCommand(messageBuilder,command);

        ////////////////////

#if DEBUG
        MostRecentMessage = messageBuilder;
#else
        FChatApi.SendMessage(messageBuilder);
#endif
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
                CommandPermission.InsufficientPermission    => "{0}, you don't have permission to do that!",
                CommandPermission.AwaitingResponse          => "{0}, you still have a challenge!",
                CommandPermission.ResponseRequired          => "{0}, you need to respond to {IncomingChallenges[message.Author.ToLower()].Player1.PlayerCharacter.MentionAndIdentity}'s challenge first!",
                _ => throw new ArgumentOutOfRangeException(command.ToString(), $"Unexpected {typeof(CommandPermission)} value: {disallowedCommand.Reason}")
            };
        }
    }

    public async Task HandleValidatedCommand(ChatMessageBuilder messageBuilder,BotCommand command)
    {
        if (Enum.TryParse(value: command.Parameters[0],ignoreCase: true,result: out Command ModuleCommand))
        try
        {
            switch (ModuleCommand)
            {
                case Command.Challenge:
                    if (!OutgoingChallenges.ContainsKey(command.User!.Name.ToLower()))
                        await IssueChallenge(command,messageBuilder);
                    else
                        throw new DisallowedCommandException(Command.Challenge,CommandPermission.AwaitingResponse);
                    break;

                case Command.Accept:
                    await AcceptChallenge(command,messageBuilder);
                    break;
                
                default:
                    messageBuilder.WithRecipient(command.User!.Name)
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

    private async Task IssueChallenge(BotCommand command,ChatMessageBuilder messageBuilder)
    {
        Match match = CommandStringInterpreter.RegexMatchChallenge().Match(command.Command);
        if (!match.Success)
        {

            throw new InvalidCommandSyntaxException("Invalid use of the \"Challenge\" command!");
        }

        IEnumerable<Group> matchGroups = match.Groups.Values;

        CharacterStat stat1 = Enum.Parse<CharacterStat>(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat1).Value,true);

        string playerIdentity = matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Player).Value;
        
        CharacterStat? stat2 = null;
        if (Enum.TryParse(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat2).Value, out CharacterStat s2))
            stat2 = s2;
        
        StringBuilder sb = new();
        sb.Append(RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns);
        sb.Append(" has challenged ");
        sb.Append(RegisteredCharacters[playerIdentity.ToLower()].FChatUser.Mention.Name.WithPronouns);
        sb.AppendLine(" to a [b]Duel[/b]! ");
        sb.Append("Use the command \"tom!xcg accept [i]stat2[/i] [i]stat2[/i]\"");
        
        messageBuilder.WithMessage(sb.ToString())
                      .WithoutMention();

        IncomingChallenges.Add(playerIdentity.ToLower(),new MatchChallenge(
            (RegisteredUser)command.User,
            RegisteredCharacters[command.User.Name.ToLower()].ModuleCharacter.CreateMatchPlayer(stat1,stat2),
            RegisteredCharacters[playerIdentity.ToLower()].ModuleCharacter
        ));
        await IncomingChallenges[playerIdentity.ToLower()].AdvanceState(MatchChallenge.Event.Initiate);
    }

    private async Task AcceptChallenge(BotCommand command,ChatMessageBuilder messageBuilder)
    {
        Match match = CommandStringInterpreter.RegexMatchAcceptChallenge().Match(command.Command);
        if (!match.Success)
            return;

        IEnumerable<Group> matchGroups = match.Groups.Values;

        CharacterStat stat1 = Enum.Parse<CharacterStat>(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat1).Value,true);

        CharacterStat? stat2 = null;
        if (Enum.TryParse(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat2).Value, out CharacterStat s2))
            stat2 = s2;
            
        StringBuilder sb = new();
        sb.Append(RegisteredCharacters[command.User!.Name.ToLower()].FChatUser.Mention.Name.WithPronouns);
        sb.Append(" accepted ");
        sb.Append(IncomingChallenges[command.User!.Name.ToLower()].Challenger.Mention.Name.WithPronouns);
        sb.Append("'s challenge!");

        messageBuilder.WithMessage(sb.ToString())
                      .WithoutMention();
        
        await IncomingChallenges[command.User!.Name.ToLower()].AdvanceState(MatchChallenge.Event.Confirm);
        OngoingMatches.Add(IncomingChallenges[command.User!.Name.ToLower()].AcceptWithDeckArchetype(stat1,stat2));
    }


    public override async Task Update()
    {
        foreach(string key in OutgoingChallenges.Where((kvp)=>kvp.Value.AtTerminalStage).Select((kvp)=>kvp.Key))
            OutgoingChallenges.Remove(key);
        
        await base.Update();
    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    public override async Task HandleJoinedChannel(Channel channel)
    {
        ActiveChannels.TryAdd(channel.Code,channel);
    }
#pragma warning restore CS1998
}