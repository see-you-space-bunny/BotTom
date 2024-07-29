using System.Text;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.Attributes;
using Plugins.Attributes;

namespace Plugins.Tokenizer;

public class CommandParser
{
	public const string DefaultPrefix = "!";
	public const string DefaultInlinePrefix = "[noparse=";
	private int ModuleStartIndex { get => BotPrefix.Length; }
	public string BotPrefix { get; private set; }

	public CommandParser() : this(null)
	{ }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.
	public CommandParser(string? botPrefix)
	{
		WithBotPrefix(botPrefix);
	}
#pragma warning restore CS8618 // Consider adding the 'required' modifier or declaring as nullable.

	public CommandParser WithBotPrefix(string? value = null)
	{
		BotPrefix = value ?? DefaultPrefix;
		return this;
	}
	
	public bool TryConvertCommand(FChatMessage fChatMessage,out CommandTokens commandTokens)
	{

		// Not a command
		if (!fChatMessage.Message.StartsWith(BotPrefix))
		{
			commandTokens = null!;
			return false;
		}

		// parse the command; starts after the comand prefix
		string[] parsedTokens = Parse(fChatMessage.Message[ModuleStartIndex..]).ToArray();
		if (parsedTokens.Length < 1)
		{
#if DEBUG
			Console.WriteLine("it might be getting the bot's attention but doesn't have enough for a real command");
#endif
			commandTokens = null!;
			return false;
		}

		commandTokens = new CommandTokens(fChatMessage, parsedTokens[0], string.Join(' ',parsedTokens.Skip(1)));
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

		// we're done with the string, return whatever's left. There IS an assumption here that a trailing " isn't necessary.
		yield return token.ToString();
	}
}