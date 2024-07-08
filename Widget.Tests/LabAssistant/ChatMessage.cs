using Engine.ModuleHost.CommandHandling;

namespace Widget.Tests.LabAssistant;

internal static class ChatMessageAssistant
{
	internal static BotCommand NewDummyMessage(string user,string message)
	{
		if (BotInfoAssistant.CommandParser.TryConvertCommand(
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
