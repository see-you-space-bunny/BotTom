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
internal class UpdateObjectModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "update";
	internal const string Description = "Update your selected persistent item.";
	#endregion



	#region enum
	private enum Option1
	{
		Clock = 0x00,
	}
	private const string ClockOption1	= "clock";

	private enum Option2
	{
		Single = 0x00,
		All = 0x01,
		Group = 0x02,
	}
	private const string SingleOption2	= "single";
	private const string GroupOption2	= "group";
	private const string AllOption2		= "all";
	#endregion

	#region C(-)
	private static readonly CommandOption<bool>		Private  	= new ("private","Hide the result from everyone except you. (default: {0})",false);
	private static readonly CommandOption<string>	Label 		= new ("label","Label of the clock you wish to update.",null,isRequired: true);
	private static readonly CommandOption<string>	Group 		= new ("group","Group of clocks you wish to update.",null,isRequired: true);
	private static readonly CommandOption<long>		Progress	= new ("progress","The number amount by which you wish to advance the clock.",null);
	private static readonly CommandOption<long>		Faces 		= new ("faces","The number amount by which you wish to modify the clock's faces.",null);
	#endregion

	internal UpdateObjectModule()
	{ }

	async Task IUserDefinedCommand.RegisterCommand()
	{
		// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
		var command = new SlashCommandBuilder()
			.WithName(Name)
			.WithDescription(Description);

		#region Build: clocks update
		var clockUpdate = new SlashCommandOptionBuilder()
				.WithName(ClockOption1)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommandGroup);

		var clockUpdateAll = new SlashCommandOptionBuilder()
				.WithName(AllOption2)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		Label		.AddOption(clockUpdateAll);
		Progress	.AddOption(clockUpdateAll);
		Faces		.AddOption(clockUpdateAll);
		Private							.AddOption(clockUpdateAll);

		var clockUpdateSingle = new SlashCommandOptionBuilder()
				.WithName(SingleOption2)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		Label		.AddOption(clockUpdateSingle);
		Progress	.AddOption(clockUpdateSingle);
		Faces		.AddOption(clockUpdateSingle);
		Private							.AddOption(clockUpdateSingle);

		var clockUpdateGroup = new SlashCommandOptionBuilder()
				.WithName(GroupOption2)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		Group	.AddOption(clockUpdateGroup);
		Private						.AddOption(clockUpdateGroup);

		clockUpdate.AddOption(clockUpdateAll);
		clockUpdate.AddOption(clockUpdateSingle);
		clockUpdate.AddOption(clockUpdateGroup);
		command.AddOption(clockUpdate);
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
				await HandleUpdateClockOption(command);
				ClockCabinet.Serialize();
				break;
		}
		await command.RespondAsync(string.Empty, ephemeral: Private.GetValue(command));
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	async Task HandleUpdateClockOption(SocketSlashCommand command)
	{
		var options									= command.Data.Options.First().Options.First();
		SocketGuildUser? guildUser 	= command.User as SocketGuildUser;
		Option2 mode	= options.Name switch {
			SingleOption2 => Option2.Single,
			GroupOption2 => Option2.Group,
			_ => Option2.All,
		};

		Dictionary<string,Clock> allClocks = ClockCabinet.Clocks[command.User.Id];

		string title 			= string.Empty;
		StringBuilder sb 	= new ();
		string label			= Label			.GetValue(options)!.ToLower();
		string group			= Group			.GetValue(options)!.ToLower();
		int? faces 				= Faces			.Defaults(command) ? null : (int)Faces			.GetValue(options);
		int? progress			= Progress	.Defaults(command) ? null : (int)Progress	.GetValue(options);
		bool ephemeral 		= Private		.GetValue(command);

		bool errorCase01 	 = allClocks.Count == 0;
		bool errorCase02 	 = mode==Option2.Single&&!allClocks.Values.Any((c)=>c.Group.Equals(label,StringComparison.CurrentCultureIgnoreCase));
		bool errorCase03 	 = mode==Option2.Group&&!allClocks.Values.Any((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase));
		Color errorColor = Color.DarkRed;

		if (errorCase01)
		{
			title 		= "[Error: UpdateClock01] You haven't created any clocks yet!";
			sb.AppendLine( "You can create a clock with:");
			sb.AppendLine($"`/new clock label: text faces: number` and an optional `group: text`.");
			sb.AppendLine( "Labels and Groups are case-insensitive.");
			ephemeral = true;
		}
		else
		if (errorCase02)
		{
			title 		= $"[Error: UpdateClock02] No clock with the Label \"{Label.GetValue(options)}\" exists!";
			sb.AppendLine( "You can create a clock with:");
			sb.AppendLine($"`/new clock label: text faces: number` and an optional `group: text`.");
			sb.AppendLine( "Labels and Groups are case-insensitive.");
			sb.AppendLine( "You can check your existing clocks with:");
			sb.AppendLine($"`/display clock all`");
			ephemeral = true;
		}
		else
		if (errorCase03)
		{
			title 		= $"[Error: UpdateClock03] No Group of clocks called \"{Label.GetValue(options)}\"!";
			sb.AppendLine( "You can create a clock with:");
			sb.AppendLine($"`/new clock label: text faces: number` and an optional `group: text`.");
			sb.AppendLine( "Labels and Groups are case-insensitive.");
			sb.AppendLine( "You can check your existing clocks with:");
			sb.AppendLine($"`/display clock all`");
			ephemeral = true;
		}
		else
		switch(mode)
		{
			case Option2.Single:
				title = "Updating clock.";
				if (faces != null)
					allClocks[label].AddProgress((int)faces!);
				if (progress != null)
					allClocks[label].AddProgress((int)progress!);
				sb.AppendLine($"The clock is now:");
				sb.AppendLine($"> {allClocks[label].ToString()}");
				break;

			case Option2.Group:
				title = $"Updating all '{group}' clocks.";
				if (faces != null)
					foreach(Clock groupedClock in allClocks.Values.Where((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase)))
					{
						sb.AppendLine($"Faces {(faces >= 0 ? "in" : "de")}creased by {faces}.");
						groupedClock.AddProgress((int)faces!);
					}
				if (progress != null)
					foreach(Clock groupedClock in allClocks.Values.Where((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase)))
					{
						sb.AppendLine($"Progress {(progress >= 0 ? "in" : "de")}creased by {progress}.");
						groupedClock.AddProgress((int)progress!);
					}
				break;

			case Option2.All:
				title = "Updating all clocks.";
				if (faces != null)
					foreach(Clock groupedClock in allClocks.Values)
					{
						sb.AppendLine($"Faces {(faces >= 0 ? "in" : "de")}creased by {faces}.");
						groupedClock.AddProgress((int)faces!);
					}
				if (progress != null)
					foreach(Clock groupedClock in allClocks.Values)
					{
						sb.AppendLine($"Progress {(progress >= 0 ? "in" : "de")}creased by {progress}.");
						groupedClock.AddProgress((int)progress!);
					}
				break;
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