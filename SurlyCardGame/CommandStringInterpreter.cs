using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SurlyCardGame.Enums;

namespace SurlyCardGame
{
    public partial class CommandStringInterpreter
    {
        private enum CommandSegment
        {
            None,
            BotCommandIdentifier,
            ModuleCommandKey,
            ModuleCommand,
            ModuleCommandOption,
            PlayerIdentity,
        }

        private string _commandString;
        private string ActiveSegment => _commandString.Substring(_currentIndex.Start,_currentIndex.Length);
        private (int Start,int End,int Length) _previousIndex = (0,0,0);
        private (int Start,int End,int Length) _currentIndex = (0,0,0);
        private CommandSegment _previousCommandSegment = CommandSegment.None;
        private CommandSegment _commandSegment = CommandSegment.BotCommandIdentifier;

        public CommandStringInterpreter(string commandString)
        {
            _commandString = commandString;
            Next();
        }

        public bool Next()
        {
            switch (_commandSegment)
            {
                case CommandSegment.None:
                    _currentIndex = (0,TournamentOrganiser.BotCommandIdentifier.Length,TournamentOrganiser.BotCommandIdentifier.Length);
                    _previousCommandSegment = _commandSegment;
                    _commandSegment = CommandSegment.BotCommandIdentifier;
                    break;

                case CommandSegment cs when cs == CommandSegment.BotCommandIdentifier:
                    _previousIndex = _currentIndex;
                    _currentIndex = MoveIndex(
                        RegexWhiteSpaces().Match(_commandString,_currentIndex.End).Length,
                        TournamentOrganiser.ModuleCommandKey.Length
                    );
                    _previousCommandSegment = _commandSegment;
                    _commandSegment = CommandSegment.ModuleCommandKey;
                    break;

                case CommandSegment cs when cs == CommandSegment.ModuleCommandKey:
                    _previousIndex = _currentIndex;
                    _currentIndex = MoveIndex(RegexWhiteSpaces().Match(_commandString,_currentIndex.End).Length);
                    _currentIndex = SetIndexLength(RegexWhiteSpaces().Match(_commandString,_currentIndex.Start).Length);
                    
                    _previousCommandSegment = _commandSegment;
                    _commandSegment = CommandSegment.ModuleCommand;

                    if (!TryIsCommand())
                        throw new ArgumentException($"{ActiveSegment} is not a valid Command!");

                    break;

                case CommandSegment cs when cs == CommandSegment.ModuleCommand:
                    _previousIndex = _currentIndex;
                    _currentIndex = MoveIndex(RegexWhiteSpaces().Match(_commandString,_currentIndex.End).Length);
                    _currentIndex = SetIndexLength(RegexWhiteSpaces().Match(_commandString,_currentIndex.Start).Length);
                    
                    _previousCommandSegment = _commandSegment;

                    // ModuleCommand OR ModuleCommandOption
                    _commandSegment = CommandSegment.ModuleCommandOption;
                    
                    break;
            }
            return true;
        }

        private (int Start,int End,int Length) MoveIndex(int value, int newLength) => (_currentIndex.End+value,_currentIndex.End+value+newLength,newLength);
        private (int Start,int End,int Length) MoveIndex(int value) => (_currentIndex.End+value,_currentIndex.End+value,0);
        private (int Start,int End,int Length) SetIndexLength(int value) => (_currentIndex.Start,_currentIndex.Start+value,value);

        private bool TryIsCommand() => Enum.GetNames<Command>().Contains(ActiveSegment,StringComparer.CurrentCultureIgnoreCase);
        public bool IsCommand() => _commandSegment == CommandSegment.ModuleCommand;

        public (string Raw,Command Value) GetCommand() => (ActiveSegment, Enum.Parse<Command>(ActiveSegment,true));

        public (string Raw,string Value) GetPlayerIdentity() => (ActiveSegment, TournamentOrganiser._playerCharacters[ActiveSegment.ToLower()].Identity);

        [GeneratedRegex(@"\s+")]
        private static partial Regex RegexWhiteSpaces();
    }
}