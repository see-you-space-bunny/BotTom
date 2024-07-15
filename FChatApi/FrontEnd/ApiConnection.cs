using System;
using System.Collections.Generic;
using System.Linq;
using WatsonWebsocket;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using FChatApi.Systems;
using FChatApi.Objects;
using FChatApi.Enums;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region GetMyCharacterList
	/// <summary>
	/// attempts to retrieve the api-user's character list from the ticket information
	/// </summary>
	/// <exception cref="NullReferenceException">if the api contains no valid ticket information</exception>
	/// <returns>a list of character names</returns>
	public static List<string> GetMyCharacterList() =>
		TicketInformation is not null ?
			[.. TicketInformation.Characters] :
			throw new NullReferenceException("You must acquire a ticket before attempting to retrieve a character list.");
#endregion

#region GetTicketInformation
	/// <summary>
	/// attempts to retrieve ticket information from the website
	/// </summary>
	/// <param name="username">account username</param>
	/// <param name="password">account password</param>
	/// <returns>ticket information</returns>
	public static async Task<TicketInformation> GetTicketInformation(string username, string password) =>
		await HttpClient
			.GetFromJsonAsync<TicketInformation>(
				string.Format(
					TicketRequestFormat,
					username,
					password,
					true,
					true
				)
			);
#endregion

#region ConnectToChat
	/// <summary>
	/// attempts to connect the api to the chat server with the provided information
	/// </summary>
	/// <param name="username">account username</param>
	/// <param name="password">account password</param>
	/// <param name="charactername">name of api-user's character to log in with</param>
	/// <param name="connectionTimeout">how long to wait until connection attempt times out</param>
	/// <exception cref="Exception">if the client failed to start, or api could not properly dispose of the previous client</exception>
	/// <exception cref="ArgumentException">if the connection timeout argument is too large, or if attempting to log into a character not associated with api-user's account</exception>
	/// <returns></returns>
	public async Task ConnectToChat(string username, string password, string charactername, TimeSpan? connectionTimeout = null)
	{
		if (connectionTimeout is not null && ((TimeSpan)connectionTimeout).TotalSeconds > MaximumConnectionTimeoutValue)
		{
			throw new ArgumentException($"Connection timeout may not be longer than {MaximumConnectionTimeoutValue} seconds");
		}

		Task<TicketInformation> ticketTask = GetTicketInformation(username, password);

		if (Client is not null)
		{
			try
			{
				Client.Dispose();
				Client	= null;
			}
			catch (Exception e)
			{
				throw new Exception($"Error disposing of previous client: {e}");
			}
		}

		Client	= new WatsonWsClient(new Uri(ChatWebsocketURI));

		Client.ServerConnected		+= Client_ChatConnected;
		Client.ServerDisconnected	+= Client_ChatDisconnected;
		Client.MessageReceived		+= Client_MessageReceived;

		TicketInformation	= await ticketTask;
		UserName			= username;
		CharacterName		= GetMyCharacterList().FirstOrDefault(c=>c.Equals(charactername,StringComparison.InvariantCultureIgnoreCase));

		if (CharacterName == default)
		{
			throw new ArgumentException("Cannot log into a character not on your character list.");
		}

		DeserializeUsers();
		
		if (!Users.TrySingleByName(CharacterName,out User user))
		{
			user = new User(){ Name = CharacterName, ChatStatus = ChatStatus.Online };
			Users.AddOrUpdate(user);
		}
		ApiUser = user;

		try
		{
			Client.Start();
		}
		catch (Exception e)
		{
			throw new Exception($"Error connecting to chat: {e}");
		}

		connectionTimeout ??= DefaultConnectionTimeout;
#if DEBUG
		Console.WriteLine(NoticeBanner);
		Console.WriteLine("Client successfully started!");
		Console.WriteLine($"Connection timeout set to {((TimeSpan)connectionTimeout).TotalSeconds} seconds.");
		Console.WriteLine(GenericBanner);
#endif
		DateTime whenToTimeout = DateTime.Now + (TimeSpan)connectionTimeout;
		while (!Client.Connected && DateTime.Now < whenToTimeout);
	}
#endregion

#region IsConnected
	/// <summary>
	/// checks if a client exists and that that client is connected
	/// </summary>
	/// <returns>the api's connection status</returns>
	public static bool IsConnected() => Client is not null && Client.Connected;

	/// <summary>
	/// used to ensure that the client waits to send messages before being affirmed as logged in
	/// </summary>
	/// <returns>the api-user's logged-in status</returns>
	public bool IsLoggedIn { get; private set; } = false;
#endregion
}