using FChatApi.Enums;
using FChatApi.Objects;
using FChatApi.Tokenizer;

namespace Widget.Tests.LabAssistant;

internal static class ChatMessageAssistant
{
	internal static BotCommand NewDummyMessage(string user,string message)
	{
		if (BotInfoAssistant.CommandParser.TryConvertCommand(
			new User(){ Name = user },
			null,
			message,
			FChatMessageType.Basic,
			out BotCommand? command
		))
			return command!;
		else
			throw new ArgumentException($"Failed to parse the following message: {message}",nameof(message));
	}
}
