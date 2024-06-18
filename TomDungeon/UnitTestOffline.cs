using BotTom.DiceRoller;
using BotTom.DiceRoller.GameSystems;
using Xunit.Abstractions;

namespace TomDungeon;

public class UnitTestOffline(ITestOutputHelper output)
{
  private readonly ITestOutputHelper _output = output;

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

  [Fact]
  public void TestBasicRoll_ExplodeDice()
  {
    var simpleRoll = DiceParser.BasicRoll("8d4e");
    Assert.Fail("Not Implemented");
  }

  [Fact]
  public void TestBasicRoll_RecursiveExplodeDice()
  {
    var simpleRoll = DiceParser.BasicRoll("12d6x");
    Assert.Fail("Not Implemented");
  }

  [Fact]
  public void TestLegentRoll_Basic()
  {
    var legendRoll = new LegendFiveRingsRoll(ringDice: 4, skillDice: 4, null);
    legendRoll.Roll();
    _output.WriteLine(legendRoll.ToString());
    Assert.Fail("Show me the output!");
  }
}