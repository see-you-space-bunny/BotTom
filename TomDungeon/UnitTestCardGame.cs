using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Widget.CardGame;
using Widget.CardGame.Attributes;
using Widget.CardGame.Enums;
using ModuleHost.CommandHandling;
using ChatApi.Core;

namespace TomDungeon;

public class UnitTestCardGame
{
    const string BotName = "Bot Tom";
    const string CommandChar = "tom!";
    CommandParser CommandParser = new CommandParser(CommandChar,[]);

    [Fact]
    public void TestAttributeHandler()
    {
        StatAliasAttribute statAliasLevel = CharacterStat.LVL.GetAttribute<CharacterStat, StatAliasAttribute>();
        Assert.Equal(["Level","Lv"],statAliasLevel.Alias);

        StatAliasAttribute statAliasLuck = CharacterStat.LUC.GetEnumAttribute<CharacterStat, StatAliasAttribute>();
        Assert.Equal(["Luck"],statAliasLuck.Alias);
    }

    
    [Theory]
    [InlineData("Daniel",           "tom!xcg challenge STR INT [user]The Cooler Daniel[/user]",
                "The Cooler Daniel","tom!xcg accept STR LUC")]
    //[InlineData("Daniel",           "[noparse=tom!xcg challenge STR INT [user]The Cooler Daniel[/user]][/noparse]",
    //          "The Cooler Daniel","tom!xcg accept STR LUC")]
    public async void TestCommand(string player1,string msgChallenge,string player2,string msgResponse)
    {
        ApiConnection.CharacterName = BotName;

        var tournamentOrganiser = new FChatTournamentOrganiser(null,CommandChar,null,null);

        var command1 = NewDummyMessage(player1,msgChallenge);
        await tournamentOrganiser.HandleRecievedMessage(command1!);

        Assert.Matches(".+"+player1+".+"+player2+".+",tournamentOrganiser.MostRecentMessage.Build().Message);
        Assert.NotEmpty(tournamentOrganiser.IncomingChallenges);

        var command2 = NewDummyMessage(player2,msgResponse);
        await tournamentOrganiser.HandleRecievedMessage(command2!);
        
        Assert.Matches(".+"+player2+".+"+player1+".+",tournamentOrganiser.MostRecentMessage.Build().Message);
        Assert.NotEmpty(tournamentOrganiser.OngoingMatches);
    }


    private BotCommand NewDummyMessage(string user,string message)
    {
        if (CommandParser.TryConvertCommand(
            new ChatApi.Objects.User(){ Name = user },
            null,
            message,
            out BotCommand? command
        ))
            return command!;
        else
            throw new ArgumentException($"Failed to parse the following message: {message}",nameof(message));
    }
}