using System;

namespace ChatApi
{
    public class ChannelEventArgs : EventArgs
    {
        /// <summary>
        /// what is our status with the channel
        /// </summary>
        public ChannelStatus status;

        /// <summary>
        /// public or private
        /// </summary>
        public ChannelType type;

        /// <summary>
        /// the channel
        /// </summary>
        public string name;

        /// <summary>
        /// sending user
        /// </summary>
        public string code;

        /// <summary>
        /// number of current users
        /// </summary>
        public int usercount;

        /// <summary>
        /// user that joined channel
        /// </summary>
        public string userJoining;
    }
}
