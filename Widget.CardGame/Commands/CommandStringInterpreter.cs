using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Widget.CardGame.Commands;

internal partial class CommandStringInterpreter
{
    public const string BotCommandIdentifier        = "tom!";
    public const string ModuleCommandKey            = "xcg";

    internal const string CommandWord               = "CommandWord";
    internal const string Stat1                     = "Stat1";
    internal const string Stat2                     = "Stat2";
    internal const string CommandOrWordOption       = "CommandOrWordOption";
    internal const string PlayerIdentity            = "PlayerIdentity";
    internal const string Integer                   = "Integer";
    internal const string Float                     = "Float";

    internal const string CommandWordMinLength      = "1";
    internal const string CommandWordMaxLength      = "24";

    internal const string WordOptionMinLength       = "3";
    internal const string WordOptionMaxLength       = "24";

    internal const string PlayerIdentityMinLength   = "3";
    internal const string PlayerIdentityMaxLength   = "32";

    internal const string IntegerMinLength          = "1";
    internal const string IntegerMaxLength          = "7";

    internal const string FloatMinLength1          = "1";
    internal const string FloatMaxLength1          = "7";
    internal const string FloatMinLength2          = "1";
    internal const string FloatMaxLength2          = "7";

    [GeneratedRegex(@"(^|(noparse=))" + BotCommandIdentifier + ModuleCommandKey +
        @"\s+((?'" + CommandWord + @"'[a-zA-Z]{" + CommandWordMinLength + @"," + CommandWordMaxLength + @"}(\s+|\[|$))){1}"
    )]
    internal static partial Regex RegexMatchAnyCommand();

    // (^|(noparse=))tom!xcg\s+((?'CommandWord'[a-zA-Z]{1,24}\s+)){1}((?'Stat1'[a-zA-Z]{3,24}\s+)){1}((?'Stat2'[a-zA-Z]{3,24}\s+)){0,1}(\[user\](?'PlayerIdentity'[a-zA-Z0-9\-\ ]{3,32})\[\/user\](\s+|\[|$)){1}
    [GeneratedRegex(@"(^|(noparse=))" + BotCommandIdentifier + ModuleCommandKey +
        @"\s+((?'" + CommandWord + @"'[a-zA-Z]{" + CommandWordMinLength + @"," + CommandWordMaxLength + @"}\s+)){1}" +
        @"((?'" + Stat1 + @"'[a-zA-Z]{" + WordOptionMinLength + @"," + WordOptionMaxLength + @"}\s+)){1}" +
        @"((?'" + Stat2 + @"'[a-zA-Z]{" + WordOptionMinLength + @"," + WordOptionMaxLength + @"}\s+)){0,1}" +
        @"(\[user\](?'" + PlayerIdentity + @"'[a-zA-Z0-9\-\ ]{" + PlayerIdentityMinLength + @"," + PlayerIdentityMaxLength + @"})\[\/user\](\s+|\[|$)){1}"
    )]
    internal static partial Regex RegexMatchChallenge();

    [GeneratedRegex(@"(^|(noparse=))" + BotCommandIdentifier + ModuleCommandKey +
        @"\s+((?'" + CommandWord + @"'[a-zA-Z]{" + CommandWordMinLength + @"," + CommandWordMaxLength + @"}(\s+|\[|$))){1}" +
        @"((?'" + Stat1 + @"'[a-zA-Z]{" + WordOptionMinLength + @"," + WordOptionMaxLength + @"}(\s+|\[|$))){1}" +
        @"((?'" + Stat2 + @"'[a-zA-Z]{" + WordOptionMinLength + @"," + WordOptionMaxLength + @"}(\s|\[|$))){0,1}"
    )]
    internal static partial Regex RegexMatchAcceptChallenge();

    [GeneratedRegex(@"(^|(noparse=))" + BotCommandIdentifier + ModuleCommandKey +
        @"\s+((?'" + CommandWord + @"'[a-zA-Z]{" + CommandWordMinLength + @"," + CommandWordMaxLength + @"}(\s+|\[|$))){1}" +
        @"((?'" + Stat1 + @"'[a-zA-Z]{" + WordOptionMinLength + @"," + WordOptionMaxLength + @"}(\s+|\[|$))){1}" +
        @"((?'" + Integer + @"'[\+\-]{0,1}[0-9]{" + IntegerMinLength + @"," + IntegerMaxLength + @"})(\s|\[|$)){1}"
    )]
    internal static partial Regex RegexMatchSetStat();

    /**
    [GeneratedRegex(@$"(noparse=|\A)({TournamentOrganiser.BotCommandIdentifier})")]
    private static partial Regex RegexBotCommandIdentifier();

    [GeneratedRegex(@$"({TournamentOrganiser.ModuleCommandKey})")]
    private static partial Regex RegexModuleCommandKey();
    
    [GeneratedRegex(@"(\s|\A)(?'CommandOrWordOption'[a-zA-Z]{3,32})(\s|\Z)")]
    private static partial Regex RegexCommandOrWordOption();

    [GeneratedRegex(@"(\s|\A)(?'PlayerIdentity'[a-zA-Z0-9\-\ ]{3,32})(\s|\Z)")]
    private static partial Regex RegexPlayerIdentity();
    
    [GeneratedRegex(@"(\s|\A)(\[user\](?'PlayerIdentity'[a-zA-Z0-9\-\ ]{3,32})\[\/user\])(\s|\Z)")]
    private static partial Regex RegexStrictPlayerIdentity();

    [GeneratedRegex(@"(\s|^)((?'Integer'[\+\-]{0,1}[0-9]{1,7})(\s|$))")]
    private static partial Regex RegexIntegerOption();

    [GeneratedRegex(@"(\s|\A)(?'Float'[\+\-]{0,1}[0-9]{1,7}\.[0-9]{1,7})(\s|\Z)")]
    private static partial Regex RegexFloatOption();
    */
}