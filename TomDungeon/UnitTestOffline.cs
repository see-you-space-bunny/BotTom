using Xunit.Abstractions;

namespace BotTom;

public class UnitTestOffline
{
  private ITestOutputHelper _output;

  public UnitTestOffline(ITestOutputHelper output)
  {
      _output = output;
  }

  [Fact]
  public void TestBasicRoll_SingleDie()
  {
    var simpleRoll = DiceParser.BasicRoll("1d10");
    Assert.InRange(simpleRoll.Item2, 1, 10);
  }

  [Fact]
  public void TestBasicRoll_MultipleDice()
  {
    var simpleRoll = DiceParser.BasicRoll("7d6");
    Assert.InRange(simpleRoll.Item2, 7, 42);
  }

  [Fact]
  public void TestBasicRoll_KeepLowDice()
  {
    var simpleRoll = DiceParser.BasicRoll("2d20kl1");
    Assert.InRange(simpleRoll.Item2, 1, 20);
  }

  [Fact]
  public void TestBasicRoll_KeepHighDice()
  {
    var simpleRoll = DiceParser.BasicRoll("2d12kh1");
    Assert.InRange(simpleRoll.Item2, 1, 12);
  }
}