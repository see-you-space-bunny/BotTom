namespace ChatApi
{
    public partial class ApiConnection
    {
        public delegate void RemoteMessageHandler(object sender, MessageEventArgs e);
        public RemoteMessageHandler MessageHandler;

        public delegate void RemoteChannelHandler(object sender, ChannelEventArgs e);
        public RemoteChannelHandler JoinedChannelHandler;
        public RemoteChannelHandler CreatedChannelHandler;
        public RemoteChannelHandler LeftChannelHandler;
        public RemoteChannelHandler PublicChannelsReceivedHandler;
        public RemoteChannelHandler PrivateChannelsReceivedHandler;
        public RemoteChannelHandler ConnectedToChat;
    }
}
