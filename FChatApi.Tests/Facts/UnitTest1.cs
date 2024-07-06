using Xunit.Abstractions;

namespace FChatApi.Tests.Facts;

public class UnitTest1(ITestOutputHelper output)
{
  private readonly ITestOutputHelper _output = output;

  [Fact]
  public void BasicPass()
  {
    Assert.True(true);
  }
}