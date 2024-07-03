using ChatApi.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatApi.Systems
{
    public class UserTracker
    {
        readonly Dictionary<string,User> KnownUsers;

        public UserTracker()
        {
            KnownUsers = [];
        }

        public bool AddUser(User user)
        {
            if (KnownUsers.TryAdd(user.name, user))
            {
                return true;
            }

            return false;
        }

        public int GetNumberActiveUsers()
        {
            return KnownUsers.Count;
        }

        public IEnumerable<KeyValuePair<string,User>> GetUsersByStatus(UserStatus status)
        {
            return KnownUsers.Where(user => user.Value.userstatus == status);
        }

        public User GetUserByName(string name)
        {
            if (KnownUsers.TryGetValue(name, out User value))
            {
                return value;
            }

            User foundUser = new()
            {
                name = name,
                chatstatus = ChatStatus.Offline
            };
            AddUser(foundUser);

            return foundUser;
        }

        public void SetChatStatus(User user, ChatStatus status, bool logging = true)
        {
            user.chatstatus = status;
            if (GetUserByName(user.name) == null)
                AddUser(user);

            User thisUser = KnownUsers[user.name];
            if (null == thisUser)
                throw new System.Exception($"Error attempting to resolve user: {user.name}.");

            thisUser.chatstatus = status;
            if (logging) Console.WriteLine($"{thisUser.name}'s chat status changed to: {status}");
        }

        public void SetUserStatus(User user, UserStatus status, bool logging = true)
        {
            user.userstatus = status;
            if (GetUserByName(user.name) == null)
                AddUser(user);

            User thisUser = KnownUsers[user.name];
            if (null == thisUser)
                throw new System.Exception($"Error attempting to resolve user: {user.name}.");

            thisUser.userstatus = status;
            if (logging) Console.WriteLine($"{thisUser.name}'s user status changed to: {status}");
        }
    }
}
