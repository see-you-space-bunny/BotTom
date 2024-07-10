using FChatApi.Tokenizer;

namespace FChatApi.Tests.LabAssistant;

internal static class BotInfoAssistant
{
	internal const string BotName = "Bot Tom";
	internal const string CommandChar = "tom!";
	internal static CommandParser CommandParser = new CommandParser(CommandChar);
	
}
