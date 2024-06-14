using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using FileManip;
using Microsoft.VisualBasic;
using org.mariuszgromada.math.mxparser;

namespace BotTom
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            Envy.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

            if(Environment.GetEnvironmentVariable("OFFLINE_DEBUG")!.Equals("false"))
            {
                var discordClient = new DiscordClient(new DiscordConfiguration
                {
                    Token = Environment.GetEnvironmentVariable("DISCORD_TOKEN"),
                    TokenType = TokenType.Bot,
                    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.GuildMessages | DiscordIntents.MessageContents
                });
                
                // ----------------------------------------------------------------------------------
                // mXparser required
                // Non-Commercial Use Confirmation
                bool isCallSuccessful = License.iConfirmNonCommercialUse(Environment.GetEnvironmentVariable("USER_NAME"));
                
                // Verification if use type has been already confirmed
                bool isConfirmed = License.checkIfUseTypeConfirmed();
                
                // Checking use type confirmation message
                String message = License.getUseTypeConfirmationMessage();
                
                // ----------------------------------------------------------------------------------
                Console.WriteLine("isCallSuccessful = " + isCallSuccessful);
                Console.WriteLine("isConfirmed = " + isConfirmed);
                Console.WriteLine("message = " + message);
                // ----------------------------------------------------------------------------------

                var slash = discordClient.UseSlashCommands();
                slash.RegisterCommands<BotTomDiceModule>();

                await discordClient.ConnectAsync();
                await Task.Delay(-1);
            }
        }
    }
}

