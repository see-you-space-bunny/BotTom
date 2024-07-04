using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleHost.CommandHandling;

public class CommandParser
{
    private int ModuleStartIndex { get; }
    private string FullBotPrefix { get; }
    public string BotPrefix { get; }

    public CommandParser(string botPrefix)
    {
        BotPrefix = botPrefix;
        FullBotPrefix = botPrefix + (botPrefix.EndsWith('!') ? string.Empty : '!');
        ModuleStartIndex = FullBotPrefix.Length;
    }

    public bool TryConvertCommand(string unparsed, out BotCommand? botCommand)
    {
        // Not a command
        if (!unparsed.StartsWith(FullBotPrefix))
        {
            botCommand = null;
            return false;
        }

        // parse the command; starts after the comand prefix
        string[] parsedTokens = Parse(unparsed[ModuleStartIndex..]).ToArray();
        if (parsedTokens.Length < 2)
        {
            // it might be getting the bot's attention but doesn't have enough for a real command
            botCommand = null;
            return false;
        }
        
        if (!Enum.TryParse(parsedTokens[0], out BotModule botModule))
        {
            // Not a recognized module
            botCommand = null;
            return false;
        }

        botCommand = new BotCommand(botModule, parsedTokens[1], parsedTokens.Skip(1).ToArray());
        return true;
    }

    private static IEnumerable<string> Parse(string unparsedCommand)
    {
        // this is true for empty strings as well, it makes a good sanity check
        if (string.IsNullOrWhiteSpace(unparsedCommand))
            yield break;

        StringBuilder token = new();
        char prev, next, current;

        // always starts out not inside quotes
        bool inString = false;

        for (int index = 0; index < unparsedCommand.Length; index++)
        {
            if (index > 0)
                prev = unparsedCommand[index - 1];
            else
                prev = '\0';

            if (index < unparsedCommand.Length - 1)
                next = unparsedCommand[index + 1];
            else
                next = '\0';

            current = unparsedCommand[index];

            // character is the start of a string
            if (current == '"' && (prev == '\0' || prev == ' ') && !inString)
            {
                inString = true;
                continue;
            }

            // character is the end of a string
            if (current == '"' && (next == '\0' || next == ' ') && inString)
            {
                inString = false;
                continue;
            }

            // character is the delimiter AND we're not in a string
            if (current == ' ' && !inString)
            {
                yield return token.ToString();
                token = token.Remove(0, token.Length);
                continue;
            }

            // None of the above were true. Add the character to the token and keep going.
            token = token.Append(current);
        }

        // we're done with the string, return whatever's left.  There IS an assumption here that a trailing " isn't necessary.
        yield return token.ToString();
    }
}