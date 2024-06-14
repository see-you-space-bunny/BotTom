using System.Xml;
using Xunit.Abstractions;
using FileManip;
using Charsheet.MorkBorg;

namespace TomDungeon;

public class UnitTestCharsheet(ITestOutputHelper output)
{
  private readonly ITestOutputHelper _output = output;

  [Fact]
  public void TestPlaceholder()
  {
    _output.WriteLine("Populate this with useful information.");
    //Assert.Fail("placeholder");
  }

  [Fact]
  public void TestBinarySerializer()
  {
    string name = "Binary Scum";
    string background = "Ones and Zeroes, bby!";
    string weaponName = "Dicer";
    int weaponDamageDieSize = 8;
    string fileName = Path.Combine(Environment.CurrentDirectory,"TestData","TestBinaryScum");
    var saveScum = new Scum(){Name = name,};
    saveScum.AbilityScores[AbilityScores.Agility] = new AbilityScore(AbilityScores.Agility,0);
    saveScum.AbilityScores[AbilityScores.Strength] = new AbilityScore(AbilityScores.Strength,1);
    saveScum.AbilityScores[AbilityScores.Toughness] = new AbilityScore(AbilityScores.Toughness,3);
    saveScum.AbilityScores[AbilityScores.Presence] = new AbilityScore(AbilityScores.Presence,5);
    saveScum.Background.Details = background;
    saveScum.Equipment.Items.Add(new InventoryItem(new Weapon(){Name=weaponName,DamageDieSize=weaponDamageDieSize}));
    BinarySerializer.Serialize(saveScum,fileName);
    var scum = new Scum();
    BinarySerializer.Deserialize(scum,fileName);
    Assert.Equal(name, scum.Name);
    Assert.Equal(0, scum.Version);
    Assert.Equal(0, scum.AbilityScores[AbilityScores.Agility].Value);
    Assert.Equal(1, scum.AbilityScores[AbilityScores.Strength].Value);
    Assert.Equal(3, scum.AbilityScores[AbilityScores.Toughness].Value);
    Assert.Equal(5, scum.AbilityScores[AbilityScores.Presence].Value);
    Assert.Equal(background, scum.Background.Details);
    Assert.Single(scum.Equipment.Items);
    Assert.Single(scum.Equipment.Weapons);
    Assert.Empty(scum.Equipment.Armors);
    Assert.Empty(scum.Feats);
    Assert.Equal(weaponName,((Weapon)scum.Equipment.Weapons.First().Details).Name);
    Assert.Equal(weaponDamageDieSize,((Weapon)scum.Equipment.Weapons.First().Details).DamageDieSize);
  }

  [Fact]
  public void TestDataContractDesrializer()
  {
    string name = "Data Scum";
    string fileName = Path.Combine(Environment.CurrentDirectory,"TestData","TestDataContractScum.xml");
    Scum scum = XmlContractSerializer.Deserialize<Scum>(fileName);
    Assert.Equal(name, scum.Name);
    Assert.Equal(0, scum.AbilityScores[AbilityScores.Agility].Value);
    Assert.Equal(1, scum.AbilityScores[AbilityScores.Strength].Value);
    Assert.Equal(3, scum.AbilityScores[AbilityScores.Toughness].Value);
    Assert.Equal(5, scum.AbilityScores[AbilityScores.Presence].Value);
  }

  [Fact]
  public void TestDataContractSerializer()
  {
    string name = "Data Scum";
    string fileName = Path.Combine(Environment.CurrentDirectory,"TestData","TestDataContractScum.xml");
    var saveScum = new Scum(){Name = name,};
    saveScum.AbilityScores[AbilityScores.Agility] = new AbilityScore(AbilityScores.Agility,0);
    saveScum.AbilityScores[AbilityScores.Strength] = new AbilityScore(AbilityScores.Strength,1);
    saveScum.AbilityScores[AbilityScores.Toughness] = new AbilityScore(AbilityScores.Toughness,3);
    saveScum.AbilityScores[AbilityScores.Presence] = new AbilityScore(AbilityScores.Presence,5);
    saveScum.XmlSerialize(fileName);
    Assert.True(File.Exists(fileName));
  }
}
