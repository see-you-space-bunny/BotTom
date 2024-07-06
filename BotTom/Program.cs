using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using org.mariuszgromada.math.mxparser;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using BotTom.Machines;
using Engine;
using Engine.ModuleHost.Attributes;
using Engine.ModuleHost.Enums;
using Widget.CardGame.Enums;

namespace BotTom;

partial class Program
{
	#region Main
	static async Task Main(string[] args)
	{
		ControlPanel.LoadFileConfig();
		Envy.Load(Path.Combine(Environment.CurrentDirectory, ".env"));
		InitMxParser();

		if (bool.TryParse(Environment.GetEnvironmentVariable("USE_DISCORD"),out bool useDiscord) && useDiscord)
		{
			string? discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN");
			
			if (discordToken is not null)
				await InitializeDiscord(discordToken);
		}

		if (bool.TryParse(Environment.GetEnvironmentVariable("USE_FCHAT"),out bool useFchat) && useFchat)
		{
			string?[] fArgs = [
				"/username="				+	Environment.GetEnvironmentVariable("FCHAT_USERNAME"),
				"/password="				+	Environment.GetEnvironmentVariable("FCHAT_PASSWORD"),
				"/charactername="		+	Environment.GetEnvironmentVariable("FCHAT_CHARACTERNAME"),
				"/startingchannel="	+	Environment.GetEnvironmentVariable("FCHAT_STARTINGCHANNEL"),
				"/commandchar="			+	Environment.GetEnvironmentVariable("FCHAT_COMMANDCHAR"),
				"/globalopslist="		+	Environment.GetEnvironmentVariable("FCHAT_GLOBAL_OPS"),
				"/ownerlist="				+	Environment.GetEnvironmentVariable("FCHAT_OWNER"),
				"/retryattempts="		+	Environment.GetEnvironmentVariable("FCHAT_RETRYATTEMPTS"),
			];
			await InitializeFChat((string[])fArgs!);
		}
	}
	#endregion

	#region InitMxParser
	private static void InitMxParser()
	{
		// ----------------------------------------------------------------------------------
		// mXparser required
		// Non-Commercial Use Confirmation
		bool isCallSuccessful = License.iConfirmNonCommercialUse(Environment.GetEnvironmentVariable("USER_NAME") ?? "Jane Doe");

		// Verification if use type has been already confirmed
		bool isConfirmed = License.checkIfUseTypeConfirmed();

		// Checking use type confirmation message
		String message = License.getUseTypeConfirmationMessage();

		// ----------------------------------------------------------------------------------
		Console.WriteLine("isCallSuccessful = " + isCallSuccessful);
		Console.WriteLine("isConfirmed = " + isConfirmed);
		Console.WriteLine("message = " + message);
		// ----------------------------------------------------------------------------------
	}
	#endregion
}