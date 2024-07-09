using FChatApi.EventArguments;
using FChatApi.Tokenizer;

namespace ModularPlugins;

public interface IFChatPlugin
{
	public abstract DateTime NextUpdate { get; }
	public abstract int ModuleType { get; }
	public abstract void HandleRecievedMessage(BotCommand command);
	public abstract void HandleJoinedChannel(ChannelEventArgs @event);
	public abstract void Update();
    public abstract void Shutdown();
}
