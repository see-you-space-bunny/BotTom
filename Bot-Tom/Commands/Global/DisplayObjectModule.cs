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
internal class DisplayObjectModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "update";
	internal const string Description = "Display your selected persistent item.";
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
	private const string GroupOption2		= "group";
	private const string AllOption2			= "all";
	#endregion

	#region C(-)
	private static readonly CommandOption<bool>		Private	= new ("private","Hide the result from everyone except you. (default: {0})",false);
	private static readonly CommandOption<string>	Label		= new ("label","Label of the clock you wish to display.",null,isRequired: true);
	private static readonly CommandOption<string>	Group		= new ("group","Group of clocks you wish to display.",null,isRequired: true);
	#endregion

	internal DisplayObjectModule()
	{ }

	async Task IUserDefinedCommand.RegisterCommand()
	{
		// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
		var command = new SlashCommandBuilder()
			.WithName(Name)
			.WithDescription(Description);

		#region Build: clocks display
		var clockDisplay = new SlashCommandOptionBuilder()
				.WithName(ClockOption1)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommandGroup);

		var clockDisplayAll = new SlashCommandOptionBuilder()
				.WithName(AllOption2)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		Private	.AddOption(clockDisplayAll);

		var clockDisplaySingle = new SlashCommandOptionBuilder()
				.WithName(SingleOption2)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		Label	.AddOption(clockDisplaySingle);
		Private						.AddOption(clockDisplaySingle);

		var clockDisplayGroup = new SlashCommandOptionBuilder()
				.WithName(GroupOption2)
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		Group	.AddOption(clockDisplayGroup);
		Private						.AddOption(clockDisplayGroup);
		
		clockDisplay.AddOption(clockDisplayAll);
		clockDisplay.AddOption(clockDisplaySingle);
		clockDisplay.AddOption(clockDisplayGroup);
		command.AddOption(clockDisplay);
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
				await HandleDisplayClockOption(command);
				ClockCabinet.Serialize();
				break;
		}
		await command.RespondAsync(string.Empty, ephemeral: Private.GetValue(command));
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	async Task HandleDisplayClockOption(SocketSlashCommand command)
	{
		var options									= command.Data.Options.First().Options.First();
		SocketGuildUser? guildUser 	= command.User as SocketGuildUser;
		Option2 mode	= options.Name switch {
			SingleOption2 => Option2.Single,
			GroupOption2 => Option2.Group,
			_ => Option2.All,
		};

		string title 			= string.Empty;
		StringBuilder sb 	= new ();
		string label 			= Label.GetValue(command)!;
		string group 			= Group.GetValue(command)!;
		bool ephemeral 		= Private.GetValue(command);

		Dictionary<string,Clock> allClocks = ClockCabinet.Clocks[command.User.Id];
		bool errorCase01 	 = allClocks.Count == 0;
		Color errorColor = Color.DarkRed;

		if (errorCase01)
		{
			title 		= "[Error: DisplayClock01] You haven't created any clocks yet!";
			sb.AppendLine( "You can create a clock with:");
			sb.AppendLine($"`/new clock label: text faces: number` and an optional `group: text`.");
			sb.AppendLine( "Labels and Groups are case-insensitive.");
			ephemeral = true;
		}
		else
		switch(mode)
		{
			case Option2.Single:
				Clock singleClock = allClocks[label];
				sb.AppendLine(singleClock.ToString());
				break;

			case Option2.Group:
				title = $"Displaying all '{group}' clocks:";
				foreach(Clock groupedClock in allClocks.Values.Where((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase)))
					sb.AppendLine($"- {groupedClock.ToString()}");
				break;

			case Option2.All:
				title = "Displaying all clocks:";
				foreach(Clock clock in allClocks.Values)
					sb.AppendLine($"- {clock.ToString()}");
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