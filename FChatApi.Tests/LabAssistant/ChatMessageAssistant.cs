using Newtonsoft.Json.Linq;

using FChatApi.EventArguments;
using FChatApi.Objects;

namespace FChatApi.Tests.LabAssitant;

internal static class ChatMessageAssistant
{
	static ChatMessageAssistant()
	{
		MessageBatches = new Dictionary<MessageBatch,List<string>>();
		
		if (!File.Exists(Path.Combine(Environment.CurrentDirectory,"TestData","messagebatch")))
			throw new FileNotFoundException();

		JObject json = JObject.Parse(File.ReadAllText(Path.Combine(Environment.CurrentDirectory,"TestData","messagebatch")));

		foreach ((string key,JToken? token) in json)
		{
			if (!Enum.TryParse(key, true, out MessageBatch batch))
				continue;

			MessageBatches.Add(batch,[]);
			foreach (JToken? message in json[key]!)
			{
				if (message["body"] is not null)
					MessageBatches[batch].Add(string.Format("{0} {1}",message["code"]!.ToString(),message["body"]!.ToString().Replace("\n",string.Empty)));
				else
					MessageBatches[batch].Add(message["code"]!.ToString());
			}
		}
	}

	public static string F_StartingChannel = "adh-testchannel";

	public static Dictionary<MessageBatch,List<string>> MessageBatches;

	public static void ConnectedToChat(object sender, ChannelEventArgs @event)
	{ }
	
	public static void HandleMessageReceived(object sender, FChatMessage @event)
	{ }

	public static void HandleJoinedChannel(object sender, ChannelEventArgs @event)
	{ }

	public static void HandleCreatedChannel(object sender, ChannelEventArgs @event)
	{ }
	
	public static void HandleLeftChannel(object sender, ChannelEventArgs @event)
	{ }

	public /*async*/ static void HandlePrivateChannelsReceived(object sender, ChannelEventArgs @event)
	{
		/// check and join starting channel here
		
		//	var privateChannels = ApiConnection.GetChannelListByType(ChannelType.Private);
		//	if (privateChannels.Any(x => x.Code.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)))
		//	{
		//		await ApiConnection.User_JoinChannel(F_StartingChannel);
		//	}
		//	else if (privateChannels.Any(x => x.Name.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)))
		//	{
		//		await ApiConnection.User_JoinChannel(privateChannels.First(x => x.Name.Equals(F_StartingChannel, StringComparison.InvariantCultureIgnoreCase)).Code);
		//	}
		Assert.True(true);
	}

	public static void HandlePublicChannelsReceived(object sender, ChannelEventArgs @event)
	{ }
}

public enum MessageBatch
{
	First,
	Second,
	Third,
	Random,
}