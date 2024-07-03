using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApi.Objects
{
    public class Channel
    {
        /// <summary>
        /// what is our status with the channel
        /// </summary>
        public ChannelStatus Status;

        /// <summary>
        /// the channel
        /// </summary>
        public string Name;

        public User Owner;

        /// <summary>
        /// sending user
        /// </summary>
        public string Code;

        public ChannelType Type;

        public bool AdEnabled;

        internal Dictionary<string,User> Mods;

        internal Dictionary<string,User> Users;

        public bool CreatedByApi;

        public string Description;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <param name="adenabled"></param>
        public Channel(string name, string code, ChannelType type, bool adenabled = false)
        {
            Name = name;
            Code = code;
            Status = ChannelStatus.Available;
            Type = type;
            AdEnabled = adenabled;
            Mods = [];
            Users = [];
            CreatedByApi = false;
            Description = string.Empty;
        }

        public void AddUser(User user)
        {
            if (!Users.TryAdd(user.name, user))
                Console.WriteLine($"Skipping duplicate entry: {user.name}");
        }

        public void RemoveUser(User user)
        {
            Users.Remove(user.name);
        }

        public void AddMod(User user)
        {
            if (!Users.TryAdd(user.name, user))
                Console.WriteLine($"Skipping duplicate mod entry: {user.name}");
        }

        public void RemoveMod(User user)
        {
            Mods.Remove(user.name);
        }
        ////////////////////////////////////////////////

        /// <summary>
        /// 
        /// </summary>
        private Channel()
        {

        }
    }
}