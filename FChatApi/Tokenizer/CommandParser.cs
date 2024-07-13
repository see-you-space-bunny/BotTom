using System.Text;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Attributes;

namespace FChatApi.Tokenizer;

public class CommandParser
{
	private int ModuleStartIndex { get => BotPrefix.Length; }
	public string BotPrefix { get; private set; }

	public CommandParser() : this(string.Empty)
	{ }

	public CommandParser(string botPrefix)
	{
		WithBotPrefix(botPrefix);
	}

	public CommandParser WithBotPrefix(string value)
	{
		BotPrefix = value + (value.EndsWith('!') ? string.Empty : '!');
		return this;
	}
	
	public bool TryConvertCommand(FChatMessage @event,out BotCommand command)
	{
		// Not a command
		if (!@event.Message.StartsWith(BotPrefix))
		{
			command = null;
			return false;
		}

		// parse the command; starts after the comand prefix
		string[] parsedTokens = Parse(@event.Message[ModuleStartIndex..]).ToArray();
		if (parsedTokens.Length < 2)
		{
#if DEBUG
			Console.WriteLine("it might be getting the bot's attention but doesn't have enough for a real command");
#endif
			command = null;
			return false;
		}
		
		string botModule = default;
		if (!(parsedTokens.Length == 0))
		{
			// Not a recognized module
			botModule = parsedTokens[0];
		}
		
		string moduleCommand = default;
		if (!(parsedTokens.Length < 0))
		{
			// Not a recognized command
			moduleCommand = parsedTokens[1];
		}

		// Command cannot be issued by a non-user
		if (@event.Author == null)
		{
			// Some operations may still trigger, but they will be exceptions
#if DEBUG
			Console.WriteLine("Command cannot be issued by a non-user");
			Console.WriteLine("Some operations may still trigger, but they will be exceptions");
#endif
			command = null;
			return false;
		}

		Privilege privilege = Privilege.None;
		if (@event.Author.PrivilegeLevel == Privilege.None)
		{
#if DEBUG
			Task.Run(()=>Console.WriteLine("Defaulting 'Privilege.None user to 'Privilege.UnregisteredUser"));
#endif
			privilege = Privilege.UnregisteredUser;
		}
		else
		{
			privilege = @event.Author!.PrivilegeLevel;
		}
		
		command = new BotCommand(@event, botModule, moduleCommand, parsedTokens.Skip(2).ToArray());
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