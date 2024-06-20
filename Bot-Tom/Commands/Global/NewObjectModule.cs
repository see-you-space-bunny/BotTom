using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTom.DiceRoller.GameSystems;
using Charsheet.ForgedInTheDark;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using FileManip;
using Newtonsoft.Json;

namespace BotTom.Commands.Global;

/// <summary>
/// Creates and registers a /fitd global command
/// </summary>
internal class NewObjectModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "new";
	internal const string Description = "Create a new persistent item.";
	#endregion

	#region enum
	private enum Option1
	{
		Clock = 0x00,
	}
	private const string ClockOption1	= "clock";
	#endregion

	#region C(-)
	private static readonly CommandOption<bool>		Private  		= new ("private","Hide the result from everyone except you. (default: {0})",false);
	private static readonly CommandOption<string>	Label 			= new ("label","Label of the clock you wish to create.",null,isRequired: true);
	private static readonly CommandOption<long>		Faces 			= new ("faces","The number of faces the new clock should have.",null,isRequired: true);
	private static readonly CommandOption<string>	Group 			= new ("group","Group of clocks the new clock should belong to.",null);
	#endregion

	internal NewObjectModule()
	{ }

	async Task IUserDefinedCommand.RegisterCommand()
	{
		// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
		var command = new SlashCommandBuilder()
			.WithName(Name)
			.WithDescription(Description);
	
		#region Build: new clock
		var clockNew = new SlashCommandOptionBuilder()
			.WithName(ClockOption1)
			.WithDescription(Description);
		Label			.AddOption(clockNew);
		Faces			.AddOption(clockNew);
		Private		.AddOption(clockNew);

		command.AddOption(clockNew);
		#endregion

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
		switch(command.Data.Options.First().Name)
		{
			case ClockOption1:
				await HandleNewClockOption(command);
				ClockCabinet.Serialize();
				break;
		}
		await command.RespondAsync(string.Empty, ephemeral: Private.GetValue(command));
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	async Task HandleNewClockOption(SocketSlashCommand command)
	{
		var options									= command.Data.Options.First();
		SocketGuildUser? guildUser 	= command.User as SocketGuildUser;

		string title			= string.Empty;
		StringBuilder sb	= new ();
		string label 			= Label				.GetValue(options)!;
		string? group			= Group				.GetValue(options);
		int faces 				= (int)Faces	.GetValue(options);
		bool ephemeral		= Private			.GetValue(command);

		Dictionary<string,Clock> allClocks = ClockCabinet.Clocks[command.User.Id];
		bool errorCase01 	 = allClocks.Keys.Any((k)=>k.Equals(label,StringComparison.CurrentCultureIgnoreCase));
		Color errorColor = Color.DarkRed;

		if (errorCase01)
		{
			title 		= $"[Error: NewClock01] Failed to create clock:";
			sb.AppendLine($"A clock with the Label \"{label}\" already exists.");
			sb.AppendLine( "Labels and Groups are case-insensitive.");
			ephemeral = true;
		}
		else
		{
			allClocks.Add(label.ToLower(),new Clock(label,group ?? string.Empty,faces));
			title = $"Created new clock:";
			sb.AppendLine(allClocks[label].ToString());
		}

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

		if (title != string.Empty)
			embedBuilder.WithTitle(title);
		
		embedBuilder.WithColor(errorCase01 ? errorColor : guildUser != null ? guildUser.Roles.First().Color : Color.Default);

		embedBuilder.WithDescription(sb.ToString()[..^1]);

		embedBuilder.WithCurrentTimestamp();

		await command.RespondAsync(embedBuilder.ToString(), ephemeral: ephemeral);
	}

	bool IUserDefinedCommand.IsGlobal => true;
	ulong IUserDefinedCommand.Guild => 0;
}