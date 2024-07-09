using System.Text;

namespace FChatApi.Tests.LabAssitant;

public class FakeMessageReceivedEventArgs(string data)
{
    public ArraySegment<byte> Data => Encoding.ASCII.GetBytes(data);
}