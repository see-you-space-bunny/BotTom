using FChatApi.Enums;
using FChatApi.Objects;
using Plugins.Tokenizer;

namespace Widget.Tests.LabAssistant;

internal static class ChatMessageAssistant
{
	internal static CommandTokens NewDummyMessage(string user,string message)
	{
		if (BotInfoAssistant.CommandParser.TryConvertCommand(
			new FChatMessageBuilder()
				.WithRecipient(new User() { Name = BotInfoAssistant.BotName })
				.WithAuthor(new User() { Name = user })
				.WithMessage(message)
				.Build(),
			out CommandTokens? command
		))
			return command!;
		else
			throw new ArgumentException($"Failed to parse the following message: {message}",nameof(message));
	}
}
