using BotTom.FChat;
using FChatApi.Core;
using FChatApi.Enums;
using Engine.ModuleHost;
using Engine.ModuleHost.CommandHandling;
using Widget.CardGame;
using Widget.FGlobals;
//using Widget.TabletopAids;

namespace BotTom;

partial class Program
{
	// -----------------------------------------------
	//             User Information
	// -----------------------------------------------
	static readonly string F_UserNameArg			= "UserName";
	static readonly string F_PassWordArg			= "Password";
	static readonly string F_CharacterNameArg		= "CharacterName";
	static readonly string F_StartingChannelArg		= "StartingChannel";
	static readonly string F_RetryAttemptsArg		= "RetryAttempts";
	static readonly string F_CommandCharArg			= "CommandChar";
	static readonly string F_OwnerListArg			= "OwnerList";
	static readonly string F_GlobalOpsListArg		= "GlobalOpsList";
	// -----------------------------------------------

	static string F_StartingChannel = string.Empty;

	/// <summary>
	/// Our chat interface
	/// </summary>
	static ApiConnection? F_Chat;
	
	public static List<string> F_ActiveChannels = [];

	/// <summary>
	/// Our bot interface
	/// </summary>
	static ChatBot? F_Bot;

	/// <summary>
	/// our main command character
	/// </summary>
	static string F_CommandChar = string.Empty;

	/// <summary>
	/// our command tokenizer
	/// </summary>
	static CommandParser F_CommandParser = new(string.Empty,[]);
	
	/// <summary>
	/// A list of global bot ops
	/// </summary>
	static List<string> F_GlobalOps = [];
	
	/// <summary>
	/// A list of bot owners
	/// </summary>
	static List<string> F_Owner = [];

	/// <summary>
	/// A collection of any failed cli args in validation check
	/// </summary>
	static readonly List<string> F_FailedCliArgs = [];

	/// <summary>
	/// The bot's character name
	/// </summary>
	public static string F_CharacterName = string.Empty;

	/// <summary>
	/// What to reset retry attempts to on reboot
	/// </summary>
	public static int RetryAttemptsReset = -1;

	#region Main
	static async Task InitializeFChat(string[] args)
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

		ValidateArgument(out string F_UserName,        	F_UserNameArg, 				cliArgDict);
		ValidateArgument(out string F_Password,        	F_PassWordArg, 				cliArgDict);
		ValidateArgument(out string F_CharacterName,   	F_CharacterNameArg, 	cliArgDict);
		ValidateArgument(out string F_StartingChannel, 	F_StartingChannelArg,	cliArgDict, true);
		ValidateArgument(out F_CommandChar,            	F_CommandCharArg,			cliArgDict);
		ValidateArgument(out F_Owner,              			F_OwnerListArg,				cliArgDict);
		ValidateArgument(out F_GlobalOps,           		F_GlobalOpsListArg,		cliArgDict, true);

		ValidateArgument(out RetryAttemptsReset,       	F_RetryAttemptsArg, 	cliArgDict);

		int RetryAttempts = RetryAttemptsReset;

		if (!ConfirmCliArgumentValidation())
		{
				Environment.Exit(-1);
		}

		F_CommandParser.WithBotPrefix(F_CommandChar);

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

#region Validation
	/// <summary>
	/// handles cli arg validation confirmation
	/// </summary>
	/// <returns>false if any of our arguments failed to validate</returns>
	static bool ConfirmCliArgumentValidation()
	{
			if (F_FailedCliArgs.Count != 0)
			{
					foreach(string failedArgReply in F_FailedCliArgs)
					{
							Console.WriteLine($"{failedArgReply}");
					}
					return false;
			}

			return true;
	}

	/// <summary>
	/// attempts to validate an argument
	/// </summary>
	/// <param name="argVal">value of argument to validate</param>
	/// <param name="argName">name of the argument we're attempting to validate</param>
	/// <param name="rawArgs">the unfiltered arg cmd to check</param>
	/// <param name="isOptional">if the arg is optional or not</param>
	static void ValidateArgument(out List<string> argVal, string argName, Dictionary<string, string> rawArgs, bool isOptional = false)
	{
			argVal = [];
			argName = argName.ToLowerInvariant();

			if (!rawArgs.TryGetValue(argName, out string? value))
			{
					if (!isOptional)
							F_FailedCliArgs.Add($"Failed Validation: {argName}");
					return;
			}

			if (string.IsNullOrWhiteSpace(value))
			{
					F_FailedCliArgs.Add($"Failed Validation: {argName}");
					return;
			}

			argVal = [.. value.Split(',')];
			Console.WriteLine($"{argName} --- {string.Join(",", argVal)}");
	}

