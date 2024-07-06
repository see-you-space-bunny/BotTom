using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Engine.DiceRoll.GameSystems;

namespace Widget.TabletopAids.GameSystems;

/// /// <summary>
/// Creates and registers a /clock global command
/// </summary>
/**
internal class ConfirmationModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "confirm";
	internal const string Description = "Confirms your currently pending interaction.";
    internal readonly CommandOption<string> Passphase = new ("passphrase","Leave this blank if you were not given a passphrase. (default: none)",null);
	private static readonly CommandOption<bool> Private = new ("private","Hide the result from everyone except you. (default: {0})",true);
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
		
		embedBuilder.WithColor(guildUser != null ? guildUser.Roles.First().Color : Color.Default)
			.WithDescription(message)
			.WithCurrentTimestamp();

		await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: ephemeral);
	}

    private (string Title,string Message) PerformConfirmedAction(SimpleConfirmationMachine confirmationMachine)
    {
			confirmationMachine.AdvanceState(SimpleConfirmationMachine.Event.Confirm);
			string title;
			string message;
			if(!(confirmationMachine.CurrentState == SimpleConfirmationMachine.State.Confirmed))
			{
				title = "Could not confirm the action!";
				message = confirmationMachine.StateInfo;
			}
			else
			switch(confirmationMachine.Target)
			{
				case SimpleConfirmationMachine.ValidTarget.DeleteClockSingle:
					var allClocksSingle = ClockCabinet.Clocks[confirmationMachine.UserId];
					title = $"Deleted clock with the Label '{confirmationMachine.TargetValue}'.";
					message = $"At time of deletion, the clock was:\n{allClocksSingle[confirmationMachine.TargetValue.ToLower()].ToStringWithDescription()}";
					allClocksSingle.Remove(confirmationMachine.TargetValue);
					break;
						
				case SimpleConfirmationMachine.ValidTarget.DeleteClockGroup:
					var allClocksGroup = ClockCabinet.Clocks[confirmationMachine.UserId];
					var groupClocks = allClocksGroup.Values.Where((c)=>c.Group.Equals(confirmationMachine.TargetValue,StringComparison.CurrentCultureIgnoreCase)).ToList();
					title = $"Deleted all clocks in Group '{groupClocks.First().Group}'.";
					message = $"{groupClocks.Count} clocks were deleted.";
					foreach(Clock groupedClock in allClocksGroup.Values.Where((c)=>c.Group.Equals(confirmationMachine.TargetValue,StringComparison.CurrentCultureIgnoreCase)))
						allClocksGroup.Remove(groupedClock.Group.ToLower());
					break;

				case SimpleConfirmationMachine.ValidTarget.DeleteClockAll:
					var allClocksAll = ClockCabinet.Clocks[confirmationMachine.UserId];
					title = $"Deleted all {allClocksAll.Count} clocks.";
					message = $"{allClocksAll.Count} clocks were deleted.";
					allClocksAll.Clear();
					break;

				default:
					return ("An error has ocurred!","Unable to perform the desired action.");
			}
			Program.CheckStateMachines();
			return (title,message);
    }
    
    bool IUserDefinedCommand.IsGlobal => true;
    ulong IUserDefinedCommand.Guild => 0;
}
*/