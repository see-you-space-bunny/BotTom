using System.Collections.Generic;
using System.Linq;

namespace ChatApi.Objects
{
    public class User
    {
        public string name;
        public string nickname;
        public string memo;
        public string gender;

        public UserStatus userstatus;
        public ChatStatus chatstatus;

        Dictionary<KinkPreference, List<string>> kinks;

        public User()
        {
            name = string.Empty;
            nickname = string.Empty;
            memo = string.Empty;
            gender = string.Empty;

            userstatus = UserStatus.None;
            chatstatus = ChatStatus.None;

            kinks = [];
        }

        public List<string> GetKinks(KinkPreference preference)
        {
            return [.. kinks[preference]];
        }
    }
}
