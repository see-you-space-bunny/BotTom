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
/// Creates and registers a /pf guild command
/// </summary>
internal class PathfinderModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "pathfinder";
	#endregion

	internal PathfinderModule()
	{ }

	async Task IUserDefinedCommand.RegisterCommand()
	{
			var command = new SlashCommandBuilder();

			// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
			command.WithName(Name);
			command.WithDescription("Roll a d20 for Pathfinder. Adding a Difficulty Class (DC) will display degree of success");
			command.AddOption("dm", 			 ApplicationCommandOptionType.Number,"The bonus or penalty you have to your roll.", isRequired: true);
			command.AddOption("dc", 			 ApplicationCommandOptionType.Number, "An optional field to specify the roll's Difficulty Class (DC)", isRequired: false);
			command.AddOption("specialdc", ApplicationCommandOptionType.String, "An optional field to set the Difficulty Class (DC) from \"level\" or \"spell\".", isRequired: false);
			command.AddOption("label", 		 ApplicationCommandOptionType.String, "An optional label to identify what the roll is for.", isRequired: false);

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
		
		long diceModifier = Convert.ToInt64(command.Data.Options.First((o)=>o.Name=="dm").Value);

		var __difficultyClass = command.Data.Options.FirstOrDefault((o)=>o!.Name=="dc", null);
		long? difficultyClass = __difficultyClass != null ? Convert.ToInt64(__difficultyClass.Value) : null;

		var __specialDC = command.Data.Options.FirstOrDefault((o)=>o!.Name=="specialdc", null);
		string? specialDC = __specialDC != null ? ((string)__specialDC.Value).Trim() : null;

		var __label = command.Data.Options.FirstOrDefault((o)=>o!.Name=="label", null);
		string? label = __label != null ? (string)__label.Value : null;

		Tuple<PathfinderRoll.DifficultyClassType,long?>? difficultyClassTup;

		if (difficultyClass == null)
		{
			difficultyClassTup = null;
		}
		else if (!(specialDC == null))
		{
			PathfinderRoll.DifficultyClassType difficultyClassType;
			switch(specialDC)
			{
				case "level":
					difficultyClassType = PathfinderRoll.DifficultyClassType.Level;
					break;
				case "spell":
					difficultyClassType = PathfinderRoll.DifficultyClassType.SpellLevel;
					break;
				default:
					difficultyClassType = PathfinderRoll.DifficultyClassType.None;
					break;
			}
			difficultyClassTup = new Tuple<PathfinderRoll.DifficultyClassType,long?> (difficultyClassType, difficultyClass);
		}
		else
		{
			difficultyClassTup = new Tuple<PathfinderRoll.DifficultyClassType,long?> (PathfinderRoll.DifficultyClassType.Basic, difficultyClass);
		}

		var p2e_r = new PathfinderRoll(diceModifier, difficultyClassTup, label);
		p2e_r.Roll();

		await command.RespondAsync(p2e_r.ToString());
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	bool IUserDefinedCommand.IsGlobal => false;
	ulong IUserDefinedCommand.Guild => 0;
}