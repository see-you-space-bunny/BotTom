using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace BotTom.Commands.Guild;

/// <summary>
/// Creates and registers a /list-roles guild command
/// </summary>
public class ListRolesModule : IUserDefinedCommand
{
	#region C(~)
	internal const string Name = "list-roles";
	#endregion

    #region P(~)
    internal bool IsGlobal => Guild is null;
    internal ulong? Guild { get; private set; } = null;
    #endregion

	internal ListRolesModule(ulong guild)
	{
        Guild = guild;
	}

	async Task IUserDefinedCommand.RegisterCommand()
	{
        if(Guild == null)
            throw new NullReferenceException(nameof(Guild));

        var guildCommand = new SlashCommandBuilder();

        // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
        guildCommand.WithName(Name);
        guildCommand.WithDescription("Lists all roles of a user.");
        guildCommand.AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to be listed", isRequired: true);
        
        try
        {
            await Program.DiscordClient.Rest.CreateGuildCommand(guildCommand.Build(), (ulong)Guild!);
        }
        catch(HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
	}

	async Task IUserDefinedCommand.HandleSlashCommand(SocketSlashCommand command)
	{
        // We need to extract the user parameter from the command. 
        // since we only have one option and it's required, we can just use the first option.
        var guildUser = (SocketGuildUser)command.Data.Options.First().Value;

        // We remove the everyone role and select the mention of each role.
        var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

        var embedBuiler = new EmbedBuilder();
        embedBuiler.WithAuthor(guildUser.DisplayName, guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl());
        embedBuiler.WithTitle("Roles");
        embedBuiler.WithDescription(roleList);
        embedBuiler.WithColor(Color.Green);
        embedBuiler.WithCurrentTimestamp();

        // Now, Let's respond with the embed.
        await command.RespondAsync(embed: embedBuiler.Build(), ephemeral: true);
	}

	#pragma warning disable CS1998
	async Task IUserDefinedCommand.HandleCommand(Dictionary<string,object> commandInfo)
	{ }
	#pragma warning restore CS1998

	bool IUserDefinedCommand.IsGlobal => IsGlobal;
	ulong IUserDefinedCommand.Guild => (ulong)Guild!;
}