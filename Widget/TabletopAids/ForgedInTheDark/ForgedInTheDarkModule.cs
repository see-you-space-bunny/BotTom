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
internal class ForgedInTheDarkModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "fitd";
	internal const string Description = "Roll d6s for Forged in the Dark systems.";
	#endregion

	#region C(-)
	private static readonly CommandOption<long>		DicePool 	= new ("dice","Your dice pool for this roll. '0' rolls 2d6 and keeps lowest.",null,isRequired: true);
	private static readonly CommandOption<string>	Label    	= new ("label","A label to identify what the roll is for. (default: none)",null);
	private static readonly CommandOption<bool>		Private  	= new ("private","Hide the result from everyone except you. (default: {0})",false);
	#endregion

	internal ForgedInTheDarkModule()
	{ }

	async Task IUserDefinedCommand.RegisterCommand()
	{
		var command = new SlashCommandBuilder();

		// Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
		command.WithName(Name);
		command.WithDescription(Description);

		DicePool	.AddOption(command);
		Label			.AddOption(command);
		Private		.AddOption(command);

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
		var fitd_r = new ForgedInTheDarkRoll(
			(int)DicePool	.GetValue(command),
					 Label		.GetValue(command)
		);
		fitd_r.Roll();

		await command.RespondAsync(fitd_r.ToString(), ephemeral: Private.GetValue(command));
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	bool IUserDefinedCommand.IsGlobal => true;
	ulong IUserDefinedCommand.Guild => 0;
}
*/