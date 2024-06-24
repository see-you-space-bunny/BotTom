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
/// Creates and registers a /sta global command
/// </summary>
internal class StarTrekModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "star-trek-adventures";
	internal const string Description = "Roll d20s for Star Trek Adventures.";
	#endregion

	#region C(-)
	private static readonly CommandOption<long>		TargetNumber					= new ("tn","Your aptitude plus discipline.",null,isRequired: true);
	private static readonly CommandOption<long>		FocusNumber						= new ("fn","If you have a focus for the roll, add your discipline here. (default: {0})",(long)1);
	private static readonly CommandOption<long>		DiceCount							= new ("dice","The number of dice you're rolling. (default: {0})",(long)2);
	private static readonly CommandOption<long>		ThreatThreshold				= new ("threat","The threshold at which you generate Threat. (default: {0})",(long)20);
	private static readonly CommandOption<long>		ComputerTargetNumber	= new ("ctn","The Computer's aptitude plus discipline.",long.MinValue);
	private static readonly CommandOption<long>		ComputerFocusNumber		= new ("cfn","If the Computer has a focus for the roll, add its discipline here.",(long)1);
	private static readonly CommandOption<string>	Label    							= new ("label","A label to identify what the roll is for. (default: none)",null);
	private static readonly CommandOption<bool>		Private								= new ("private","Hide the result from everyone except you. (default: {0})",false);
	#endregion

	internal StarTrekModule()
	{ }

	async Task IUserDefinedCommand.RegisterCommand()
	{
			var command = new SlashCommandBuilder();

			// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
			command.WithName(Name);
			command.WithDescription(Description);
			TargetNumber					.AddOption(command);
			FocusNumber						.AddOption(command);
			DiceCount							.AddOption(command);
			ThreatThreshold				.AddOption(command);
			ComputerTargetNumber	.AddOption(command);
			ComputerFocusNumber		.AddOption(command);
			Label    							.AddOption(command);
			Private								.AddOption(command);

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
		SocketGuildUser? guildUser 	= command.User as SocketGuildUser;
		long	tn	=	TargetNumber				.GetValue(command);
		long	fn	=	FocusNumber					.Defaults(command)	? (long)FocusNumber.DefaultValue!					: FocusNumber					.GetValue(command);
		long	dc	=	DiceCount						.Defaults(command)	? (long)DiceCount.DefaultValue!						: DiceCount						.GetValue(command);
		long	tt	=	ThreatThreshold			.Defaults(command)	? (long)ThreatThreshold.DefaultValue!			: ThreatThreshold			.GetValue(command);
		long?	ctn	=	ComputerTargetNumber.Defaults(command)	? long.MinValue														: ComputerTargetNumber.GetValue(command);
		long cfn	=	ComputerFocusNumber	.Defaults(command)	? (long)ComputerFocusNumber.DefaultValue!	: ComputerFocusNumber	.GetValue(command);
		string label = Label .GetValue(command) ?? string.Empty;

		var sta_r = new StarTrekRoll(tn,fn,dc,tt,ctn,cfn,null);
		sta_r.Roll();

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

		embedBuilder.WithDescription(sta_r.ToString());

		embedBuilder.WithCurrentTimestamp();

		await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: Private.GetValue(command));
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	bool IUserDefinedCommand.IsGlobal => false;
	ulong IUserDefinedCommand.Guild => 0;
}