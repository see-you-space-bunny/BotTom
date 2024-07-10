using System;
using System.Reflection;
using WatsonWebsocket;
using System.Net.Http;
using FChatApi.Systems;
using FChatApi.Objects;
using System.Collections.Generic;
using System.IO;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region Fields (-)
	/// <summary>
	/// raw client assembly information from <c>Properties/AssemblyInfo.cs</c>
	/// </summary>
	private static AssemblyName _clientAssembly = Assembly.GetExecutingAssembly().GetName();
#endregion


#region Identification P(+)
	/// <summary>
	///name of the api as a chat-client<br/>
	/// <i>Required for chat server identification (<c>IDN</c>).</i>
	/// </summary>
	public static string ClientId { get; } =
		string.Format(
			"{0} | {1}",
			_clientAssembly.Name,
			_clientAssembly.FullName
		);

	/// <summary>
	/// version number of the api as a chat-client<br/>
	/// <i>Required for chat server identification (<c>IDN</c>).</i>
	/// </summary>
	public static string ClientVersion { get; } =
		string.Format(
			"{0}.{1}.{2}.{3} Build:{4}",
			_clientAssembly.Version.Major,
			_clientAssembly.Version.Minor,
			_clientAssembly.Version.MajorRevision,
			_clientAssembly.Version.MinorRevision,
			_clientAssembly.Version.Build
		);
	
	/// <summary>
	/// version number of the Api as a chat-client.<br/>
	/// <i>Required for chat server identification (<c>IDN</c>).</i>
	/// </summary>
	public static string UserName { get; private set; } = string.Empty;
	
	/// <summary>
	/// name of the character the api-user is logging in with.<br/>
	/// <i>Required for chat server identification (<c>IDN</c>).</i>
	/// </summary>
	public static string CharacterName { get; private set; } = string.Empty;
	
	/// <summary>
	/// name of the character the api-user is logging in with.<br/>
	/// <i>Required for chat server identification (<c>IDN</c>).</i>
	/// </summary>
	public static User ApiUser { get; private set; }
	
	/// <summary>
	/// the ticket information recieved from identification with the website<br/>
	/// <i>
	/// Required for chat server identification (<c>IDN</c>).<br/>
	/// Required for http requests to the website.
	/// </i>
	/// </summary>
	public static TicketInformation TicketInformation { get; private set; } = null;
#endregion


#region Cache P(+)
	private static string _cacheURL					= "sessioncache";
	private static string _allUsersURI				= "users";
	public static string CacheRoot { get; set; }	= Environment.CurrentDirectory;
	public static string CacheURL { get=>Path.Combine(CacheRoot,_cacheURL); set=>_cacheURL=value; }
	public static string CacheAllKnownUsersURI { get => Path.Combine(CacheRoot,_cacheURL,_allUsersURI); set=>_allUsersURI=value; }
#endregion


#region Web Clients P(+)
	/// <summary>
	/// the websocket client around which the chat-api is built
	/// </summary>
	public static WatsonWsClient Client { get; private set; } = null;

	/// <summary>
	/// the http client the api uses to handle requests to the website
	/// </summary>
	public static HttpClient HttpClient { get; } = new ();
#endregion


#region Timeout P(+)
	/// <summary>
	/// the api's default connection timeout duration
	/// </summary>
	public static TimeSpan DefaultConnectionTimeout { get; private set; } = new TimeSpan(0, 0, 10);

	/// <summary>
	/// the api's maximum allowable connection timeout duration in seconds
	/// </summary>
	private const ushort MaximumConnectionTimeoutValue = 30;

	/// <summary>
	/// the api's minimum allowable time in seconds a character may be timed out
	/// </summary>
	private const ushort MinimumChannelUserTimeoutValue = 1;

	/// <summary>
	/// the api's maximum allowable time in seconds a character may be timed out<br/>
	/// <i>this value should remain relatively if used for automated timeouts</i>
	/// </summary>
	private const ushort MaximumChannelUserTimeoutValue = 90;
#endregion


#region Instance Trackers P(-)
	/// <summary>
	/// the api's user tracker</i>
	/// </summary>
	public static UserTracker Users { get; set; } = new UserTracker();
	
	/// <summary>
	/// the api's channel tracker</i>
	/// </summary>
	public static ChannelTracker Channels { get; set; } = new ChannelTracker();
#endregion


#region Websocket Client P(+)
	/// <summary>
	/// URI pointing to the chat server's websocket
	/// </summary>
	public const string ChatWebsocketURI	= "wss://chat.f-list.net/chat2";
	
	/// <summary>
	/// URL pointing to the website
	/// </summary>
	public const string WebsiteURL			= "https://www.f-list.net/";

	/// <summary>
	/// URI pointing to the ticket request resource
	/// </summary>
	public const string TicketURI			= WebsiteURL + "json/getApiTicket.php";

	/// <summary>
	/// format string used to request ticket information <br/>
	/// &gt; 0. account name<br/>
	/// &gt; 1. account password<br/>
	/// &gt; 2. request no friends<br/>
	/// &gt; 3. request no bookmarks<br/>
	/// </summary>
	public const string TicketRequestFormat = TicketURI + "?account={0}&password={1}&no_friends={2}&no_bookmarks={3}";
#endregion
}