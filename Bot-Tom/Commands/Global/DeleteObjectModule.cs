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
internal class DeleteObjectModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "delete";
	internal const string Description = "Delete a persistent object.";
	#endregion

	#region 
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
	private static readonly CommandOption<bool>		Private  		= new ("private","Hide the result from everyone except you. (default: {0})",false);
	private static readonly CommandOption<string>	DeleteLabel = new ("label","Label of the item you wish to delete.",null,isRequired: true);
	private static readonly CommandOption<string>	DeleteGroup = new ("group","Group of items you wish to delete.",null,isRequired: true);
	#endregion

	internal DeleteObjectModule()
	{ }

	async Task IUserDefinedCommand.RegisterCommand()
	{
		// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
		var command = new SlashCommandBuilder()
			.WithName(Name)
			.WithDescription(Description);

		#region Build: clocks delete
		var deleteClock = new SlashCommandOptionBuilder()
				.WithName(ClockOption1)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommandGroup);

		var deleteClockAll = new SlashCommandOptionBuilder()
				.WithName(AllOption2)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		Private			.AddOption(deleteClockAll);

		var deleteClockSingle = new SlashCommandOptionBuilder()
				.WithName(SingleOption2)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		DeleteLabel	.AddOption(deleteClockSingle);
		Private			.AddOption(deleteClockSingle);

		var deleteClockGroup = new SlashCommandOptionBuilder()
				.WithName(GroupOption2)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		DeleteGroup	.AddOption(deleteClockGroup);
		Private			.AddOption(deleteClockGroup);

		deleteClock.AddOption(deleteClockAll);
		deleteClock.AddOption(deleteClockSingle);
		deleteClock.AddOption(deleteClockGroup);

		command.AddOption(deleteClock);
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
				await HandleDeleteClockOption(command);
				ClockCabinet.Serialize();
				break;
		}
		await command.RespondAsync(string.Empty, ephemeral: Private.GetValue(command));
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998
	async Task HandleDeleteClockOption(SocketSlashCommand command)
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
		string? label			= DeleteLabel.GetValue(command);
		string? group			= DeleteGroup.GetValue(command);
		bool ephemeral 		= Private.GetValue(command);

		bool errorCase01 	 = allClocks.Count == 0;
		bool errorCase02 	 = mode==Option2.Single&&!allClocks.Values.Any((c)=>c.Group.Equals(label,StringComparison.CurrentCultureIgnoreCase));
		bool errorCase03 	 = mode==Option2.Group&&!allClocks.Values.Any((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase));
		Color errorColor = Color.DarkRed;

		if (errorCase01)
		{
			title 		= "[Error: DeleteClock01] You haven't created any clocks yet!";
			sb.AppendLine( "You can create a clock with:");
			sb.AppendLine($"`/new clock label: text faces: number` and an optional `group: text`.");
			sb.AppendLine( "Labels and Groups are case-insensitive.");
			ephemeral = true;
		}
		else
		if (errorCase02)
		{
			title 		= $"[Error: DeleteClock02] No clock with the Label \"{DeleteLabel.GetValue(options)}\" exists!";
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
			title 		= $"[Error: DeleteClock03] No Group of clocks called \"{DeleteLabel.GetValue(options)}\"!";
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
				title = $"Deleted clock with the Label '{label}'.";
				allClocks.Remove(label!.ToLower());
				break;

			case Option2.Group:
				int clocksCount = allClocks.Values.Where((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase)).ToList().Count;
				title = $"Deleted all {clocksCount} clocks in Group '{group}'.";
				foreach(Clock groupedClock in allClocks.Values.Where((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase)))
					allClocks.Remove(groupedClock.Group.ToLower());
				break;

			case Option2.All:
				title = $"Deleted all {allClocks.Count} clocks.";
				allClocks.Clear();
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

		
		embedBuilder.WithColor(errorCase01||errorCase02||errorCase03 ? errorColor : guildUser != null ? guildUser.Roles.First().Color : Color.Default);

		embedBuilder.WithDescription(sb.ToString()[..^1]);

		embedBuilder.WithCurrentTimestamp();

		await command.RespondAsync(embedBuilder.ToString(), ephemeral: ephemeral);
	}

	bool IUserDefinedCommand.IsGlobal => true;
	ulong IUserDefinedCommand.Guild => 0;
}