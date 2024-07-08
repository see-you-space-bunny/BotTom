using Xunit.Abstractions;

namespace BotTom.Tests.Facts;

public class DiscordNET(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;

	[Fact]
	public void BasicPass()
	{
		Assert.True(true);
	}
}