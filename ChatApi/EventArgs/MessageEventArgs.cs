using System;

namespace ChatApi
{
    /// <summary>
    /// custom event arg as easy way to send channel info up
    /// </summary>
    /// no i don't know why I'm doing it this way
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// the clean
        /// </summary>
        public string message;

        /// <summary>
        /// DEPRECATED if it's a whisper
        /// </summary>
        public bool iswhisper;

        /// <summary>
        /// the channel
        /// </summary>
        public string channel;

        /// <summary>
        /// sending user
        /// </summary>
        public string user;

        /// <summary>
        /// advanced message type
        /// </summary>
        public MessageType messagetype;
    }
}
