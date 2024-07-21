using FChatApi.Objects;

namespace Plugins.Tokenizer;

public class CommandTokens(FChatMessage fChatMessage, string botModule, string moduleCommand, string[] parameters)
{
	public FChatMessage Message { get; } = fChatMessage;
	public string BotModule { get; } = botModule;
	public string ModuleCommand { get; } = moduleCommand;
	public string[] Parameters { get; } = parameters;
	public bool TryParseModule<TModule>(out TModule value) where TModule : struct => Enum.TryParse(BotModule,true,out value);
	public bool TryParseCommand<TCommand>(out TCommand value) where TCommand : struct => Enum.TryParse(ModuleCommand,true,out value);
}

