using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotTom.DiceRoller.GameSystems;
using BotTom.Machines;
using Charsheet.ForgedInTheDark;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using FileManip;
using Newtonsoft.Json;

namespace BotTom.Commands.Global;

/// <summary>
/// Creates and registers a /clock global command
/// </summary>
internal class ClockModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "clock";
	internal const string Description = "Clocks for games such as LANCER or Forged in the Dark.";
	internal const string Display = "display";
	internal const string New = "new";
	internal const string Update = "update";
	internal const string Delete = "delete";
	internal const string CreateClockInfo = "You can create a clock with\n`/new clock label: text faces: number` and an optional `group: text`.\n(Labels and Groups are case-insensitive.)";
	internal const string DisplayClockInfo = "You can check your existing clocks with:\n`/display clock all`";
	internal const string NoClocksInfo = "You haven't created any clocks yet!";
	internal const string NoLabelInfo = "No clock with the Label \"{0}\" exists";
	internal const string NoGroupInfo = "No Group of clocks called \"{0}\" exists";
	internal readonly Dictionary<string,string> Descriptions = [];
	internal readonly Dictionary<string,CommandOption<string>> Label = [];
	internal readonly Dictionary<string,CommandOption<string>> DescriptionOpt = [];
	internal readonly Dictionary<string,CommandOption<string>> Group = [];
	internal readonly Dictionary<string,CommandOption<long>> Faces = [];
	internal readonly Dictionary<string,CommandOption<long>> Progress = [];
	#endregion

	#region 
	#endregion

	#region enum
	private enum Option1
	{
		Single = 0x00,
		All = 0x01,
		Group = 0x02,
	}
	private enum OptionFields
	{
		Label = 0x00,
		Group = 0x01,
		Faces = 0x02,
		Progress = 0x03,
	}
	#endregion

	#region C(-)
	private static readonly CommandOption<bool> Private = new ("private","Hide the result from everyone except you. (default: {0})",false);
	#endregion

	internal ClockModule()
	{
		Descriptions.Add(Display,"Display a persistent item.");
		Descriptions.Add(New,    "Create a persistent item." );
		Descriptions.Add(Update, "Update a persistent item." );
		Descriptions.Add(Delete, "Delete a persistent item." );

		Label.Add(Display,new ("label","Label of the clock you wish to display.",   null,isRequired: true));
		Group.Add(Display,new ("group","Group of clocks you wish to display.",      null,isRequired: true));

		Label.Add(New,new ("label","Label of the clock you wish to create.",            null,isRequired: true));
		Faces.Add(New,new ("faces","The number of faces the new clock should have.",    null,isRequired: true));
		Group.Add(New,new ("group","Group of clocks the new clock should belong to.",   null));

		Label 				.Add(Update,new ("label",   			"Label of the clock you wish to update.",   null,isRequired: true));
		Group 				.Add(Update,new ("group",   			"Group of clocks you wish to update.",      null,isRequired: true));
		DescriptionOpt.Add(Update,new ("description",   "The new description you want to assign the clock.",   							null));
		Progress			.Add(Update,new ("progress",			"The number amount by which you wish to advance the clock.",        null));
		Faces 				.Add(Update,new ("faces",   			"The number amount by which you wish to modify the clock's faces.", null));

		Label.Add(Delete,new ("label","Label of the item you wish to delete.",  null,isRequired: true));
		Group.Add(Delete,new ("group","Group of items you wish to delete.",     null,isRequired: true));
	}

	async Task IUserDefinedCommand.RegisterCommand()
	{
		// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
		var command = new SlashCommandBuilder()
			.WithName(Name)
			.WithDescription(Description);

		#region Build: clock display
		var clockDisplay = new SlashCommandOptionBuilder()
				.WithName(Display)
				.WithDescription(Descriptions[Display])
				.WithType(ApplicationCommandOptionType.SubCommandGroup);

		var clockDisplayAll = new SlashCommandOptionBuilder()
				.WithName(Option1.All.ToString().ToLower())
				.WithDescription(Descriptions[Display])
				.WithType(ApplicationCommandOptionType.SubCommand);
		Private					.AddOption(clockDisplayAll);
 
		var clockDisplaySingle = new SlashCommandOptionBuilder()
				.WithName(Option1.Single.ToString().ToLower())
				.WithDescription(Descriptions[Display])
				.WithType(ApplicationCommandOptionType.SubCommand);
		Label[Display]	.AddOption(clockDisplaySingle);
		Private					.AddOption(clockDisplaySingle);

		var clockDisplayGroup = new SlashCommandOptionBuilder()
				.WithName(Option1.Group.ToString().ToLower())
				.WithDescription(Descriptions[Display])
				.WithType(ApplicationCommandOptionType.SubCommand);
		Group[Display]	.AddOption(clockDisplayGroup);
		Private					.AddOption(clockDisplayGroup);
		
		clockDisplay.AddOption(clockDisplayAll);
		clockDisplay.AddOption(clockDisplaySingle);
		clockDisplay.AddOption(clockDisplayGroup);
		command.AddOption(clockDisplay);
		#endregion

		#region Build: clock update
		var clockUpdate = new SlashCommandOptionBuilder()
				.WithName(Update)
				.WithDescription(Descriptions[Update])
				.WithType(ApplicationCommandOptionType.SubCommandGroup);

		var clockUpdateAll = new SlashCommandOptionBuilder()
				.WithName(Option1.All.ToString().ToLower())
				.WithDescription(Description)
				.WithType(ApplicationCommandOptionType.SubCommand);
		Progress[Update]	.AddOption(clockUpdateAll);
		Faces[Update]			.AddOption(clockUpdateAll);
		Private						.AddOption(clockUpdateAll);

		var clockUpdateSingle = new SlashCommandOptionBuilder()
				.WithName(Option1.Single.ToString().ToLower())
				.WithDescription(Descriptions[Update])
				.WithType(ApplicationCommandOptionType.SubCommand);
		Label[Update]						.AddOption(clockUpdateSingle);
		DescriptionOpt[Update]	.AddOption(clockUpdateSingle);
		Progress[Update]				.AddOption(clockUpdateSingle);
		Faces[Update]						.AddOption(clockUpdateSingle);
		Private									.AddOption(clockUpdateSingle);

		var clockUpdateGroup = new SlashCommandOptionBuilder()
				.WithName(Option1.Group.ToString().ToLower())
				.WithDescription(Descriptions[Update])
				.WithType(ApplicationCommandOptionType.SubCommand);
		Group[Update]	    .AddOption(clockUpdateGroup);
		Progress[Update]	.AddOption(clockUpdateGroup);
		Faces[Update]			.AddOption(clockUpdateGroup);
		Private						.AddOption(clockUpdateGroup);

		clockUpdate.AddOption(clockUpdateAll);
		clockUpdate.AddOption(clockUpdateSingle);
		clockUpdate.AddOption(clockUpdateGroup);
		command.AddOption(clockUpdate);
		#endregion

		#region Build: clock delete
		var deleteClock = new SlashCommandOptionBuilder()
				.WithName(Delete)
				.WithDescription(Descriptions[Delete])
				.WithType(ApplicationCommandOptionType.SubCommandGroup);

		var deleteClockAll = new SlashCommandOptionBuilder()
				.WithName(Option1.All.ToString().ToLower())
				.WithDescription(Descriptions[Delete])
				.WithType(ApplicationCommandOptionType.SubCommand);
		Private			.AddOption(deleteClockAll);

		var deleteClockSingle = new SlashCommandOptionBuilder()
				.WithName(Option1.Single.ToString().ToLower())
				.WithDescription(Descriptions[Delete])
				.WithType(ApplicationCommandOptionType.SubCommand);
		Label[Delete]	.AddOption(deleteClockSingle);
		Private			.AddOption(deleteClockSingle);

		var deleteClockGroup = new SlashCommandOptionBuilder()
				.WithName(Option1.Group.ToString().ToLower())
				.WithDescription(Descriptions[Delete])
				.WithType(ApplicationCommandOptionType.SubCommand);
		Group[Delete]	.AddOption(deleteClockGroup);
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
			case Display:
				await HandleDisplayClockOption(command);
				ClockCabinet.Serialize();
				break;

			case Update:
				await HandleUpdateClockOption(command);
				ClockCabinet.Serialize();
				break;

			case Delete:
				await HandleDeleteClockOption(command);
				ClockCabinet.Serialize();
				break;
		}
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

    #region HandleDisplay
    async Task HandleDisplayClockOption(SocketSlashCommand command)
	{
		var options									= command.Data.Options.First((o)=>o.Name==Display).Options.First();
		SocketGuildUser? guildUser 	= command.User as SocketGuildUser;
        Option1 mode = Enum.Parse<Option1>(options.Name,true);

		string title 			= string.Empty;
		StringBuilder sb 	= new ();
		string label 			= (Label[Display].GetValue(options) ?? string.Empty).ToLower();
		string group 			= (Group[Display].GetValue(options) ?? string.Empty).ToLower();
		bool ephemeral 		= Private.GetValue(options);

		Dictionary<string,Clock> allClocks;
		if(ClockCabinet.Clocks.TryGetValue(command.User.Id, out var value))
			allClocks = value;
		else
		{
			allClocks = [];
			ClockCabinet.Clocks.Add(command.User.Id,allClocks);
		}
		bool errorCase01	= allClocks.Count == 0;
		bool errorCase02	= mode==Option1.Single&&!allClocks.Values.Any((c)=>c.Label.Equals(label,StringComparison.CurrentCultureIgnoreCase));
		bool errorCase03	= mode==Option1.Group &&!allClocks.Values.Any((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase));
		Color errorColor	= Color.DarkRed;

		if (errorCase01)
		{
			CreateErrorMessage("DisplayClock01",NoClocksInfo,string.Empty,sb,out title,out ephemeral,true);
		}
		else
		if (errorCase02)
		{
			foreach(string key in allClocks.Keys)
				sb.Append($"{key}, ");
			sb.Remove(sb.Length-2,2);
			sb.AppendLine();
			CreateErrorMessage("DisplayClock02",NoLabelInfo,Label[Display].GetValue(options)!,sb,out title,out ephemeral);
		}
		else
		if (errorCase03)
		{
			foreach(string key in allClocks.Keys.Where((c)=>!string.IsNullOrEmpty(allClocks[c].Group)))
				sb.Append($"{key}, ");
			sb.Remove(sb.Length-2,2);
			sb.AppendLine();
			CreateErrorMessage("DisplayClock03",NoGroupInfo,Group[Display].GetValue(options)!,sb,out title,out ephemeral);
		}
		else
		switch(mode)
		{
			case Option1.Single:
				Clock singleClock = allClocks[label];
				sb.AppendLine(singleClock.ToStringWithDescription());
				break;

			case Option1.Group:
				title = $"Displaying all '{group}' clocks:";
				foreach(Clock groupedClock in allClocks.Values.Where((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase)))
					sb.AppendLine($"- {groupedClock.ToString()}");
				break;

			case Option1.All:
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

		await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: ephemeral);
	}
    #endregion

    #region HandleUpdate
    async Task HandleUpdateClockOption(SocketSlashCommand command)
	{
		var options									= command.Data.Options.First((o)=>o.Name==Update).Options.First();
		SocketGuildUser? guildUser 	= command.User as SocketGuildUser;
        Option1 mode = Enum.Parse<Option1>(options.Name,true);

		Dictionary<string,Clock> allClocks;
		if(ClockCabinet.Clocks.Keys.Contains(command.User.Id))
			allClocks = ClockCabinet.Clocks[command.User.Id];
		else
		{
			allClocks = new Dictionary<string, Clock>();
			ClockCabinet.Clocks.Add(command.User.Id,allClocks);
		}

		string title 	    = string.Empty;
		StringBuilder sb    = new ();
		string label 			= (Label[Display]		.GetValue(options) ?? string.Empty).ToLower();
		string group 			= (Group[Display]		.GetValue(options) ?? string.Empty).ToLower();
		int? faces 		    = Faces[Update]	    .Defaults(options) ? null : (int)Faces[Update]		.GetValue(options);
		int? progress	    = Progress[Update]	.Defaults(options) ? null : (int)Progress[Update]	.GetValue(options);
		bool ephemeral 	    = Private		    	.GetValue(options);

		bool errorCase01 	 = allClocks.Count == 0;
		bool errorCase02 	 = mode==Option1.Single&&!allClocks.Values.Any((c)=>c.Label.Equals(label,StringComparison.CurrentCultureIgnoreCase));
		bool errorCase03 	 = mode==Option1.Group &&!allClocks.Values.Any((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase));
		Color errorColor = Color.DarkRed;

		if (errorCase01)
		{
			CreateErrorMessage("UpdateClock01",NoClocksInfo,string.Empty,sb,out title,out ephemeral,true);
		}
		else
		if (errorCase02)
		{
			CreateErrorMessage("UpdateClock02",NoLabelInfo,Label[Update].GetValue(options)!,sb,out title,out ephemeral);
		}
		else
		if (errorCase03)
		{
			CreateErrorMessage("UpdateClock03",NoGroupInfo,Group[Update].GetValue(options)!,sb,out title,out ephemeral);
		}
		else
		switch(mode)
		{
			case Option1.Single:
				title = "Updating clock.";
				if (faces != null)
					allClocks[label].AddProgress((int)faces!);
				if (progress != null)
					allClocks[label].AddProgress((int)progress!);
				sb.AppendLine($"The clock is now:");
				sb.AppendLine($"> {allClocks[label].ToStringWithDescription()}");
				break;

			case Option1.Group:
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

			case Option1.All:
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

		await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: ephemeral);
	}
    #endregion

    #region HandleDelete
	async Task HandleDeleteClockOption(SocketSlashCommand command)
	{
		var options									= command.Data.Options.First((o)=>o.Name==Delete).Options.First();
		SocketGuildUser? guildUser 	= command.User as SocketGuildUser;
        Option1 mode = Enum.Parse<Option1>(options.Name,true);

		Dictionary<string,Clock> allClocks;
		if(ClockCabinet.Clocks.TryGetValue(command.User.Id, out Dictionary<string, Clock>? value))
			allClocks = value;
		else
		{
			allClocks = [];
			ClockCabinet.Clocks.Add(command.User.Id,allClocks);
		}

		string title 		= string.Empty;
		StringBuilder sb 	= new ();
		string label 			= (Label[Display].GetValue(options) ?? string.Empty).ToLower();
		string group 			= (Group[Display].GetValue(options) ?? string.Empty).ToLower();
		bool ephemeral 		= Private.GetValue(options);

		bool errorCase01    = allClocks.Count == 0;
		bool errorCase02    = mode==Option1.Single&&!allClocks.Values.Any((c)=>c.Label.Equals(label,StringComparison.CurrentCultureIgnoreCase));
		bool errorCase03    = mode==Option1.Group &&!allClocks.Values.Any((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase));
        Color errorColor    = Color.DarkRed;

		if (errorCase01)
		{
			CreateErrorMessage("DeleteClock01",NoClocksInfo,string.Empty,sb,out title,out ephemeral,true);
		}
		else
		if (errorCase02)
		{
			CreateErrorMessage("DeleteClock02",NoLabelInfo,Label[Delete].GetValue(options)!,sb,out title,out ephemeral);
		}
		else
		if (errorCase03)
		{
			CreateErrorMessage("DeleteClock03",NoGroupInfo,Group[Delete].GetValue(options)!,sb,out title,out ephemeral);
		}
		else
		switch(mode)
		{
			case Option1.Single:
				var confirmationMachine = new SimpleConfirmationMachine(command.User.Id,SimpleConfirmationMachine.ValidTarget.DeleteClockSingle,label);
				confirmationMachine.AdvanceState(SimpleConfirmationMachine.Event.Initiate);
				Program.StateMachines.Add(confirmationMachine);
				title = $"Deleted clock with the Label '{label}'.";
				sb.AppendLine($"At time of deletion, the clock was:\n{allClocks[label].ToStringWithDescription()}");
				allClocks.Remove(label);
				break;

			case Option1.Group:
				int clocksCount = allClocks.Values.Where((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase)).ToList().Count;
				title = $"Deleted all clocks in Group '{group}'.";
				sb.AppendLine($"{clocksCount} clocks were deleted.");
				foreach(Clock groupedClock in allClocks.Values.Where((c)=>c.Group.Equals(group,StringComparison.CurrentCultureIgnoreCase)))
					allClocks.Remove(groupedClock.Group.ToLower());
				break;

			case Option1.All:
				title = $"Deleted all {allClocks.Count} clocks.";
				sb.AppendLine($"{allClocks.Count} clocks were deleted.");
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

		await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: ephemeral);
	}
    #endregion
	bool IUserDefinedCommand.IsGlobal => true;
	ulong IUserDefinedCommand.Guild => 0;

	private void CreateErrorMessage(string errorname, string errorInfo, string errorInput, StringBuilder sb, out string title, out bool ephemeral, bool createOnly=false)
	{
		title = $"[Error: {errorname}] {string.Format(errorInfo,errorInput)}";
		ephemeral = true;
		sb.AppendLine(CreateClockInfo);
		if (createOnly)
			return;
		sb.AppendLine();
		sb.AppendLine(DisplayClockInfo);
	}
}