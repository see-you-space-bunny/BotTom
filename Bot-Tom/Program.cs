using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using org.mariuszgromada.math.mxparser;
using BotTom.Commands;
using BotTom.Commands.Global;
using BotTom.Commands.Guild;
using BotTom.Machines;

namespace BotTom;

partial class Program
{
	#region Main
	static async Task Main(string[] args)
	{
		ControlPanel.LoadFileConfig();
		Envy.Envy.Load(Path.Combine(Environment.CurrentDirectory, ".env"));
		InitMxParser();
		await InitializeDiscord();
	}
	#endregion


	#region InitMxParser
	private static void InitMxParser()
	{
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
	}
	#endregion
}