using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotTom.DiceRoller.GameSystems;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace BotTom.Commands.Global;

/// <summary>
/// Creates and registers a /pf global command
/// </summary>
internal class PathfinderModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "pf";
	internal const string Description = "Roll a d20 for Pathfinder. Adding a Difficulty Class (DC) will display degree of success";
	#endregion
	
	#region C(-)
	private static readonly CommandOption<long>		DiceModifier 		= new ("dm","The bonus or penalty you have to your roll.",null,isRequired: true);
	private static readonly CommandOption<long>		DifficultyClass = new ("dc","An optional field to specify the roll's Difficulty Class (DC)",null);
	private static readonly CommandOption<string>	SpecialDC				= new ("specialdc","An optional field to set the Difficulty Class (DC) from \"level\" or \"spell\".",null);
	private static readonly CommandOption<string>	Label						= new ("label","A label to identify what the roll is for. (default: none)",null);
	private static readonly CommandOption<bool>		Private					= new ("private","Hide the result from everyone except you. (default: {0})",false);
	#endregion

	internal PathfinderModule()
	{ }

	private static Tuple<PathfinderRoll.DifficultyClassType,long?>? GetDifficultyClassTup(long? difficultyClass, string? specialDC)
	{

		Tuple<PathfinderRoll.DifficultyClassType,long?>? difficultyClassTup;

		if (difficultyClass == null)
		{
			difficultyClassTup = null;
		}
		else if (!(specialDC == null))
		{
			var difficultyClassType = specialDC switch
			{
					"level" => PathfinderRoll.DifficultyClassType.Level,
					"spell" => PathfinderRoll.DifficultyClassType.SpellLevel,
					_       => PathfinderRoll.DifficultyClassType.None,
			};
			difficultyClassTup = new Tuple<PathfinderRoll.DifficultyClassType,long?> (difficultyClassType, difficultyClass);
		}
		else
		{
			difficultyClassTup = new Tuple<PathfinderRoll.DifficultyClassType,long?> (PathfinderRoll.DifficultyClassType.Basic, difficultyClass);
		}
		return difficultyClassTup;
	}

	async Task IUserDefinedCommand.RegisterCommand()
	{
			var command = new SlashCommandBuilder();

			// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
			command.WithName(Name);
			command.WithDescription(Description);
			DiceModifier		.AddOption(command);
			DifficultyClass	.AddOption(command);
			SpecialDC				.AddOption(command);
			Label			 			.AddOption(command);
			Private		 			.AddOption(command);

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
		// First lets extract our variables

		var p2e_r = new PathfinderRoll(
			DiceModifier	.GetValue(command),
			DifficultyClass.Defaults(command) ? null : GetDifficultyClassTup(
				DifficultyClass.GetValue(command),
				SpecialDC			 .GetValue(command)
			),
			Label.GetValue(command)
		);
		p2e_r.Roll();

		await command.RespondAsync(p2e_r.ToString(), ephemeral: Private.GetValue(command));
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	bool IUserDefinedCommand.IsGlobal => false;
	ulong IUserDefinedCommand.Guild => 0;
}