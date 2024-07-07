using System;
using System.Text;
using WatsonWebsocket;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using FChatApi.Systems;
using FChatApi.Objects;
using FChatApi.Enums;
using FChatApi.EventArguments;

namespace FChatApi.Core;

public partial class ApiConnection
{
#region Constructor
	public ApiConnection()
	{ }
#endregion


///////////////////////////////////////////////////


#region MessageReceived (E)
	async void Client_MessageReceived(object sender, MessageReceivedEventArgs @event)
	{
		string message = Encoding.UTF8.GetString(@event.Data.ToArray());
		//Console.WriteLine($"Message from server: {message}");
		await ParseMessage(Enum.Parse<MessageCode>(message.Split(' ').First()), message.Split(" ".ToCharArray(), 2).Last());
	}
#endregion

#region ChatDisConnected (E)
	void Client_ChatDisconnected(object sender, EventArgs eventArgs)
	{
		Console.WriteLine("Disconnected from F-Chat servers!");
	}
#endregion

#region ChatConnected (E)
	async void Client_ChatConnected(object sender, EventArgs eventArgs)
	{
		Console.WriteLine("Connected to F-Chat servers! Sending identification...");
		await IdentifySelf(UserName, TicketInformation.Ticket, CharacterName, ClientId, ClientVersion);
		StartReplyThread(20);
	}
#endregion


///////////////////////////////////////////////////



#region IdentifySelf
	async static Task IdentifySelf(string accountName, string ticket, string botName, string botClientID, string botClientVersion)
	{
		string toSend = $"{MessageCode.IDN}  {{ \"method\": \"ticket\", \"account\": \"{accountName}\", \"ticket\": \"{ticket}\", \"character\": \"{botName}\", \"cname\": \"{botClientID}\", \"cversion\": \"{botClientVersion}\" }}";
		await Client.SendAsync(toSend);
	}
#endregion


///////////////////////////////////////////////////


#region ParseToJObject
	static JObject ParseToJObject(string message, MessageCode hycybh)
	{
		JObject returnCarrier;

		try
		{
			if (message.Split(' ').Length <= 1)
			{
				if (string.Equals(hycybh.ToString(), message))
				{
					return null;
				}

				returnCarrier = JObject.Parse(message);
			}
			else
			{
				returnCarrier = JObject.Parse(message.Replace(hycybh.ToString(), "").TrimStart());
			}
		}
		catch
		{
			throw new Exception($"Failure to parse message: {message}");
		}

		return returnCarrier;
	}
#endregion

