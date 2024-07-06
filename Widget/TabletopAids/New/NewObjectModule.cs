using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Engine.DiceRoll.GameSystems;

namespace Widget.TabletopAids.GameSystems;

/// <summary>
/// Creates and registers a /fitd global command
/// </summary>
/**
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
	private static readonly CommandOption<bool>		Private = new ("private","Hide the result from everyone except you. (default: {0})",false);
	private static readonly CommandOption<string>	Label 	= new ("label","Label of the clock you wish to create.",null,isRequired: true);
	private static readonly CommandOption<long>		Faces 	= new ("faces","The number of faces the new clock should have.",null,isRequired: true);
	private static readonly CommandOption<string>	DescriptionOpt 	= new ("description","The description you want to assign the clock.",null);
	private static readonly CommandOption<string>	Group 	= new ("group","Group of clocks the new clock should belong to.",null);
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
			.WithName(Option1.Clock.ToString().ToLower())
			.WithDescription(Description)
			.WithType(ApplicationCommandOptionType.SubCommand);
		Label						.AddOption(clockNew);
		DescriptionOpt	.AddOption(clockNew);
		Group						.AddOption(clockNew);
		Faces						.AddOption(clockNew);
		Private					.AddOption(clockNew);

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
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	async Task HandleNewClockOption(SocketSlashCommand command)
	{
		var options									= command.Data.Options.First();
		SocketGuildUser? guildUser 	= command.User as SocketGuildUser;

		string title				= string.Empty;
		StringBuilder sb		= new ();
		string label 				= Label						.GetValue(options)!;
		string group				= Group						.GetValue(options) ?? string.Empty;
		string description	= DescriptionOpt	.GetValue(options) ?? string.Empty;
		int faces 					= (int)Faces			.GetValue(options);
		bool ephemeral			= Private					.GetValue(options);

		Dictionary<string,Clock> allClocks;
		if(ClockCabinet.Clocks.Keys.Contains(command.User.Id))
			allClocks = ClockCabinet.Clocks[command.User.Id];
		else
		{
			allClocks = [];
			ClockCabinet.Clocks.Add(command.User.Id,allClocks);
		}
		bool errorCase01	= allClocks.Keys.Any((k)=>k.Equals(label,StringComparison.CurrentCultureIgnoreCase));
		Color errorColor	= Color.DarkRed;

		if (errorCase01)
		{
			title 		= $"[Error: NewClock01] Failed to create clock:";
			sb.AppendLine($"A clock with the Label \"{label}\" already exists.");
			sb.AppendLine( "(Labels and Groups are case-insensitive.)");
			ephemeral = true;
		}
		else
		{
			allClocks.Add(label.ToLower(),new Clock(label,group ?? string.Empty,faces));
			allClocks[label.ToLower()].AddDescription(description);
			title = $"Created new clock:";
			sb.AppendLine(allClocks[label.ToLower()].ToStringWithDescription());
		}

		var embedBuilder = new EmbedBuilder();

		if (guildUser != null)
		{
			embedBuilder.WithAuthor(
				guildUser.DisplayName,
				guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl()
			);
		}
		else
			embedBuilder.WithAuthor(
				command.User.GlobalName,
				command.User.GetAvatarUrl() ?? command.User.GetDefaultAvatarUrl()
			);

		if (title != string.Empty)
			embedBuilder.WithTitle(title);
		
		embedBuilder.WithColor(errorCase01 ? errorColor : guildUser != null ? guildUser.Roles.First().Color : Color.Default);

		embedBuilder.WithDescription(sb.ToString()[..^1]);

		embedBuilder.WithCurrentTimestamp();

		await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: ephemeral);
	}

	bool IUserDefinedCommand.IsGlobal => true;
	ulong IUserDefinedCommand.Guild => 0;
}
*/