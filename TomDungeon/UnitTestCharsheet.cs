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
    using (var writer = XmlWriter.Create(fileName,new XmlWriterSettings() { Indent = true }))
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
      using (var reader = XmlReader.Create(fileName))
      {
        var scum = new Scum();
        (scum as IXmlSerializable).ReadXml(reader);
        Assert.Equal("Named Scum",scum.Name);
      }
    }
  }
}