	/// <summary>
	/// attempts to validate an argument
	/// </summary>
	/// <param name="argVal">value of argument to validate</param>
	/// <param name="argName">name of the argument we're attempting to validate</param>
	/// <param name="rawArgs">the unfiltered arg cmd to check</param>
	/// <param name="isOptional">if the arg is optional or not</param>
	static void ValidateArgument(out string argVal, string argName, Dictionary<string, string> rawArgs, bool isOptional = false)
	{
			argVal = string.Empty;
			argName = argName.ToLowerInvariant();

			if (!rawArgs.TryGetValue(argName, out string? value))
			{
					if (!isOptional)
							F_FailedCliArgs.Add($"Failed Validation: {argName}");
					return;
			}

			if (string.IsNullOrWhiteSpace(value))
			{
					F_FailedCliArgs.Add($"Failed Validation: {argName}");
					return;
			}

			argVal = value;
			Console.WriteLine($"{argName} --- {(argName.Equals(F_PassWordArg, StringComparison.InvariantCultureIgnoreCase) ? "************" : $"{argVal}")}");
	}

	/// <summary>
	/// attempts to validate an argument
	/// </summary>
	/// <param name="argVal">value of argument to validate</param>
	/// <param name="argName">name of the argument we're attempting to validate</param>
	/// <param name="rawArgs">the unfiltered arg cmd to check</param>
	/// <param name="isOptional">if the arg is optional or not</param>
	static void ValidateArgument(out int argVal, string argName, Dictionary<string, string> rawArgs, bool isOptional = false)
	{
			argVal = -1;
			argName = argName.ToLowerInvariant();

			if (!rawArgs.ContainsKey(argName) && !isOptional)
			{
					F_FailedCliArgs.Add($"Failed Validation: {argName}");
					return;
			}

			if (string.IsNullOrWhiteSpace(rawArgs[argName]))
			{
					F_FailedCliArgs.Add($"Failed Validation: {argName}");
					return;
			}

			if (!int.TryParse(rawArgs[argName], out argVal))
			{
					F_FailedCliArgs.Add($"Failed Validation: {argName}");
					return;
			}

			Console.WriteLine($"{argName} --- {argVal}");
	}
#endregion

	/// <summary>
	/// Connection loop for easier retries
	/// </summary>
	/// <returns>false if we lose our connection</returns>
	static async Task<bool> RunChat(string userName, string passWord, string characterName, string startingChannel, string commandChar)
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

			// Add our plugins here ////////////////////////////////////////////////
#if DEBUG
			//F_Bot.AddPlugin(new GatchaGame(F_Chat, commandChar));
			//BOTACTION:TODO//F_Bot.AddPlugin(new LostRPG(F_Chat, commandChar));
			F_Bot.AddPlugin<FChatGlobalCommands>(new FChatGlobalCommands(F_Chat)
				.SetOperators(F_Owner,		Privilege.OwnerOperator)
				.SetOperators(F_GlobalOps,Privilege.GlobalOperator)
			);
			F_Bot.AddPlugin<FChatTournamentOrganiser>(new FChatTournamentOrganiser(F_Chat));
#else
			// F_Bot.AddPlugin(new FChatTournamentOrganiser(F_Chat));
#endif
			// End Plugin Adding ///////////////////////////////////////////////////

			F_CharacterName = characterName;

			F_Chat.ConnectedToChat                = ConnectedToChat;
			F_Chat.MessageHandler                 = HandleMessageReceived;
			F_Chat.JoinedChannelHandler           = HandleJoinedChannel;
			F_Chat.CreatedChannelHandler          = HandleCreatedChannel;
			F_Chat.LeftChannelHandler             = HandleLeftChannel;
			F_Chat.PrivateChannelsReceivedHandler = HandlePrivateChannelsReceived;
			F_Chat.PublicChannelsReceivedHandler  = HandlePublicChannelsReceived;

			await F_Chat.ConnectToChat(userName, passWord, characterName);
			F_StartingChannel = startingChannel;

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

	/// <summary>
	/// update loop, do w/e in here
	/// </summary>
	static async void Update()
	{
		await F_Bot!.Update();

		Task shutTask = null!;
		if (F_Bot != null && F_Bot!.ShutdownFlag)
		{
			shutTask = F_Bot.Shutdown();
		}

		////////////////////
		
	

		////////////////////

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
}