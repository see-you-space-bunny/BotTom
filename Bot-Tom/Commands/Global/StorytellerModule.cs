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
/// Creates and registers a /st global command
/// </summary>
internal class StorytellerModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "st";
	internal const string Description = "Roll d10s for Chronicles of Darkness (and other Storyteller systems).";
	#endregion

	#region C(-)
	private static readonly CommandOption<long>		DicePool		= new ("dice","Your dice pool. A pool of zero is a chance die.",null,isRequired: true);
	private static readonly CommandOption<long>		Hits				= new ("hits","The roll's difficulty rating. (default: {0})",8);
	private static readonly CommandOption<long>		NAgain			= new ("n-again","N-again threshold for exploding dice. A 0 means no result explodes. (default: {0})",10);
	private static readonly CommandOption<long>		Exceptional = new ("exceptional","How many successes are needed for an exceptional success. (default: {0})",5);
	private static readonly CommandOption<bool>		Rote				= new ("rote","If this is 'true', then the roll is treated as a rote action. (default: {0})",false);
	private static readonly CommandOption<long>		BonusHits		= new ("bonushits","How many extra successes you recieve when you roll at least one success. (default: {0})",0);
	private static readonly CommandOption<string>	Label				= new ("label","A label to identify what the roll is for. (default: none)",null);
	private static readonly CommandOption<bool>		Private			= new ("private","Hide the result from everyone except you. (default: {0})",false);
	#endregion

	internal StorytellerModule()
	{ }

	async Task IUserDefinedCommand.RegisterCommand()
	{
			var command = new SlashCommandBuilder();

			// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
			command.WithName(Name);
			command.WithDescription(Description);
			DicePool	 .AddOption(command);
			Hits			 .AddOption(command);
			NAgain		 .AddOption(command);
			Exceptional.AddOption(command);
			Rote			 .AddOption(command);
			BonusHits	 .AddOption(command);
			Label			 .AddOption(command);
			Private		 .AddOption(command);

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

		var st_r = new StorytellerRoll(
			(int)DicePool		.GetValue(command),
			(int)Hits				.GetValue(command),
			(int)NAgain			.GetValue(command),
			(int)Exceptional.GetValue(command),
			(int)BonusHits	.GetValue(command),
					 Rote				.GetValue(command),
					 Label			.GetValue(command)
		);
		st_r.Roll();

		await command.RespondAsync(st_r.ToString(), ephemeral: Private.GetValue(command));
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	bool IUserDefinedCommand.IsGlobal => false;
	ulong IUserDefinedCommand.Guild => 0;
}