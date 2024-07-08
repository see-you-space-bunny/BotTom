using Xunit.Abstractions;

namespace Engine.Tests.Facts;

public class @Serialization(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;

	[Fact]
	public void BasicPass()
	{
		Assert.True(true);
	}
}
