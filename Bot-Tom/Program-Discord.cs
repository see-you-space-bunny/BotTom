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
	static async Task InitializeDiscord()
	{
		_client = new DiscordSocketClient(new DiscordSocketConfig
		{
				// How much logging do you want to see?
				LogLevel = LogSeverity.Info,
				
				// If you or another service needs to do anything with messages
				// (eg. checking Reactions, checking the content of edited/deleted messages),
				// you must set the MessageCacheSize. You may adjust the number as needed.
				//MessageCacheSize = 50,

				// If your platform doesn't have native WebSockets,
				// add Discord.Net.Providers.WS4Net from NuGet,
				// add the `using` at the top, and uncomment this line:
				//WebSocketProvider = WS4NetProvider.Instance
		});
        
		_commands = new CommandService(new CommandServiceConfig
		{
				// Again, log level:
				LogLevel = LogSeverity.Info,
				
				// There's a few more properties you can set,
				// for example, case-insensitive commands.
				CaseSensitiveCommands = false,
		});
        
		// Subscribe the logging handler to both the client and the CommandService.
		_client.Log += Log;
		_commands.Log += Log;
		
		// Setup your DI container.
		/** _services = ConfigureServices(); */

		await _client.LoginAsync(
			TokenType.Bot,
			token: Environment.GetEnvironmentVariable("DISCORD_TOKEN")
		);
		
		_client.SlashCommandExecuted += SlashCommandHandler;
		_client.Ready += RegisterCommandsOnReadyAsync;

		await _client.StartAsync();

		// Block this task until the program is closed.
		await Task.Delay(-1);
	}
	#endregion

	#pragma warning disable CS8618
	#region F(-)
	private static DiscordSocketClient _client;

	// Keep the CommandService and DI container around for use with commands.
	// These two types require you install the Discord.Net.Commands package.
	private static CommandService _commands;
	/** private static IServiceProvider _services; */
	#endregion
	#pragma warning restore CS8618

	#region F(~)
	private static List<IStateMachine> _stateMachines = [];
	#endregion

	#region P(~)
	internal static DiscordSocketClient DiscordClient => _client;
	internal static List<IStateMachine> StateMachines => _stateMachines;
	internal static IEnumerable<SimpleConfirmationMachine> ConfirmationMachines => _stateMachines.OfType<SimpleConfirmationMachine>();
	internal static Dictionary<string,IUserDefinedCommand> RegisteredCommands { get; } = [];
	#endregion
  
	#region ConfigureServices
	// If any services require the client, or the CommandService, or something else you keep on hand,
	// pass them as parameters into this method as needed.
	// If this method is getting pretty long, you can seperate it out into another file using partials.
	/**
	private static IServiceProvider ConfigureServices()
	{
			var map = new ServiceCollection()        
					// Repeat this for all the service classes
					// and other dependencies that your commands might need.
					.AddSingleton(new SomeServiceClass());
					
			// When all your required services are in the collection, build the container.
			// Tip: There's an overload taking in a 'validateScopes' bool to make sure
			// you haven't made any mistakes in your dependency graph.
			return map.BuildServiceProvider();
	}
	*/
	#endregion

	#region Log
	private static Task Log(LogMessage msg)
	{
        switch (msg.Severity)
        {
            case LogSeverity.Critical:
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Verbose:
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
        }
        Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message} {msg.Exception}");
        Console.ResetColor();
        
        // If you get an error saying 'CompletedTask' doesn't exist,
        // your project is targeting .NET 4.5.2 or lower. You'll need
        // to adjust your project's target framework to 4.6 or higher
        // (instructions for this are easily Googled).
        // If you *need* to run on .NET 4.5 for compat/other reasons,
        // the alternative is to 'return Task.Delay(0);' instead.
        return Task.CompletedTask;
	}
	#endregion

	#region MainAsync
	/**
	private static async Task MainAsync()
	{
		// Centralize the logic for commands into a separate method.
		await InitCommands();

		// Login and connect.
		await _client.LoginAsync(
			TokenType.Bot,
			Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
		await _client.StartAsync();

		// Wait infinitely so your bot actually stays connected.
		await Task.Delay(Timeout.Infinite);
	}
	*/
	#endregion

	#region InitCommands
	/**
	private static async Task InitCommands()
	{
			// Either search the program and add all Module classes that can be found.
			// Module classes MUST be marked 'public' or they will be ignored.
			// You also need to pass your 'IServiceProvider' instance now,
			// so make sure that's done before you get here.
			await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
			
			// Or add Modules manually if you prefer to be a little more explicit:
			//await _commands.AddModuleAsync<SomeModule>(_services);
			// Note that the first one is 'Modules' (plural) and the second is 'Module' (singular).

			// Subscribe a handler to see if a message invokes a command.
			_client.MessageReceived += HandleCommandAsync;
	}
	*/
	#endregion

	#region HandleCommandAsync
	/**
	private static async Task HandleCommandAsync(SocketMessage arg)
	{
		// Bail out if it's a System Message.
		var msg = arg as SocketUserMessage;
		if (msg == null) return;

		// We don't want the bot to respond to itself or other bots.
		if (msg.Author.Id == _client.CurrentUser.Id || msg.Author.IsBot) return;
		
		// Create a number to track where the prefix ends and the command begins
		int pos = 0;
		// Replace the '!' with whatever character
		// you want to prefix your commands with.
		// Uncomment the second half if you also want
		// commands to be invoked by mentioning the bot instead.
		if (msg.HasCharPrefix('!', ref pos) || msg.HasMentionPrefix(_client.CurrentUser, ref pos) )
		{
			// Create a Command Context.
			var context = new SocketCommandContext(_client, msg);
			
			// Execute the command. (result does not indicate a return value, 
			// rather an object stating if the command executed successfully).
			var result = await _commands.ExecuteAsync(context, pos, _services);

			// Uncomment the following lines if you want the bot
			// to send a message if it failed.
			// This does not catch errors from commands with 'RunMode.Async',
			// subscribe a handler for '_commands.CommandExecuted' to see those.
			//if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
			//    await msg.Channel.SendMessageAsync(result.ErrorReason);
		}
	}
	*/
	#endregion

	#region RegisterCommandsOnReadyAsync
	private static async Task RegisterCommandsOnReadyAsync()
	{
		RegisteredCommands.Add(ListRolesModule.Name,				new ListRolesModule(ControlPanel.PrivateGuilds.First().Key));
		RegisteredCommands.Add(StarTrekModule.Name,					new StarTrekModule());
		RegisteredCommands.Add(PathfinderModule.Name,				new PathfinderModule());
		RegisteredCommands.Add(StorytellerModule.Name,			new StorytellerModule());
		RegisteredCommands.Add(ForgedInTheDarkModule.Name,	new ForgedInTheDarkModule());
		RegisteredCommands.Add(ClockModule.Name,						new ClockModule());
		RegisteredCommands.Add(NewObjectModule.Name,				new NewObjectModule());
		RegisteredCommands.Add(ConfirmationModule.Name,			new ConfirmationModule());

		foreach(IUserDefinedCommand userDefinedCommand in RegisteredCommands.Values)
			await userDefinedCommand.RegisterCommand();
	}
	#endregion

	#region SlashCommandHandler
	private static async Task SlashCommandHandler(SocketSlashCommand command)
	{
			await RegisteredCommands[command.Data.Name].HandleSlashCommand(command);
	}
	#endregion

	#region CheckStateMachines
	public static void CheckStateMachines()
	{
		_stateMachines.RemoveAll((m)=>m.AtTerminalStage||m.IsExpired);
	}
	#endregion
}