using Xunit.Abstractions;

using FChatApi.Core;

using CardGame;

namespace Widget.Tests.Theories;

public class @CardGame(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;
	
	[Theory]
	[InlineData("Daniel",	"tom!xcg challenge STR INT [user]The Cooler Daniel[/user]",
							"The Cooler Daniel","tom!xcg accept VIT LUC")]
	//[InlineData("Daniel",	"[noparse=tom!xcg challenge DEX CHA [user]The Cooler Daniel[/user]][/noparse]",
	//						"The Cooler Daniel","tom!xcg accept VIT LUC")]
	public void TestAccept(string player1,string msgChallenge,string player2,string msgResponse)
	{
		ApiConnection.DebugSetCharacterName(BotInfoAssistant.BotName);
		ApiConnection.DebugAddCharacters([BotInfoAssistant.BotName,player1,player2]);

		var tournamentOrganiser = new FChatTournamentOrganiser(new ApiConnection(),TimeSpan.MaxValue);

		var command1 = ChatMessageAssistant.NewDummyMessage(player1,msgChallenge);
		tournamentOrganiser.HandleRecievedMessage(command1!);
		// TODO: improve tokenizer to properl capture user names
		Assert.Matches(".+"+player1+".+"+player2+".+",tournamentOrganiser.MostRecentMessage.Build().Message);
		Assert.NotEmpty(tournamentOrganiser.IncomingChallenges);

		var command2 = ChatMessageAssistant.NewDummyMessage(player2,msgResponse);
		tournamentOrganiser.HandleRecievedMessage(command2!);
		
		Assert.Matches(".+"+player2+".+"+player1+".+",tournamentOrganiser.MostRecentMessage.Build().Message);
		Assert.Empty(tournamentOrganiser.IncomingChallenges);
		Assert.NotEmpty(tournamentOrganiser.OngoingMatches);
	}

	[Theory]
	[InlineData("Daniel",	"[noparse=tom!xcg challenge DEX CHA [user]The Cooler Daniel[/user]][/noparse]",
							"The Cooler Daniel","tom!xcg reject")]
	//[InlineData("Daniel",	"[noparse=tom!xcg challenge DEX CHA [user]The Cooler Daniel[/user]][/noparse]",
	//						"The Cooler Daniel","tom!xcg reject")]
	public void TestReject(string player1,string msgChallenge,string player2,string msgResponse)
	{
		ApiConnection.DebugSetCharacterName(BotInfoAssistant.BotName);
		ApiConnection.DebugAddCharacters([BotInfoAssistant.BotName,player1,player2]);

		var tournamentOrganiser = new FChatTournamentOrganiser(new ApiConnection(),TimeSpan.MaxValue);

		var command1 = ChatMessageAssistant.NewDummyMessage(player1,msgChallenge);
		tournamentOrganiser.HandleRecievedMessage(command1!);
		// TODO: improve tokenizer to properl capture user names
		Assert.Matches(".+"+player1+".+",tournamentOrganiser.MostRecentMessage.Build().Message);
		Assert.NotEmpty(tournamentOrganiser.IncomingChallenges);

		var command2 = ChatMessageAssistant.NewDummyMessage(player2,msgResponse);
		tournamentOrganiser.HandleRecievedMessage(command2!);
		
		Assert.Matches(".+"+player2+".+",tournamentOrganiser.MostRecentMessage.Build().Message);
		Assert.Empty(tournamentOrganiser.IncomingChallenges);
		Assert.Empty(tournamentOrganiser.OngoingMatches);
	}
}