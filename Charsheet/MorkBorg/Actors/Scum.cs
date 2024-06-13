using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Charsheet.MorkBorg.Enum;
using Charsheet.MorkBorg.Props;
using Charsheet.MorkBorg.Text;

[assembly:InternalsVisibleTo("TomDungeon")]
namespace Charsheet.MorkBorg.Actors;

internal partial class Scum : IXmlSerializable
{
  #region (~) Constructor
  internal Scum()
  {
    AbilityScores.Add(Enum.AbilityScores.Agility,new AbilityScore(Enum.AbilityScores.Agility,System.MorkBorg.DefaultAbilityScore));
    AbilityScores.Add(Enum.AbilityScores.Strength,new AbilityScore(Enum.AbilityScores.Strength,System.MorkBorg.DefaultAbilityScore));
    AbilityScores.Add(Enum.AbilityScores.Toughness,new AbilityScore(Enum.AbilityScores.Toughness,System.MorkBorg.DefaultAbilityScore));
    AbilityScores.Add(Enum.AbilityScores.Presence,new AbilityScore(Enum.AbilityScores.Presence,System.MorkBorg.DefaultAbilityScore));
  }
  #endregion

  #region F(-)
  private readonly Dictionary<AbilityScores,AbilityScore> _abilityScores = [];
  private readonly TextContainer _background = new ();
  private readonly Inventory _equipment = new ();
  private readonly Inventory _feats = new ();
  private readonly Resource _hitPoints = new (System.MorkBorg.DefaultHitPointsMaximum,System.MorkBorg.DefaultHitPointsCurrent);
  private readonly Resource _omens = new (System.MorkBorg.DefaultOmensMaximum,System.MorkBorg.DefaultOmensCurrent);
  #endregion

  #region P(+)
  internal Dictionary<AbilityScores,AbilityScore> AbilityScores => _abilityScores;
  internal TextContainer Background => _background;
  internal CreatorCredit Credit { get; set; } = new();
  internal Inventory Equipment => _equipment;
  internal Inventory Feats => _feats;
  internal Resource HitPoints => _hitPoints;
  internal string Name { get; set; } = System.MorkBorg.DefaultName;
  internal Resource Omens => _omens;
  internal int Version { get; set; } = System.MorkBorg.CurrentVersion;
  #endregion
  
  #region IXmlSerializable
  XmlSchema? IXmlSerializable.GetSchema() => null;

  void IXmlSerializable.ReadXml(XmlReader reader)
  {
    if(reader.MoveToContent() == XmlNodeType.Element && reader.LocalName.Equals("Scum", StringComparison.CurrentCultureIgnoreCase))
    {
      switch(Int32.Parse(reader.GetAttribute("Version") ?? "0"))
      {
        case 0: DeserializerV0(reader); break;
      }
    }
  }

  void IXmlSerializable.WriteXml(XmlWriter writer)
  {
    switch(Version)
    {
      case 0: SerializeV0(writer); break;
    }
  }
  #endregion

  #region ToString()
  public override string ToString()
  {
    return base.ToString() ?? "Scum";
  }
  #endregion

  #region Serial v0
  private void DeserializerV0(XmlReader reader)
  {
    Name = reader.GetAttribute("Name") ?? System.MorkBorg.DefaultName;

    if(reader.ReadToDescendant("AbilityScore"))
    {
      int i = 0;
      List<Enum.AbilityScores> scoresAlreadyLoaded = [];
      while(
        reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "AbilityScore"
        && i < System.MorkBorg.MaxAbilityScores
        && scoresAlreadyLoaded.Count < AbilityScores.Count
        )
      {
        if(Enum.AbilityScores.TryParse<AbilityScores>(reader["Name"], true, out AbilityScores key))
          AbilityScores[key] = new (key, Int32.Parse(reader["Value"] ?? "0"));
        scoresAlreadyLoaded.Add(key);
        ++i;
      }
    }

    if(reader.ReadToNextSibling("HitPoints"))
    {
      if(reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "AbilityScore")
      {
        _hitPoints.Maximum = Int32.Parse(reader["Maximum"] ?? $"{System.MorkBorg.DefaultHitPointsMaximum}");
        _hitPoints.Current = Int32.Parse(reader["Current"] ?? $"{System.MorkBorg.DefaultHitPointsCurrent}");
      }
    }
    
    if(reader.ReadToNextSibling("Omens"))
    {
      if(reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "AbilityScore")
      {
        _omens.Maximum = Int32.Parse((reader["Die"]    ?? $"d{System.MorkBorg.DefaultOmensMaximum}")[1..]);
        _omens.Current = Int32.Parse(reader["Current"] ??  $"{System.MorkBorg.DefaultOmensCurrent}");
      }
    }
  }

  private void SerializeV0(XmlWriter writer)
  {
    writer.WriteStartElement("Scum");
    writer.WriteAttributeString("Name",Name);
    writer.WriteAttributeString("Version",Version.ToString());

    foreach(AbilityScore abilityScore in AbilityScores.Values)
    {
        writer.WriteStartElement("AbilityScore");
        writer.WriteAttributeString("Name",abilityScore.Name);
        writer.WriteAttributeString("Value",abilityScore.Value.ToString());
        writer.WriteEndElement();
    }

    writer.WriteStartElement("HitPoints");
    writer.WriteAttributeString("Maximum",HitPoints.Maximum.ToString());
    writer.WriteAttributeString("Current",HitPoints.Current.ToString());
    writer.WriteEndElement();

    writer.WriteStartElement("Omens");
    writer.WriteAttributeString("Die",$"d{Omens.Maximum}");
    writer.WriteAttributeString("Current",Omens.Current.ToString());
    writer.WriteEndElement();


    writer.WriteEndElement();
  }
  #endregion
}
