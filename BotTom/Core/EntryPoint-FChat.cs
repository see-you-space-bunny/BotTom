using BotTom.FChat;

using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Tokenizer;

using Engine.ModuleHost;
using Engine.ModuleHost.Enums;

using ModularPlugins;
using ModularPlugins.Interfaces;

using FGlobals;
using CardGame;

namespace BotTom;

partial class Program
{
	// -----------------------------------------------
	//             User Information
	// -----------------------------------------------
	private static readonly string F_UserNameArg			= "UserName";
	private static readonly string F_PassWordArg			= "Password";
	private static readonly string F_CharacterNameArg		= "CharacterName";
	private static readonly string F_StartingChannelArg		= "StartingChannel";
	private static readonly string F_RetryAttemptsArg		= "RetryAttempts";
	private static readonly string F_CommandCharArg			= "CommandChar";
	private static readonly string F_OwnerListArg			= "OwnerList";
	private static readonly string F_GlobalOpsListArg		= "GlobalOpsList";
	// -----------------------------------------------

	private static string F_StartingChannel = string.Empty;

	/// <summary>
	/// Our chat interface
	/// </summary>
	private static ApiConnection? F_Chat;

	/// <summary>
	/// Our bot interface
	/// </summary>
	private static ChatBot? F_Bot;

	/// <summary>
	/// our main command character
	/// </summary>
	private static string F_CommandChar = string.Empty;

	/// <summary>
	/// our command tokenizer
	/// </summary>
	private static CommandParser F_CommandParser = default!;
	
	/// <summary>
	/// A list of global bot ops
	/// </summary>
	private static List<string> F_GlobalOps = [];
	
	/// <summary>
	/// A list of bot owners
	/// </summary>
	private static List<string> F_Owner = [];

	/// <summary>
	/// A collection of any failed cli args in validation check
	/// </summary>
	private static readonly List<string> F_FailedCliArgs = [];

	/// <summary>
	/// The bot's character name
	/// </summary>
	private static string F_CharacterName = string.Empty;

	/// <summary>
	/// the character testing the bot
	/// </summary>
	private static string FCHAT_OWNER = string.Empty;

	/// <summary>
	/// What to reset retry attempts to on reboot
	/// </summary>
	private static int RetryAttemptsReset = -1;

	#region (-) InitializeFChat
	private static async Task InitializeFChat(string[] args)
	{
		// cli arg parsing and validation
		if (args.Length == 0)
		{
				Console.WriteLine($"Error: expecting launch arguments.");
				Environment.Exit(-1);
		}

		string cliArgumentsStr = string.Join(" ", args);
		List<string> cliArgs = [.. cliArgumentsStr.Split('/', StringSplitOptions.RemoveEmptyEntries)];
		cliArgs.ForEach(arg => arg = arg.Replace(" ", ""));
		Dictionary<string, string> cliArgDict = [];
		foreach (var singleArg in cliArgs)
		{
				var splitThing = singleArg.Split('=');
				cliArgDict.Add(splitThing.First().ToLowerInvariant(), splitThing.Last().Trim());
		}

		ValidateArgument(out string F_UserName,        	F_UserNameArg, 			cliArgDict);
		ValidateArgument(out string F_Password,        	F_PassWordArg, 			cliArgDict);
		ValidateArgument(out string F_CharacterName,   	F_CharacterNameArg, 	cliArgDict);
		ValidateArgument(out string F_StartingChannel, 	F_StartingChannelArg,	cliArgDict, true);
		ValidateArgument(out F_CommandChar,            	F_CommandCharArg,		cliArgDict);
		ValidateArgument(out F_Owner,              		F_OwnerListArg,			cliArgDict);
		ValidateArgument(out F_GlobalOps,           	F_GlobalOpsListArg,		cliArgDict, true);

		ValidateArgument(out RetryAttemptsReset,       	F_RetryAttemptsArg, 	cliArgDict);

		int RetryAttempts = RetryAttemptsReset;

		if (!ConfirmCliArgumentValidation())
		{
				Environment.Exit(-1);
		}
		
		FCHAT_OWNER = F_Owner[0];

		F_CommandParser = new CommandParser(F_CommandChar);

		while (RetryAttempts > 0)
		{
			try
			{
				await RunChat(F_UserName, F_Password, F_CharacterName, F_StartingChannel, F_CommandChar);
			}
			catch(Exception e)
			{
				RetryAttempts--;
				Console.WriteLine($"Error. Attempting to reconnect to chat. Attempt {4 - RetryAttempts} out of 3 : {e}");
			}
		}

		Environment.Exit(0);
	}
#endregion

#region (-) RunChat
	/// <summary>
	/// Connection loop for easier retries
	/// </summary>
	/// <returns>false if we lose our connection</returns>
	private static async Task<bool> RunChat(string userName, string passWord, string characterName, string startingChannel, string commandChar)
	{
		if (F_Chat != null)
			F_Chat = null;

		if (F_Bot != null)
		{
			await F_Bot.Shutdown();
			F_Bot = null;
			Thread.Sleep(10000);
		}

		try
		{
			F_Chat 	= new ApiConnection();
			F_Bot 	= new ChatBot();

////////////// Add our plugins here ////////////////////////////////////////////////
#if DEBUG
			F_Bot.AddPlugin(BotModule.System,new FChatGlobalCommands(F_Chat,TimeSpan.MaxValue)
				.SetOperators(F_Owner,		Privilege.OwnerOperator)
				.SetOperators(F_GlobalOps,	Privilege.GlobalOperator));
			F_Bot.AddPlugin(BotModule.XCG,new FChatTournamentOrganiser(F_Chat,new TimeSpan(500)));
#else
			F_Bot.AddPlugin<FChatGlobalCommands>(new FChatGlobalCommands(F_Chat)
				.SetOperators(F_Owner,		Privilege.OwnerOperator)
				.SetOperators(F_GlobalOps,	Privilege.GlobalOperator));
#endif
////////////// End Plugin Adding ///////////////////////////////////////////////////

			F_CharacterName							= characterName;

			F_Chat.ConnectedToChat					= ConnectedToChat;
			F_Chat.MessageHandler					= HandleMessageReceived;
			F_Chat.JoinedChannelHandler				= HandleJoinedChannel;
			F_Chat.CreatedChannelHandler			= HandleCreatedChannel;
			F_Chat.LeftChannelHandler				= HandleLeftChannel;
			F_Chat.PrivateChannelsReceivedHandler	= HandlePrivateChannelsReceived;
			F_Chat.PublicChannelsReceivedHandler	= HandlePublicChannelsReceived;

			await F_Chat.ConnectToChat(userName, passWord, characterName);
			F_StartingChannel						= startingChannel;

			SystemController.Instance.SetApi(F_Chat);

			// initiate the loop
			while (ApiConnection.IsConnected())
			{
				Update();
			}

			return false;
		}
		catch (Exception e)
		{
			Console.WriteLine($"Error connecting to chat: {e}");
			Thread.Sleep(10000);
			return false;
		}
	}
#endregion

#region (-) Update
	/// <summary>
	/// update loop, do w/e in here
	/// </summary>
	private static async void Update()
	{
		await F_Bot!.Update();

		Task shutTask = null!;
		if (F_Bot != null && ApiConnection.ShutdownFlag)
		{
			shutTask = F_Bot.Shutdown();
		}

////////////////////////////
		

		
////////////////////////////

		if (shutTask != null)
		{
			await shutTask;
			Environment.Exit(0);
		}
		else
		{
			Thread.Sleep(10000);
		}
	}
#endregion
}