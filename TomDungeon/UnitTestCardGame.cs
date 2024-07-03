using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Widget.CardGame;
using Widget.CardGame.Attributes;
using Widget.CardGame.Enums;

namespace TomDungeon;

public class UnitTestCardGame
{
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
    [InlineData("Daniel",           "[noparse=tom!xcg challenge STR INT [user]The Cooler Daniel[/user]][/noparse]",
                "The Cooler Daniel","tom!xcg accept STR LUC")]
    public async void TestCommand(string player1,string msgChallenge,string player2,string msgResponse)
    {
        var message1 = new BotTom.CardiApi.ChatMessage(
            author: player1,
            recipient: "Bot Tom",
            messageType: BotTom.CardiApi.MessageType.Basic,
            channel: "",
            message: msgChallenge
        );
        var message2 = new BotTom.CardiApi.ChatMessage(
            author: player2,
            recipient: "Bot Tom",
            messageType: BotTom.CardiApi.MessageType.Basic,
            channel: "",
            message: msgResponse
        );

        await TournamentOrganiser.HandleCommand(message1);

        Assert.NotEmpty(TournamentOrganiser.MessageQueue);
        Assert.Matches(
            @"\[user\]([a-zA-Z0-9\-\ ]{3,32})\[\/user\]\s.+\[user\]([a-zA-Z0-9\-\ ]{3,32})\[\/user\]\s.+",
            TournamentOrganiser.MessageQueue.First().Message
        );
        TournamentOrganiser.MessageQueue.Clear();

        Assert.NotEmpty(TournamentOrganiser.IncomingChallenges);

        await TournamentOrganiser.HandleCommand(message2);
        
        Assert.NotEmpty(TournamentOrganiser.MessageQueue);
        Assert.Matches(
            @"(\[user\])([a-zA-Z0-9\-\ ]{3,32})(\[\/user\]).+(\[user\])([a-zA-Z0-9\-\ ]{3,32})(\[\/user\]\').+",
            TournamentOrganiser.MessageQueue.First().Message
        );

        Assert.NotEmpty(TournamentOrganiser.OngoingMatches);
    }
}