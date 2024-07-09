#define UNIT_TEST
using System.Text;

using Xunit.Abstractions;

using FChatApi.Core;
using FChatApi.Enums;
using FChatApi.Tests.LabAssitant;

namespace FChatApi.Tests.Theories;

public class UnitTestDummyMessages
{
    private readonly ITestOutputHelper _output;

    public UnitTestDummyMessages(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
	[InlineData(MessageBatch.First)]
	[InlineData(MessageBatch.Second)]
	[InlineData(MessageBatch.Third)]
	public async void BasicRecieveMessages(MessageBatch batch)
	{
		FileStream ostrm;
		StreamWriter writer;
		TextWriter oldOut = Console.Out;
		try
		{
			ostrm	= new FileStream (Path.Combine(Environment.CurrentDirectory,$"output-{batch}"), FileMode.OpenOrCreate, FileAccess.Write);
			writer	= new StreamWriter (ostrm);
		}
		catch (Exception e)
		{
			Console.WriteLine (e.Message);
			return;
		}
		Console.SetOut (writer);

        ApiConnection api = new ApiConnection
        {
            ConnectedToChat					= ChatMessageAssistant.ConnectedToChat,
            MessageHandler					= ChatMessageAssistant.HandleMessageReceived,
            JoinedChannelHandler			= ChatMessageAssistant.HandleJoinedChannel,
            CreatedChannelHandler			= ChatMessageAssistant.HandleCreatedChannel,
            LeftChannelHandler				= ChatMessageAssistant.HandleLeftChannel,
            PrivateChannelsReceivedHandler	= ChatMessageAssistant.HandlePrivateChannelsReceived,
            PublicChannelsReceivedHandler	= ChatMessageAssistant.HandlePublicChannelsReceived,
        };
		//ApiConnection.Client_ChatConnected();
        List<string> messages;
		if (batch == MessageBatch.Random)
		{
			messages = [];
		}
		else
		{
			messages = ChatMessageAssistant.MessageBatches[batch];
		}
		foreach (string message in messages)
		{
			var @event = new FakeMessageReceivedEventArgs(message);
			
			string decodedMessage = Encoding.UTF8.GetString(@event.Data.ToArray());
			Console.WriteLine($"Message from server: {decodedMessage}".Replace("\n",string.Empty));
			await api.ParseMessage(Enum.Parse<MessageCode>(decodedMessage.Split(' ').First()), decodedMessage.Split(" ".ToCharArray(), 2).Last());
		}
		Assert.True(true);
		Console.SetOut (oldOut);
		writer.Close();
		ostrm.Close();
		Console.WriteLine ("Done");
	}
}