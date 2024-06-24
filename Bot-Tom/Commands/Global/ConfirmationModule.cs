using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotTom.Machines;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace BotTom.Commands.Global;

/// /// <summary>
/// Creates and registers a /clock global command
/// </summary>
internal class ConfirmationModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "confirm";
	internal const string Description = "Confirms your currently pending interaction.";
    internal readonly CommandOption<string> Passphase = new ("passphrase","Leave this blank if you were not given a passphrase. (default: none)",null);
	private static readonly CommandOption<bool> Private = new ("private","Hide the result from everyone except you. (default: {0})",false);
    #endregion

    async Task IUserDefinedCommand.RegisterCommand()
	{
		// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
		var command = new SlashCommandBuilder()
			.WithName(Name)
			.WithDescription(Description);
        Passphase   .AddOption(command);
        Private     .AddOption(command);

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
		string? passphrase  = Passphase .Defaults(command) ? null                           : (string)Passphase .GetValue(command)!;
		bool ephemeral      = Private   .Defaults(command) ? (bool)Private.DefaultValue!    : (bool)Private .GetValue(command)!;
		var confirmationMachine = Program.ConfirmationMachines.First((m)=>m.UserId==command.User.Id);
        string title, message;
        if (passphrase is null || (passphrase is not null && passphrase.Trim() == confirmationMachine.Passphrase))
        {
            var (Title, Message) = PerformConfirmedAction(confirmationMachine);
            title = Title;
            message = Message;
        }
        else
        {
            title = "Wrong passphrase!!";
            message = "Operation cancelled.";
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
		
		embedBuilder.WithColor(guildUser != null ? guildUser.Roles.First().Color : Color.Default);

		embedBuilder.WithDescription(message);

		embedBuilder.WithCurrentTimestamp();

		await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: ephemeral);
	}

    private (string Title,string Message) PerformConfirmedAction(SimpleConfirmationMachine confirmationMachine)
    {
        switch(confirmationMachine.Target)
        {
            case SimpleConfirmationMachine.ValidTarget.DeleteClockSingle:
                return (string.Empty,string.Empty);
                
            case SimpleConfirmationMachine.ValidTarget.DeleteClockGroup:
                return (string.Empty,string.Empty);

            case SimpleConfirmationMachine.ValidTarget.DeleteClockAll:
                return (string.Empty,string.Empty);

            default:
                return ("An error has ocurred!","Unable to perform the desired action.");
        }
    }
    
    bool IUserDefinedCommand.IsGlobal => true;
    ulong IUserDefinedCommand.Guild => 0;
}