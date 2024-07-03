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
using BotTom.CardiApi;
using Bot_Tom.CardiApi;
using System.Text;

namespace Widget.CardGame
{
    public static partial class TournamentOrganiser
    {
        private const string BotUserIdentity = "Bot Tom";

        internal static Dictionary<string,PlayerCharacter> RegisteredCharacters { get; }

        internal static Dictionary<string,MatchChallenge> IncomingChallenges { get; }
        internal static Dictionary<string,MatchChallenge> OutgoingChallenges => IncomingChallenges.ToOutgoing();

        internal static List<BoardState> OngoingMatches { get; }
        internal static List<ChatMessage> MessageQueue { get; }


        static TournamentOrganiser()
        {
#if DEBUG
            string player1 = "Daniel";
            string player2 = "The Cooler Daniel";
            RegisteredCharacters = new Dictionary<string,PlayerCharacter>{
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

        public static async Task HandleCommand(ChatMessage message)
        {
            foreach(string key in OutgoingChallenges.Where((kvp)=>kvp.Value.AtTerminalStage).Select((kvp)=>kvp.Key))
                OutgoingChallenges.Remove(key);

            ChatMessageBuilder messageBuilder = new ChatMessageBuilder()
                .WithAuthor(BotUserIdentity)
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
                                      .WithChannel(message.Channel)
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
                              .WithChannel(message.Channel)
                              .WithMessage(
                    disallowedCommand.Reason switch
                    {
                        CommandPermission.AwaitingResponse => $"{RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity} already!",
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

        private static async Task AcceptChallenge(ChatMessage message,ChatMessageBuilder messageBuilder)
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
                          .WithChannel(message.Channel)
                          .WithMessage(sb.ToString())
                          .WithoutMention();
            
            await IncomingChallenges[message.Author.ToLower()].AdvanceState(MatchChallenge.Event.Confirm);
            OngoingMatches.Add(IncomingChallenges[message.Author.ToLower()].AcceptWithDeckArchetype(stat1,stat2));
        }

        private static async Task IssueChallenge(ChatMessage message,ChatMessageBuilder messageBuilder)
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
                          .WithChannel(message.Channel)
                          .WithMessage(sb.ToString())
                          .WithoutMention();

            IncomingChallenges.Add(playerIdentity.ToLower(),new MatchChallenge(
                RegisteredCharacters[message.Author.ToLower()].CreateMatchPlayer(stat1,stat2),
                RegisteredCharacters[playerIdentity.ToLower()]
            ));
            await IncomingChallenges[playerIdentity.ToLower()].AdvanceState(MatchChallenge.Event.Initiate);
        }

        private static async Task QueueMessage(ChatMessage message)
        {
            MessageQueue.Add(message);
        }
    }
}