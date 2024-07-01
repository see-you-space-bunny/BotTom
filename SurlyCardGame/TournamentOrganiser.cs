using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurlyCardGame.Enums;

namespace SurlyCardGame
{
    public static class TournamentOrganiser
    {
        public const string BotCommandIdentifier = "tom!";
        public const string ModuleCommandKey = "xcg";
        internal static Dictionary<string,PlayerCharacter> _playerCharacters = [];
        internal static Dictionary<string,(DateTime TimeIssued,MatchPlayer MatchPlayer)> _incomingChallenges = [];
        internal static List<BoardState> _ongoingMatches = [];

        static TournamentOrganiser()
        {

        }

        public static string HandleCommand(string identity, string commandString)
        {
            var commandStringInterpreter = new CommandStringInterpreter(commandString);
            try
            {
                commandStringInterpreter.Next();
                commandStringInterpreter.Next();
                (string rawCommand,Command command) = commandStringInterpreter.GetCommand();
                (string rawPlayerIdentity,string playerIdentity) = commandStringInterpreter.GetPlayerIdentity();

                switch (command)
                {
                    case Command.Challenge:
                    case Command.Chal:
                        IssueChallenge(identity,"STR","INT",playerIdentity);
                        break;

                    case Command.Accept:
                        //
                        break;
                }
                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private static void IssueChallenge(string player1,string stat1,string stat2,string player2)
        {
            _incomingChallenges.Add(player2,(DateTime.Now,_playerCharacters[player1.ToLower()].CreateMatchPlayer(stat1,stat2)));
        }
    }
}