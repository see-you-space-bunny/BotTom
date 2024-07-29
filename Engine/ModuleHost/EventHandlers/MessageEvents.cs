using Engine.ModuleHost.Enums;
using Plugins.Tokenizer;

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
	public void HandleMessage(CommandTokens command)
	{
		FChatPlugins[module].HandleRecievedMessage(command);
	}
}