using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using WatsonWebsocket;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using FChatApi.Systems;
using FChatApi.Objects;

namespace FChatApi.Core;

public partial class ApiConnection
{
    public static WatsonWsClient Client { get; set; } = null;
    public static TicketInformation TicketInformation { get; set; } = null;
    public static string UserName { get; set; } = string.Empty;
    public static string CharacterName { get; set; } = string.Empty;
    public static string ClientId { get; set; } = "Fii_Bot";
    public static string ClientVersion { get; set; } = "2.1.0.0";
    public static TimeSpan ConnectionTimeout { get; set; } = new TimeSpan(0, 0, 10);
    public static UserTracker UserTracker { get; set; } = new UserTracker();
    public static ChannelTracker ChannelTracker { get; set; } = new ChannelTracker();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<string> GetCharacterList()
    {
        if (TicketInformation != null)
        {
            return [.. TicketInformation.Characters];
        }
        throw new Exception("You must acquire a ticket before attempting to retrieve a character list.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static async Task<TicketInformation> GetTicketInformation(string username, string password)
    {
        try
        {
            // fchat login info and url
            string fchatURI = "https://www.f-list.net/json/getApiTicket.php";
            string completeString = $"{fchatURI}?account={username}&password={password}&no_friends=true&no_bookmarks=true";

            using(var client = new HttpClient())
            {
                TicketInformation = await client.GetFromJsonAsync<TicketInformation>(completeString);
            }
            UserName = username;
            return TicketInformation;
        }
        catch(Exception e)
        {
            throw new Exception($"Failure obtaining ticket: {e}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="charactername"></param>
    /// <returns></returns>
    public async Task<bool> ConnectToChat(string username, string password, string charactername)
    {
        TicketInformation = await GetTicketInformation(username, password);

        if (TicketInformation == null)
        {
            throw new Exception("Error obtaining ticket information");
        }

        try
        {
            if (Client != null)
            {
                Client.Dispose();
                Client = null;
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Error disposing of previous client: {e}");
        }
        try
        {
            UserName = username;
            CharacterName = charactername;
            Client = new WatsonWsClient(new Uri("wss://chat.f-list.net/chat2"));
            Client.ServerConnected += Client_ChatConnected;
            Client.ServerDisconnected += Client_ChatDisConnected;
            Client.MessageReceived += Client_MessageReceived;
            Client.Start();
        }
        catch (Exception e)
        {
            throw new Exception($"Error connecting to chat: {e}");
        }
        DateTime whenToTimeout = DateTime.Now + ConnectionTimeout;
        while (!Client.Connected && DateTime.Now < whenToTimeout);
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static bool IsConnected()
    {
        if (Client != null)
        {
            return Client.Connected;
        }

        return false;
    }
}