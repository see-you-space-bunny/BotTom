using Xunit.Abstractions;
using Xunit.Sdk;

namespace BotTom;

public class UnitTestMorkBorg
{
  private ITestOutputHelper _output;

  private string[] _BackgroundResults = {
    "TextTextText1",
    "TextTextText2",
    "TextTextText3",
    "TextTextText4",
    "TextTextText5",
    "TextTextText6"
  };

  private string[] _NameResults = {
    "Cursed Skinwalker",
    "TextTextText"
  };

  public UnitTestMorkBorg(ITestOutputHelper output)
  {
      _output = output;
  }

  [Fact]
  public void TestClassName()
  {
    var scvm = new MakeSCVM();
    Assert.Contains(scvm.Name,_NameResults);
    _output.WriteLine(scvm.Name);
  }

  [Fact]
  public void TestRandomBackground()
  {
    var scvm = new MakeSCVM();
    Assert.Contains(scvm.Background,_BackgroundResults);
    _output.WriteLine(scvm.Background);
  }
}