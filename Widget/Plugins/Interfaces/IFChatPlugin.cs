using Plugins.Tokenizer;
using FChatApi.Objects;

namespace Plugins.Interfaces;

public interface IFChatPlugin
{
	public abstract DateTime NextUpdate { get; }
	public abstract void HandleRecievedMessage(CommandTokens command);
	public abstract void HandleJoinedChannel(ChannelEventArgs @event);
	public abstract void HandleCreatedChannel(ChannelEventArgs @event);
	public abstract void Initialize();
	public abstract void Update();
    public abstract void Shutdown();
}
