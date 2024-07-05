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
            if (KnownUsers.TryAdd(user.Name, user))
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
            return KnownUsers.Where(user => user.Value.UserStatus == status);
        }

        public User GetUserByName(string name)
        {
            if (KnownUsers.TryGetValue(name, out User value))
            {
                return value;
            }

            User foundUser = new()
            {
                Name = name,
                ChatStatus = ChatStatus.Offline
            };
            AddUser(foundUser);

            return foundUser;
        }

        public void SetChatStatus(User user, ChatStatus status, bool logging = true)
        {
            user.ChatStatus = status;
            if (GetUserByName(user.Name) == null)
                AddUser(user);

            User thisUser = KnownUsers[user.Name];
            if (null == thisUser)
                throw new System.Exception($"Error attempting to resolve user: {user.Name}.");

            thisUser.ChatStatus = status;
            if (logging) Console.WriteLine($"{thisUser.Name}'s chat status changed to: {status}");
        }

        public void SetUserStatus(User user, UserStatus status, bool logging = true)
        {
            user.UserStatus = status;
            if (GetUserByName(user.Name) == null)
                AddUser(user);

            User thisUser = KnownUsers[user.Name];
            if (null == thisUser)
                throw new System.Exception($"Error attempting to resolve user: {user.Name}.");

            thisUser.UserStatus = status;
            if (logging) Console.WriteLine($"{thisUser.Name}'s user status changed to: {status}");
        }
    }
}
