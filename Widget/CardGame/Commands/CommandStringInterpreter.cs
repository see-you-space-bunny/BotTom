using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Widget.CardGame.Commands;

internal partial class CommandStringInterpreter
{

    internal const string StartOfCommand            = @"(^|(\[noparse=))";
    internal const string OptionSeparator           = @"\s+";
    internal const string EndOfCommand              = @"(\s+|\]|$)";

    internal const string ThisRequiredNextRequired  = @")"+OptionSeparator+@"){1}";
    internal const string ThisRequiredNextOptional  = @")"+EndOfCommand+@"){1}";
    internal const string ThisOptionalNextRequired  = @")"+OptionSeparator+@"){0,1}";
    internal const string ThisOptionalNextOptional  = @")"+EndOfCommand+@"){0,1}";

    public const string BotIdentifier               = @"tom!";
    public const string ModuleKey                   = @"xcg";

    internal const string OptionLengthMinCommand    = @"1";
    internal const string OptionLengthMinWord       = @"3";
    internal const string OptionLengthMinInteger    = @"1";
    internal const string OptionLengthMinFloat1     = @"1";
    internal const string OptionLengthMinFloat2     = @"1";
    internal const string OptionLengthMinPlayer     = @"3";

    internal const string OptionLengthMaxCommand    = @"24";
    internal const string OptionLengthMaxWord       = @"24";
    internal const string OptionLengthMaxInteger    = @"7";
    internal const string OptionLengthMaxFloat1     = @"7";
    internal const string OptionLengthMaxFloat2     = @"7";
    internal const string OptionLengthMaxPlayer     = @"32";

    internal const string OptionLengthCommand       = @"{"+OptionLengthMinCommand   +@","+OptionLengthMaxCommand+@"}";
    internal const string OptionLengthWord          = @"{"+OptionLengthMinWord      +@","+OptionLengthMaxWord   +@"}";
    internal const string OptionLengthInteger       = @"{"+OptionLengthMinInteger   +@","+OptionLengthMaxInteger+@"}";
    internal const string OptionLengthFloat1        = @"{"+OptionLengthMinFloat1    +@","+OptionLengthMaxFloat1 +@"}";
    internal const string OptionLengthFloat2        = @"{"+OptionLengthMinFloat2    +@","+OptionLengthMaxFloat2 +@"}";
    internal const string OptionLengthPlayer        = @"{"+OptionLengthMinPlayer    +@","+OptionLengthMaxPlayer +@"}";


    internal const string Command                   = @"Command";
    internal const string Stat1                     = @"Stat1";
    internal const string Stat2                     = @"Stat2";
    internal const string Amount                    = @"Amount";
    internal const string Player                    = @"Player";

    internal const string NamedPatternStart         = @"((?'";
    internal const string NamedCommandPattern       = @"'[a-zA-Z]"  + OptionLengthCommand;
    internal const string NamedWordPattern          = @"'[a-zA-Z]"  + OptionLengthWord;
    internal const string NamedIntegerPattern       = @"'[0-9]"     + OptionLengthInteger;

    internal const string CommandPattern            = NamedPatternStart + Command + NamedCommandPattern;
    internal const string OptionStat1Pattern        = NamedPatternStart + Stat1   + NamedWordPattern;
    internal const string OptionStat2Pattern        = NamedPatternStart + Stat2   + NamedWordPattern;
    internal const string OptionAmountPattern       = NamedPatternStart + Amount  + NamedIntegerPattern;

    internal const string UserPattern               = @"\[user\](((?'"+Player+@"'[a-zA-Z0-9\-\ ]"+OptionLengthPlayer+@")\[\/user\]";

    [GeneratedRegex(
        StartOfCommand      + BotIdentifier +
        ModuleKey           + OptionSeparator +
        CommandPattern      + ThisRequiredNextOptional)]
    internal static partial Regex RegexMatchAnyCommand();

    [GeneratedRegex(
        StartOfCommand      + BotIdentifier +
        ModuleKey           + OptionSeparator +
        CommandPattern      + ThisRequiredNextRequired +
        OptionStat1Pattern  + ThisRequiredNextOptional +
        OptionStat2Pattern  + ThisOptionalNextRequired +
        UserPattern         + ThisRequiredNextOptional)]
    internal static partial Regex RegexMatchChallenge();

    [GeneratedRegex(
        StartOfCommand      + BotIdentifier +
        ModuleKey           + OptionSeparator +
        CommandPattern      + ThisRequiredNextRequired +
        OptionStat1Pattern  + ThisRequiredNextOptional +
        OptionStat2Pattern  + ThisOptionalNextOptional)]
    internal static partial Regex RegexMatchAcceptChallenge();

    [GeneratedRegex(
        StartOfCommand      + BotIdentifier +
        ModuleKey           + OptionSeparator +
        CommandPattern      + ThisRequiredNextRequired +
        OptionStat1Pattern  + ThisRequiredNextRequired +
        OptionAmountPattern + ThisRequiredNextOptional)]
    internal static partial Regex RegexMatchSetStat();
}