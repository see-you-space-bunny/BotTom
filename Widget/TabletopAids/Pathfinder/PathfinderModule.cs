using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Engine.DiceRoll.GameSystems;

namespace Widget.TabletopAids.GameSystems;

/// <summary>
/// Creates and registers a /pf global command
/// </summary>
/**
internal class PathfinderModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "pathfinder";
	internal const string Description = "Roll a d20 for Pathfinder. Adding a Difficulty Class (DC) will display degree of success";
	#endregion
	
	#region C(-)
	private static readonly CommandOption<long>		DiceModifier 		= new ("dm","The bonus or penalty you have to your roll.",null,isRequired: true);
	private static readonly CommandOption<long>		DifficultyClass = new ("dc","A field to specify the roll's Difficulty Class (DC)",null,isRequired: true);
	private static readonly CommandOption<string>	Label						= new ("label","A label to identify what the roll is for. (default: none)",null);
	private static readonly CommandOption<bool>		Private					= new ("private","Hide the result from everyone except you. (default: {0})",false);
	#endregion

	private enum Option1
	{
		Standard_DC = 0x00,
		Level_DC = 0x01,
		Spell_DC = 0x02,
	}

	internal PathfinderModule()
	{ }

	async Task IUserDefinedCommand.RegisterCommand()
	{
		var command = new SlashCommandBuilder();

		// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
		command.WithName(Name);
		command.WithDescription(Description);
		
		var standardDC = new SlashCommandOptionBuilder()
				.WithName(Option1.Standard_DC.ToString().ToLower().Replace('_','-'))
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		DiceModifier		.AddOption(standardDC);
		DifficultyClass	.AddOption(standardDC);
		Label			 			.AddOption(standardDC);
		Private		 			.AddOption(standardDC);
		
		var levelDC = new SlashCommandOptionBuilder()
				.WithName(Option1.Level_DC.ToString().ToLower().Replace('_','-'))
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		DiceModifier		.AddOption(levelDC);
		DifficultyClass	.AddOption(levelDC);
		Label			 			.AddOption(levelDC);
		Private		 			.AddOption(levelDC);
		
		var spellDC = new SlashCommandOptionBuilder()
				.WithName(Option1.Spell_DC.ToString().ToLower().Replace('_','-'))
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		DiceModifier		.AddOption(spellDC);
		DifficultyClass	.AddOption(spellDC);
		Label			 			.AddOption(spellDC);
		Private		 			.AddOption(spellDC);

		command.AddOption(standardDC);
		command.AddOption(levelDC);
		command.AddOption(spellDC);

		try
		{
				await Program.DiscordClient.Rest.CreateGlobalCommand(command.Build());
		}
		catch(HttpException exception)
		{
				var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
				Console.WriteLine(json);
		}
	}

	async Task IUserDefinedCommand.HandleSlashCommand(SocketSlashCommand command)
	{
		var options									= command.Data.Options.First();
		SocketGuildUser? guildUser 	= command.User as SocketGuildUser;

		Option1 mode 		= Enum.Parse<Option1>(options.Name.Replace('-','_'),true);
		int dm					= (int)DiceModifier			.GetValue(options);
		int dc					= (int)DifficultyClass	.GetValue(options);
		string label		= Label.GetValue(options) ?? string.Empty;
		bool ephemeral	= Private.GetValue(options);

		var dcType = mode switch
			{
					Option1.Level_DC		=> PathfinderRoll.DifficultyClassType.Level,
					Option1.Spell_DC		=> PathfinderRoll.DifficultyClassType.SpellLevel,
					Option1.Standard_DC	=> PathfinderRoll.DifficultyClassType.Basic,
					_       						=> PathfinderRoll.DifficultyClassType.None,
		};
		var p2e_r = new PathfinderRoll(dm,new (dcType, dc),null);
		p2e_r.Roll();

		var embedBuilder = new EmbedBuilder();

		if (guildUser != null)
		{
			embedBuilder.WithAuthor(
				guildUser.DisplayName,
				guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl());
		}
		else
			embedBuilder.WithAuthor(
				command.User.GlobalName,
				command.User.GetAvatarUrl() ?? command.User.GetDefaultAvatarUrl());

		if (label != string.Empty)
			embedBuilder.WithTitle(label);
		
		embedBuilder.WithColor(guildUser != null ? guildUser.Roles.First().Color : Color.Default);

		embedBuilder.WithDescription(p2e_r.ToString());

		embedBuilder.WithCurrentTimestamp();

		await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: ephemeral);
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	bool IUserDefinedCommand.IsGlobal => true;
	ulong IUserDefinedCommand.Guild => 0;
}
*/