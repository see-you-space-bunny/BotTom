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
  public void TestBasicRoll()
  {
    var simpleRoll = DiceParser.BasicRoll("1d10");

    _output.WriteLine($"{simpleRoll.Item1} = `{simpleRoll.Item2:00}`");

    simpleRoll = DiceParser.BasicRoll("7d6");
    _output.WriteLine($"{simpleRoll.Item1} = `{simpleRoll.Item2:00}`");

    simpleRoll = DiceParser.BasicRoll("2d20kl1");
    _output.WriteLine($"{simpleRoll.Item1} = `{simpleRoll.Item2:00}`");
  }
}