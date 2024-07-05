using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatApi.Objects;

public struct Mention(string name,string nickname,string pronouns,BBCodeColor nicknameColor)
{
    private const string MentionFormatName = "[user]{0}[/user]";
    private const string MentionFormatNickname = "[color={1}][b]{0}[/b][/color]";
    private const string MentionFormatNicknameAndPronouns = "[color={1}][b]{0}[/b][/color][sup]{2}[/sup]";
    private const string MentionFormatNameAndPronouns = "[user]{0}[/user][sup]{1}[/sup]";
    private const string MentionFormatNameAndNickname = "[color={2}][b]{1}[/b][/color] ([user]{0}[/user])";
    private const string MentionFormatNameAndNicknameAndPronouns = "[color={2}][b]{1}[/b][/color][sup]{3}[/sup] ([user]{0}[/user])";

    public (string Raw,string Basic,string WithPronouns) Name = (name,
        string.Format(MentionFormatName,name),
        string.Format(MentionFormatNameAndPronouns,name,pronouns)
    );

    public (string Raw,string Basic,string WithPronouns) Nickname = (nickname,
        string.Format(MentionFormatNickname,nickname,nicknameColor),
        string.Format(MentionFormatNicknameAndPronouns,name,nicknameColor,pronouns)
    );

    public (string Raw,string Basic,string WithPronouns) NameAndNickname = (name,
        string.Format(MentionFormatNameAndNickname,name,nickname,nicknameColor),
        string.Format(MentionFormatNameAndNicknameAndPronouns,name,nickname,nicknameColor,pronouns)
    );

    public Mention(User user,BBCodeColor nicknameColor) : this(user.Name,user.Nickname,user.Pronouns,nicknameColor) { }

}