namespace Widget.Tests.LabAssistant;

internal class ChatMessageAssistant
{
    internal BotCommand NewDummyMessage(string user,string message)
    {
        if (CommandParser.TryConvertCommand(
            user,
            new RegisteredUser(){ Name = user },
            null,
            message,
            out BotCommand? command
        ))
            return command!;
        else
            throw new ArgumentException($"Failed to parse the following message: {message}",nameof(message));
    }
}
