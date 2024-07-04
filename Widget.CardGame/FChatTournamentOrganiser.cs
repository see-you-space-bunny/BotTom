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

namespace Widget.CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin
{
    internal Dictionary<string,PlayerCharacter> RegisteredCharacters { get; }

    internal Dictionary<string,MatchChallenge> IncomingChallenges { get; }
    internal Dictionary<string,MatchChallenge> OutgoingChallenges => IncomingChallenges.ToOutgoing();

    internal List<BoardState> OngoingMatches { get; }
    internal List<ChatMessage> MessageQueue { get; }


    public FChatTournamentOrganiser(ApiConnection api, Channel[]? activeChannels, string commandChar,string? floatingCommandChar = null,TimeSpan? updateInterval = null) : base(api,activeChannels ?? [],commandChar,floatingCommandChar,updateInterval)
    {
#if DEBUG
        string player1 = "Daniel";
        string player2 = "The Cooler Daniel";
        RegisteredCharacters    = new Dictionary<string,PlayerCharacter>{
            [player1.ToLower()] = new PlayerCharacter(player1),
            [player2.ToLower()] = new PlayerCharacter(player2),
        };
        MessageQueue            = [];
        IncomingChallenges      = [];
        OngoingMatches          = [];
#else
        RegisteredCharacters    = [];
        MessageQueue            = [];
        IncomingChallenges      = [];
        OngoingMatches          = [];
#endif
    }

    public async override Task HandleRecievedMessage(BotCommand command, Channel? channel, string? message, User sendingUser, bool isOp)
    {
        ChatMessageBuilder messageBuilder = new ChatMessageBuilder()
            .WithAuthor(ApiConnection.CharacterName)
            .WithRecipient(sendingUser.Name)
            .WithMention(RegisteredCharacters[sendingUser.Name.ToLower()].MentionAndIdentity)
            .WithMessage(await ValidateCommandUse(command) ?? string.Empty);
        
        ////// DO STUFF HERE
        


        ////////////////////

        FChatApi.SendMessage(messageBuilder);
    }


    private async Task<string?> ValidateCommandUse(BotCommand command)
    {
        try
        {

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

    public async Task HandleCommand(ChatMessage message)
    {

        ChatMessageBuilder messageBuilder = new ChatMessageBuilder()
            .WithAuthor(ApiConnection.CharacterName)
            .WithRecipient(message.Author)
            .WithMention(RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity);

        Match match = CommandStringInterpreter.RegexMatchAnyCommand().Match(message.Message);
        if (!match.Success)
            return;

        if (Enum.TryParse(
            value: match.Groups.Values.Single((g)=>g.Name==CommandStringInterpreter.Command).Value,
            ignoreCase: true,
            result: out Command command))
        try
        {
            
            switch (command)
            {
                case Command.Challenge:
                    if (!OutgoingChallenges.ContainsKey(message.Author.ToLower()))
                        await IssueChallenge(message,messageBuilder);
                    else
                        throw new DisallowedCommandException(Command.Challenge,CommandPermission.AwaitingResponse);
                    break;

                case Command.Accept:
                    await AcceptChallenge(message,messageBuilder);
                    break;
                
                default:
                    messageBuilder.WithRecipient(message.Author)
                                  .WithMessageType(message.MessageType)
                                  //.WithChannel(message.Channel.Code)
                                  .WithMessage("That is not a valid command!")
                                  .WithMention(RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity);
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
        catch (DisallowedCommandException disallowedCommand)
        {
            messageBuilder.WithMessageType(message.MessageType)
                          //.WithChannel(message.Channel)
                          .WithMessage(
                disallowedCommand.Reason switch
                {
                    CommandPermission.InsufficientPermission    => $"{RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity}, you don't have permission to do that!",
                    CommandPermission.AwaitingResponse          => $"{RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity}, you still have a challenge!",
                    CommandPermission.ResponseRequired          => $"{RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity}, you need to respond to {IncomingChallenges[message.Author.ToLower()].Player1.PlayerCharacter.MentionAndIdentity}'s challenge first!",
                    _ => throw new ArgumentOutOfRangeException(nameof(disallowedCommand.Reason), $"Unexpected {typeof(CommandPermission)} value: {disallowedCommand.Reason}")
                }
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        await QueueMessage(messageBuilder.Build());
    }

    private async Task AcceptChallenge(ChatMessage message,ChatMessageBuilder messageBuilder)
    {
        Match match = CommandStringInterpreter.RegexMatchAcceptChallenge().Match(message.Message);
        if (!match.Success)
            return;

        IEnumerable<Group> matchGroups = match.Groups.Values;

        CharacterStat stat1 = Enum.Parse<CharacterStat>(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat1).Value,true);

        CharacterStat? stat2 = null;
        if (Enum.TryParse(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat2).Value, out CharacterStat s2))
            stat2 = s2;
            
        StringBuilder sb = new();
        sb.Append(RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity);
        sb.Append(" accepted ");
        sb.Append(IncomingChallenges[message.Author.ToLower()].Player1.PlayerCharacter.MentionAndIdentity);
        sb.Append("'s challenge!");

        messageBuilder.WithMessageType(message.MessageType)
                      //.WithChannel(message.Channel)
                      .WithMessage(sb.ToString())
                      .WithoutMention();
        
        await IncomingChallenges[message.Author.ToLower()].AdvanceState(MatchChallenge.Event.Confirm);
        OngoingMatches.Add(IncomingChallenges[message.Author.ToLower()].AcceptWithDeckArchetype(stat1,stat2));
    }

    private async Task IssueChallenge(ChatMessage message,ChatMessageBuilder messageBuilder)
    {
        Match match = CommandStringInterpreter.RegexMatchChallenge().Match(message.Message);
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
        sb.Append(RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity);
        sb.Append(" has challenged ");
        sb.Append(RegisteredCharacters[playerIdentity.ToLower()].MentionAndIdentity);
        sb.AppendLine(" to a [b]Duel[/b]! ");
        sb.Append("Use the command \"tom!xcg accept [i]stat2[/i] [i]stat2[/i]\"");
        
        messageBuilder.WithMessageType(message.MessageType)
                      //.WithChannel(message.Channel)
                      .WithMessage(sb.ToString())
                      .WithoutMention();

        IncomingChallenges.Add(playerIdentity.ToLower(),new MatchChallenge(
            RegisteredCharacters[message.Author.ToLower()].CreateMatchPlayer(stat1,stat2),
            RegisteredCharacters[playerIdentity.ToLower()]
        ));
        await IncomingChallenges[playerIdentity.ToLower()].AdvanceState(MatchChallenge.Event.Initiate);
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

    private async Task QueueMessage(ChatMessage message)
    {
        MessageQueue.Add(message);
    }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
}