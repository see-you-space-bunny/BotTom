using Engine.ModuleHost.Enums;
using FChatApi.Tokenizer;

namespace Engine.ModuleHost;

/// <summary>
/// This is our main bot interface
/// </summary>
public partial class ChatBot
{
	/// <summary>
	/// Handles when we receive a message from the chat server
	/// </summary>
	/// <param name="command">command being sent, if any</param>
	public void HandleMessage(BotCommand command)
	{
		if (!command.TryParseModule(out BotModule module))
			module = BotModule.System;
		
		FChatPlugins[module].HandleRecievedMessage(command);
	}
}