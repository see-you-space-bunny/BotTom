using System.Text;

using FChatApi.Objects;
using FChatApi.Enums;
using Plugins.Tokenizer;

using ModularPlugins;

using CardGame.Enums;
using CardGame.Commands;

namespace CardGame;

public partial class FChatTournamentOrganiser : FChatPlugin<CardGameCommand>
{
	private void ImportStats(CommandTokens commandTokens,FChatMessageBuilder response)
	{
		if (commandTokens.Parameters.TryGetAs("Level",out int level))
			PlayerCharacters[commandTokens.Source.Author].Stats.AddOrUpdate(CharacterStat.LVL,level);

		if (commandTokens.Parameters.TryGetAs("STR",out int str))
			PlayerCharacters[commandTokens.Source.Author].Stats.AddOrUpdate(CharacterStat.STR,str);
		if (commandTokens.Parameters.TryGetAs("VIT",out int vit))
			PlayerCharacters[commandTokens.Source.Author].Stats.AddOrUpdate(CharacterStat.VIT,vit);
		if (commandTokens.Parameters.TryGetAs("DEX",out int dex))
			PlayerCharacters[commandTokens.Source.Author].Stats.AddOrUpdate(CharacterStat.DEX,dex);
			
		if (commandTokens.Parameters.TryGetAs("INT",out int @int))
			PlayerCharacters[commandTokens.Source.Author].Stats.AddOrUpdate(CharacterStat.INT,@int);
		if (commandTokens.Parameters.TryGetAs("CHA",out int cha))
			PlayerCharacters[commandTokens.Source.Author].Stats.AddOrUpdate(CharacterStat.CHA,cha);
		if (commandTokens.Parameters.TryGetAs("LUC",out int luc))
			PlayerCharacters[commandTokens.Source.Author].Stats.AddOrUpdate(CharacterStat.LUC,luc);
		
		//////////
			
		var responseBuilder = new StringBuilder()
			.AppendLine("You have successfully imported your stats as the following: [spoiler]")
			.Append(PlayerCharacters[commandTokens.Source.Author].GetFormattedStatBlock())
			.Append("[/spoiler]");
		
		//////////

		response
			.WithMessage(responseBuilder.ToString())
			.WithMessageType(FChatMessageType.Whisper);
	}
}