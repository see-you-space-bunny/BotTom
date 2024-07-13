using FChatApi.Tokenizer;

namespace FChatApi.Tests.LabAssistant;

internal static class BotInfoAssistant
{
	internal const string BotName = "ApiUser";
	internal const string CommandChar = "bot!";
	internal static CommandParser CommandParser = new CommandParser(CommandChar);
	
}
