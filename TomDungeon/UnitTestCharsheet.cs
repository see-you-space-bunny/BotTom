using System.Xml;
using Xunit.Abstractions;
using Charsheet.MorkBorg.Actors;
using System.Xml.Serialization;

namespace Charsheet;

public class UnitTestCharsheet(ITestOutputHelper output)
{
  private readonly ITestOutputHelper _output = output;

  [Fact]
  public void TestPlaceholder()
  {
    _output.WriteLine("Populate this with useful information.");
    Assert.Fail("placeholder");
  }

  [Fact]
  public void TestMorkBorgWriteScum()
  {
    string fileName = Path.Combine(Environment.CurrentDirectory,"MorkBorg","TestWriteScum.xml");
    using(var writer = XmlWriter.Create(fileName,new XmlWriterSettings() { Indent = true }))
    {
      var scum = new Scum();
      (scum as IXmlSerializable).WriteXml(writer);
    }
  }

  [Fact]
  public void TestMorkBorgReadScum()
  {
    string fileName = Path.Combine(Environment.CurrentDirectory,"MorkBorg","TestReadScum.xml");
    if(File.Exists(fileName))
    {
      using(var reader = XmlReader.Create(fileName))
      {
        var scum = new Scum();
        (scum as IXmlSerializable).ReadXml(reader);
        Assert.Equal("Named Scum",scum.Name);
        Assert.Equal(0,scum.Version);
        Assert.Equal(0,scum.AbilityScores[MorkBorg.Enum.AbilityScores.Agility].Value);
        Assert.Equal(1,scum.AbilityScores[MorkBorg.Enum.AbilityScores.Strength].Value);
        Assert.Equal(2,scum.AbilityScores[MorkBorg.Enum.AbilityScores.Toughness].Value);
        Assert.Equal(3,scum.AbilityScores[MorkBorg.Enum.AbilityScores.Presence].Value);
        Assert.Equal(4,scum.HitPoints.Maximum);
        Assert.Equal(2,scum.HitPoints.Current);
        Assert.Equal(2,scum.Omens.Maximum);
        Assert.Equal(1,scum.Omens.Current);
      }
    }
  }
}
