using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Objects;
using FChatApi.Enums;

namespace Engine.ModuleHost.CardiApi;

public struct Mention
{
    private const string MentionFormatName = "[user]{0}[/user]";
    private const string MentionFormatNickname = "[color={1}][b]{0}[/b][/color]";
    private const string MentionFormatNicknameAndPronouns = "[color={1}][b]{0}[/b][/color][sup]{2}[/sup]";
    private const string MentionFormatNameAndPronouns = "[user]{0}[/user][sup]{1}[/sup]";
    private const string MentionFormatNameAndNickname = "[color={2}][b]{1}[/b][/color] ([user]{0}[/user])";
    private const string MentionFormatNameAndNicknameAndPronouns = "[color={2}][b]{1}[/b][/color][sup]{3}[/sup] ([user]{0}[/user])";

    public (string Raw,string Basic,string WithPronouns) Name;

    public (string Raw,string Basic,string WithPronouns) Nickname;

    public (string Raw,string Basic,string WithPronouns) NameAndNickname;

    public Mention(string name,string nickname,string pronouns,BBCodeColor nicknameColor)
    {
        Name = (name,
            string.Format(MentionFormatName,name),
            string.Format(MentionFormatNameAndPronouns,name,pronouns)
        );

        if (!string.IsNullOrWhiteSpace(nickname))
        {
            Nickname = (nickname,
                string.Format(MentionFormatNickname,nickname,nicknameColor.ToString().ToLower()),
                string.Format(MentionFormatNicknameAndPronouns,name,nicknameColor.ToString().ToLower(),pronouns)
            );
            NameAndNickname = (name,
                string.Format(MentionFormatNameAndNickname,name,nickname,nicknameColor.ToString().ToLower()),
                string.Format(MentionFormatNameAndNicknameAndPronouns,name,nickname,nicknameColor.ToString().ToLower(),pronouns)
            );
        }
        else
        {
            Nickname = Name;
            NameAndNickname = Name;
        }
    }

    public Mention(RegisteredUser user) : this(user.Name,user.Nickname,user.Pronouns,user.NickColor)
    { }

}