using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Widget.CardGame;

namespace TomDungeon;

public class UnitTestCardGame
{
    [Fact]
    public void Test()
    {
        Assert.True(true);
    }

    
    [Theory]
    [InlineData("Daniel","tom!xcg challenge STR INT [user]The Cooler Daniel[/user]",
                "The Cooler Daniel","tom!xcg accept STR LUC")]
    [InlineData("Daniel","[noparse=tom!xcg challenge STR INT [user]The Cooler Daniel[/user]][/noparse]",
                "The Cooler Daniel","tom!xcg accept STR LUC")]
    public void TestCommand(string player1,string msgChallenge,string player2,string msgResponse)
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

        TournamentOrganiser.HandleCommand(message1);
        TournamentOrganiser.HandleActionQueue();

        Assert.Equal(
            TournamentOrganiser.MessageQueue.First().Message,
            $"[user]{player1}[/user] has challenged [user]{player2}[/user] to a [b]Duel[/b]! " +
                "Use the command \"tom!xcg accept [i]stat2[/i] [i]stat2[/i]\""
        );
        TournamentOrganiser.MessageQueue.Clear();

        TournamentOrganiser.HandleCommand(message2);
        TournamentOrganiser.HandleActionQueue();
        
        Assert.Equal(
            TournamentOrganiser.MessageQueue.First().Message,
            $"[user]{player2}[/user] accepted [user]{player1}[/user]'s challenge!"
        );
    }
}