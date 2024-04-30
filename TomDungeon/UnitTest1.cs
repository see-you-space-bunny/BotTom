using Xunit.Abstractions;

namespace BotTom;

public class UnitTest1
{
  private ITestOutputHelper _output;

  public UnitTest1(ITestOutputHelper output)
  {
      _output = output;
  }

  [Fact]
  public void Test1()
  {
    var scvm = new MakeSCVM("");
    _output.WriteLine(scvm.XmlTest);
  }

  [Fact]
  public void Test2()
  {
    var scvm = new MakeSCVM("");
    _output.WriteLine(scvm.XmlTest);
  }
}