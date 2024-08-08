using FChatApi.Objects;
using FChatApi.Enums;
using Plugins.Tokenizer;

namespace Widget.Tests.LabAssistant;

internal static class BotInfoAssistant
{
	internal const string BotName = "ApiUser";
	internal const string CommandChar = "?";
	internal static readonly User ApiUser = new (){ Name = BotName, PrivilegeLevel = Privilege.OwnerOperator};
	internal static readonly CommandParser CommandParser = new (CommandChar);
	
}
