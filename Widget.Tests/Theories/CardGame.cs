using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Widget.CardGame;
using Widget.CardGame.Attributes;
using Widget.CardGame.Enums;

namespace Widget.Tests.Theories;

public class @CardGame
{
    [Theory]
    [InlineData("Daniel",           "tom!xcg challenge STR INT [user]The Cooler Daniel[/user]",
                "The Cooler Daniel","tom!xcg accept STR LUC")]
    //[InlineData("Daniel",           "[noparse=tom!xcg challenge STR INT [user]The Cooler Daniel[/user]][/noparse]",
    //          "The Cooler Daniel","tom!xcg accept STR LUC")]
    public void TestCommand(string player1,string msgChallenge,string player2,string msgResponse)
    {
        ApiConnection.CharacterName = BotInfo.BotName;

        var tournamentOrganiser = new FChatTournamentOrganiser(null);

        var command1 = ChatMessageAssistant.NewDummyMessage(player1,msgChallenge);
        tournamentOrganiser.HandleRecievedMessage(command1!);

        Assert.Matches(".+"+player1+".+"+player2+".+",tournamentOrganiser.MostRecentMessage.Build().Message);
        Assert.NotEmpty(tournamentOrganiser.IncomingChallenges);

        var command2 = ChatMessageAssistant.NewDummyMessage(player2,msgResponse);
        tournamentOrganiser.HandleRecievedMessage(command2!);
        
        Assert.Matches(".+"+player2+".+"+player1+".+",tournamentOrganiser.MostRecentMessage.Build().Message);
        Assert.NotEmpty(tournamentOrganiser.OngoingMatches);
    }
}