	///////////////////////////////////////////////////
	///////////////////////////////////////////////////


#region ParseMessage
	async Task ParseMessage(MessageCode hycybh, string message)
	{
		JObject json;
		try
		{
			json = ParseToJObject(message, hycybh);
		}
		catch(Exception)
		{
			json = null;
		}

		List<Task> tasks = [];
		switch (hycybh)
		{
#region STA
			case MessageCode.STA:
				{
					tasks.Add(Task.Run(() =>
						{
							if (json["character"].ToString().Equals(CharacterName, StringComparison.InvariantCultureIgnoreCase))
							{
								Console.WriteLine("Status Changed");
							}
						}
					));
				}
				break;
#endregion

#region IDN
			case MessageCode.IDN:
				{
					ConnectedToChat?.Invoke(this, null);

					tasks.Add(RequestInternalChannelList(ChannelType.Private));
					tasks.Add(RequestInternalChannelList(ChannelType.Public));

					Console.WriteLine("Connected to Chat");
				}
				break;
#endregion

#region ORS
			case MessageCode.ORS:
				{
					List<Channel> privateChannelList = [];
					foreach (var channel in json["channels"])
					{
						privateChannelList.Add(new Channel(channel["title"].ToString(), channel["name"].ToString(), ChannelType.Private));
					}

					tasks.Add(Task.Run(() =>
						{
							ChannelTracker.RefreshAvailableChannels(privateChannelList, ChannelType.Private);
							PrivateChannelsReceivedHandler?.Invoke(this, new ChannelEventArgs() { });
							Console.WriteLine($"Private Channels Recieved... {privateChannelList.Count} total Private Channels.");
						}
					));
				}
				break;
#endregion

#region CHA
			case MessageCode.CHA:
				{
					List<Channel> publicChannelList = [];
					foreach (var channel in json["channels"])
					{
						string name = channel["name"].ToString();
						publicChannelList.Add(new Channel(name, name, ChannelType.Public));
					}

					tasks.Add(Task.Run(() =>
						{
							ChannelTracker.RefreshAvailableChannels(publicChannelList, ChannelType.Public);
							PublicChannelsReceivedHandler?.Invoke(this, new ChannelEventArgs() { });
							Console.WriteLine($"Public Channels Recieved... {publicChannelList.Count} total Public Channels.");
						}
					));
				}
				break;
#endregion

#region MSG
			case MessageCode.MSG:
				{
					MessageHandler?.Invoke(MessageCode.MSG, new MessageEventArgs() { channel = json["channel"].ToString(), message = json["message"].ToString(), user = json["character"].ToString() });
					Console.WriteLine(message);
				}
				break;
#endregion

#region PRI
			case MessageCode.PRI:
				{
					MessageHandler?.Invoke(MessageCode.PRI, new MessageEventArgs() { user = json["character"].ToString(), message = json["message"].ToString() });
					Console.WriteLine(message);
				}
				break;
#endregion

#region JCH
			case MessageCode.JCH:
				{
					User user = UserTracker.GetUserByName(json["character"]["identity"].ToString());
					Channel channel;
					try
					{
						channel = ChannelTracker.GetChannelByNameOrCode(json["title"].ToString());
					}
					catch
					{
						channel = ChannelTracker.AddManualChannel(json["title"].ToString(), ChannelStatus.Available, json["channel"].ToString());
					}

					if (user.Name.Equals(CharacterName))
					{
						ChannelTracker.WatchChannels.Add(channel.Code,channel);
					}

					if (channel == null)
					{
						return;
					}

					// creating channel
					bool creating = channel.Status == ChannelStatus.Creating && user.Name.Equals(CharacterName);
					if (creating)
					{
						channel = ChannelTracker.FinalizeChannelCreation(json["title"].ToString(), json["channel"].ToString(), user);
						Console.WriteLine($"Created Channel: {json["channel"]}");
						CreatedChannelHandler?.Invoke(this, new ChannelEventArgs() { name = channel.Name, status = ChannelStatus.Joined, code = channel.Code, type = channel.Type });
					}
					else
					{
						// join channel
						channel = ChannelTracker.GetChannelByNameOrCode(json["channel"].ToString());
						if (user != null && channel != null)
						{
							if (!creating)
							{
								JoinedChannelHandler?.Invoke(this, new ChannelEventArgs() { name = json["title"].ToString(), status = ChannelStatus.Joined, code = channel.Code, type = channel.Type, userJoining = user.Name });
							}

							tasks.Add(Task.Run(() =>
								{
									channel.AddUser(user);
									Console.WriteLine($"{user.Name} joined Channel: {channel.Name}. {channel.Users.Count} total users in channel.");
								}
							));
						}
					}
				}
				break;
#endregion

#region LCH
			case MessageCode.LCH:
				{
					if (json["character"].ToString().Equals(CharacterName, StringComparison.InvariantCultureIgnoreCase))
					{
						LeftChannelHandler?.Invoke(this, new ChannelEventArgs() { name = json["channel"].ToString(), status = ChannelStatus.Left });
						ChannelTracker.ChangeChannelStatus(json["channel"].ToString(), ChannelStatus.Available);
					}

					User user = UserTracker.GetUserByName(json["character"].ToString());
					Channel channel = ChannelTracker.GetChannelByNameOrCode(json["channel"].ToString());

					if (user.Name.Equals(CharacterName))
					{
						ChannelTracker.WatchChannels.Remove(channel.Code);
					}

					if (user != null && channel != null)
					{

						tasks.Add(Task.Run(() =>
							{
								channel.RemoveUser(user);
								Console.WriteLine($"{user.Name} left Channel: {json["channel"]}. {channel.Users.Count} total users in channel.");
							}
						));
					}

				}
				break;
#endregion

#region PIN
			case MessageCode.PIN:
				{
					tasks.Add(Client.SendAsync(MessageCode.PIN.ToString()));
				}
				break;
#endregion

#region VAR
			case MessageCode.VAR:
				{

				}
				break;
#endregion

#region HLO
			case MessageCode.HLO:
				{

				}
				break;
#endregion

#region CON
			case MessageCode.CON:
				{
					tasks.Add(Task.Run(() => Console.WriteLine($"{json["count"]} connected users sent.")));
				}
				break;
#endregion

#region FRL
			case MessageCode.FRL:
				{
					// friends list
				}
				break;
#endregion

#region IGN
			case MessageCode.IGN:
				{
					// ignore list
				}
				break;
#endregion

#region ADL
			case MessageCode.ADL:
				{

				}
				break;
#endregion

#region LIS
			case MessageCode.LIS:
				{
					foreach(var userinfo in json["characters"])
					{
						User tempUser = new()
						{
							Name		= userinfo[0].ToString(),
							Gender		= userinfo[1].ToString(),
							ChatStatus	= (ChatStatus)Enum.Parse(typeof(ChatStatus), userinfo[2].ToString().ToLowerInvariant(), true)
						};
						if (!UserTracker.TryAddUser(tempUser))
						{
						tasks.Add(Task.Run(
								() => UserTracker
									.GetUserByName(tempUser.Name)
									.Update(tempUser)));
						}
						//UserTracker.SetChatStatus(tempUser, tempUser.ChatStatus, false);
					}
#if DEBUG
					tasks.Add(Task.Run(() => Console.WriteLine($"Added {json["characters"].Count()} users. Total users: {UserTracker.GetNumberActiveUsers()}")));
#endif
				}
				break;
#endregion

#region NLN
			case MessageCode.NLN:
				{
					User tempUser = new()
					{
						Name = json["identity"].ToString(),
						UserStatus = (UserStatus)Enum.Parse(typeof(UserStatus), json["status"].ToString().ToLowerInvariant(), true),
						Gender = json["gender"].ToString()
					};

					tasks.Add(Task.Run(() => UserTracker.SetChatStatus(tempUser, ChatStatus.Online, false)));
				}
				break;
#endregion

#region COL
			case MessageCode.COL:
				{
					Channel channel = ChannelTracker.GetChannelByNameOrCode(json["channel"].ToString());

					bool ownerAdded = false;
					foreach (string username in json["oplist"].Select(user => user.ToString()))
					{
						if (string.IsNullOrWhiteSpace(username))
						{
							continue;
						}

						User user = UserTracker.GetUserByName(username);

						if (!ownerAdded)
						{
							channel.Owner	= user;
							ownerAdded		= true;
						}

						tasks.Add(Task.Run(() => channel.AddMod(user)));
					}
#if DEBUG
					tasks.Add(Task.Run(() => Console.WriteLine($"Found {channel.Mods.Count} mods for channel: {channel.Name}")));
#endif
				}
				break;
#endregion

#region FLN
			case MessageCode.FLN:
				{
					User user = UserTracker.GetUserByName(json["character"].ToString());
					UserTracker.SetChatStatus(user, ChatStatus.Offline, false);

					tasks.Add(Task.Run(() =>
						{
							foreach (var channel in ChannelTracker.WatchChannels.Values)
							{
								bool needsRemoved = false;

								if (channel.Users.ContainsKey(user.Name))
								{
									needsRemoved = true;
								}

								if (needsRemoved)
								{
									channel.RemoveUser(user);
								}
							}
						}
					));
				}
				break;
#endregion

#region ICH
			case MessageCode.ICH:
				{
					// joining channel
					Channel channel = ChannelTracker.ChangeChannelStatus(json["channel"].ToString(), ChannelStatus.Joined);

					foreach (User user in json["users"].Select(u => UserTracker.GetUserByName(u["identity"].ToString())))
					{
						tasks.Add(Task.Run(() =>
							{
								if (null == user)
								{
									Console.WriteLine($"Error attempting to add user {user.Name} to {channel.Name} channel's userlist.");
								}

								channel.AddUser(user);
								channel.AdEnabled = !json["mode"].ToString().Equals("chat");
							}
						));
					}
#if DEBUG
					tasks.Add(Task.Run(() => Console.WriteLine($"Adding {json["users"].Count()} users to {channel.Name} channel's userlist successful.")));
#endif
				}
				break;
#endregion

#region CDS
			case MessageCode.CDS:
				{
					tasks.Add(Task.Run(() => ChannelTracker.GetChannelByNameOrCode(json["channel"].ToString()).Description = json["description"].ToString()));
				}
				break;
#endregion

#region default
			default:
				{

				}
				break;
#endregion
		}
		await Task.WhenAll([.. tasks]);
	}
#endregion
}