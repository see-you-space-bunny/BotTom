using Plugins.Tokenizer;

namespace Widget.Tests.LabAssistant;

internal static class BotInfoAssistant
{
	internal const string BotName = "ApiUser";
	internal const string CommandChar = "bot!";
	internal static CommandParser CommandParser = new CommandParser(CommandChar);
	
}
