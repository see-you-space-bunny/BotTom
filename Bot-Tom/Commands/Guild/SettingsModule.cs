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
/// Creates and registers a /settings guild command
/// </summary>
internal class SettingsModule
{
    internal static async Task RegisterSettingsCommand()
    {
        var guildCommand = new SlashCommandBuilder()
            .WithName("settings")
            .WithDescription("Changes some settings within the bot.")
            .AddOption(new SlashCommandOptionBuilder()
                .WithName("field-a")
                .WithDescription("Gets or sets the field A")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("set")
                    .WithDescription("Sets the field A")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("value", ApplicationCommandOptionType.String, "the value to set the field", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("get")
                    .WithDescription("Gets the value of field A.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
            ).AddOption(new SlashCommandOptionBuilder()
                .WithName("field-b")
                .WithDescription("Gets or sets the field B")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("set")
                    .WithDescription("Sets the field B")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("value", ApplicationCommandOptionType.Integer, "the value to set the fie to.", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("get")
                    .WithDescription("Gets the value of field B.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
            ).AddOption(new SlashCommandOptionBuilder()
                .WithName("field-c")
                .WithDescription("Gets or sets the field C")
                .WithType(ApplicationCommandOptionType.SubCommandGroup)
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("set")
                    .WithDescription("Sets the field C")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                    .AddOption("value", ApplicationCommandOptionType.Boolean, "the value to set the fie to.", isRequired: true)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("get")
                    .WithDescription("Gets the value of field C.")
                    .WithType(ApplicationCommandOptionType.SubCommand)
                )
            );

        try
        {
            await Program.DiscordClient.Rest.CreateGuildCommand(guildCommand.Build(), ControlPanel.PrivateGuilds.First().Key);
        }
        catch(HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
            Console.WriteLine(json);
        }
    }

    internal static async Task HandleSettingsCommand(SocketSlashCommand command)
    {
        // First lets extract our variables
        var fieldName = command.Data.Options.First().Name;
        var getOrSet = command.Data.Options.First().Options.First().Name;
        // Since there is no value on a get command, we use the ? operator because "Options" can be null.
        var userOption = command.Data.Options.First().Options.First().Options?.FirstOrDefault();

        switch (fieldName)
        {
            case "field-a":
                {
                    if(getOrSet == "get")
                    {
                        await command.RespondAsync($"The value of `field-a` is `{ControlPanel.FieldA}`");
                    }
                    else if (getOrSet == "set")
                    {
                        ControlPanel.FieldA = (string)userOption!.Value;
                        await command.RespondAsync($"`field-a` has been set to `{ControlPanel.FieldA}`");
                    }
                }
                break;
            case "field-b":
                {
                    if (getOrSet == "get")
                    {
                        await command.RespondAsync($"The value of `field-b` is `{ControlPanel.FieldB}`");
                    }
                    else if (getOrSet == "set")
                    {
                        ControlPanel.FieldB = (long)userOption!.Value;
                        await command.RespondAsync($"`field-b` has been set to `{ControlPanel.FieldB}`");
                    }
                }
                break;
            case "field-c":
                {
                    if (getOrSet == "get")
                    {
                        await command.RespondAsync($"The value of `field-c` is `{ControlPanel.FieldC}`");
                    }
                    else if (getOrSet == "set")
                    {
                        ControlPanel.FieldC = (bool)userOption!.Value;
                        await command.RespondAsync($"`field-c` has been set to `{ControlPanel.FieldC}`");
                    }
                }
                break;
        }
    }
}