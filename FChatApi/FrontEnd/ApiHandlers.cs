using FChatApi.Objects;

namespace FChatApi.Core
{
	public partial class ApiConnection
	{
		public delegate void RemoteMessageHandler(object sender, FChatMessage @event);
		public RemoteMessageHandler MessageHandler;

		public delegate void RemoteChannelHandler(object sender, ChannelEventArgs @event);
		public RemoteChannelHandler JoinedChannelHandler;
		public RemoteChannelHandler CreatedChannelHandler;
		public RemoteChannelHandler LeftChannelHandler;
		public RemoteChannelHandler PublicChannelsReceivedHandler;
		public RemoteChannelHandler PrivateChannelsReceivedHandler;
		public RemoteChannelHandler ConnectedToChat;
	}
}
