using Engine.ModuleHost.CommandHandling;

namespace Widget.Tests.LabAssistant;

internal static class BotInfoAssistant
{
    const string BotName = "Bot Tom";
    const string CommandChar = "tom!";
    CommandParser CommandParser = new CommandParser(CommandChar,[]);
    
}
