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

namespace Widget.CardGame
{
    public static partial class TournamentOrganiser
    {

        internal static Dictionary<string,PlayerCharacter> RegisteredCharacters { get; }
        internal static Dictionary<string,MatchChallenge> IncomingChallenges { get; }
        internal static List<BoardState> OngoingMatches { get; }
        internal static Dictionary<string,MatchChallenge> OutgoingChallenges => IncomingChallenges.ToOutgoing();

        internal static List<ICommand> ActionQueue { get; }
        public static List<BotTom.CardiApi.ChatMessage> MessageQueue { get; }


        static TournamentOrganiser()
        {
#if DEBUG
            string player1 = "Daniel";
            string player2 = "The Cooler Daniel";
            RegisteredCharacters = new Dictionary<string,PlayerCharacter>{
                [player1.ToLower()] = new PlayerCharacter(player1),
                [player2.ToLower()] = new PlayerCharacter(player2),
            };
            ActionQueue             = [];
            MessageQueue            = [];
            IncomingChallenges      = [];
            OngoingMatches          = [];
#else
            RegisteredCharacters    = [];
            ActionQueue             = [];
            MessageQueue            = [];
            IncomingChallenges      = [];
            OngoingMatches          = [];
#endif
        }

        public static void HandleCommand(BotTom.CardiApi.ChatMessage message)
        {
            if (!CommandStringInterpreter.RegexMatchAnyCommand().IsMatch(message.Message))
                return;

            Command command = Enum.Parse<Command>(
                CommandStringInterpreter.RegexMatchAnyCommand()
                .Match(message.Message)
                .Groups
                .Values
                .Single((g)=>g.Name==CommandStringInterpreter.CommandWord)
                .Value,
                true);
            try
            {
                switch (command)
                {
                    case Command.Challenge:
                        IssueChallenge(message);
                        break;

                    case Command.Accept:
                        AcceptChallenge(message);
                        break;
                    
                    default:
                        MessageQueue.Add(new BotTom.CardiApi.ChatMessage(
                            "Bot Tom",
                            message.Recipient,
                            message.MessageType,
                            message.Channel,
                            $"That is not a valid command!"
                        ));
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
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void AcceptChallenge(BotTom.CardiApi.ChatMessage message)
        {
            if (!CommandStringInterpreter.RegexMatchAcceptChallenge().IsMatch(message.Message))
                return;

            IEnumerable<Group> matchGroups = CommandStringInterpreter.RegexMatchAcceptChallenge().Match(message.Message).Groups.Values;

            CharacterStat stat1 = Enum.Parse<CharacterStat>(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat1).Value,true);

            CharacterStat? stat2 = null;
            if (Enum.TryParse(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat2).Value, out CharacterStat s2))
                stat2 = s2;
            IncomingChallenges[message.Author.ToLower()].AcceptWithDeckArchetype(stat1,stat2);
            IncomingChallenges[message.Author.ToLower()].AdvanceState(MatchChallenge.Event.Confirm);

            MessageQueue.Add(new BotTom.CardiApi.ChatMessage(
                "Bot Tom",
                message.Recipient,
                message.MessageType,
                message.Channel,
                $"{RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity} accepted " +
                    $"{IncomingChallenges[message.Author.ToLower()].Player1.PlayerCharacter.MentionAndIdentity}'s challenge!"
            ));
        }

        private static void IssueChallenge(BotTom.CardiApi.ChatMessage message)
        {
            if (!CommandStringInterpreter.RegexMatchChallenge().IsMatch(message.Message))
                return;

            IEnumerable<Group> matchGroups = CommandStringInterpreter.RegexMatchChallenge().Match(message.Message).Groups.Values;

            CharacterStat stat1 = Enum.Parse<CharacterStat>(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat1).Value,true);

            string playerIdentity = matchGroups.Single((g)=>g.Name==CommandStringInterpreter.PlayerIdentity).Value;
            
            CharacterStat? stat2 = null;
            if (Enum.TryParse(matchGroups.Single((g)=>g.Name==CommandStringInterpreter.Stat2).Value, out CharacterStat s2))
                stat2 = s2;
            
            IncomingChallenges.Add(playerIdentity,new MatchChallenge(
                RegisteredCharacters[message.Author.ToLower()].CreateMatchPlayer(stat1,stat2),
                RegisteredCharacters[playerIdentity.ToLower()]
            ));
            IncomingChallenges[playerIdentity].AdvanceState(MatchChallenge.Event.Initiate);
            
            MessageQueue.Add(new BotTom.CardiApi.ChatMessage(
                "Bot Tom",
                message.Recipient,
                message.MessageType,
                message.Channel,
                $"{RegisteredCharacters[message.Author.ToLower()].MentionAndIdentity} has challenged " + 
                    $"{RegisteredCharacters[playerIdentity.ToLower()].MentionAndIdentity} to a [b]Duel[/b]! " +
                    "Use the command \"tom!xcg accept [i]stat2[/i] [i]stat2[/i]\""
            ));
        }

        public static async Task HandleActionQueue()
        {
            foreach(ICommand action in ActionQueue)
            {
                action.ExecuteCommand();
            }
            ActionQueue.Clear();
        }
    }
}