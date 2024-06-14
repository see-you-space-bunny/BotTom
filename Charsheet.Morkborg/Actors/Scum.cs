using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FileManip;

[assembly:InternalsVisibleTo("TomDungeon")]
namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
public partial class Scum : IBinarySerializable
{
  #region (~) Constructor
  internal Scum()
  {
    AbilityScores.Add(MorkBorg.AbilityScores.Agility,  new AbilityScore(MorkBorg.AbilityScores.Agility,  DefaultValues.AbilityScore));
    AbilityScores.Add(MorkBorg.AbilityScores.Strength, new AbilityScore(MorkBorg.AbilityScores.Strength, DefaultValues.AbilityScore));
    AbilityScores.Add(MorkBorg.AbilityScores.Toughness,new AbilityScore(MorkBorg.AbilityScores.Toughness,DefaultValues.AbilityScore));
    AbilityScores.Add(MorkBorg.AbilityScores.Presence, new AbilityScore(MorkBorg.AbilityScores.Presence, DefaultValues.AbilityScore));
  }
  public Scum(
    string? name = null,
    string? background = null,
    AbilityScore[]? abilityScores = null,
    int? hitPoints = null,
    int? omens = null
  ) : base() 
  {
    Name = name ?? Name;
    Background.Details = background ?? Background.Details;
    if(abilityScores is not null)
      foreach(AbilityScore abilityScore in abilityScores)
        AbilityScores[abilityScore.EnumReference] = abilityScore;
    HitPoints.Maximum = hitPoints ?? HitPoints.Maximum;
    HitPoints.Current = hitPoints ?? HitPoints.Current;
    Omens.Maximum = omens ?? Omens.Maximum;
    Omens.Current = omens ?? Omens.Current;
  }
  public Scum(
    string? name = null,
    string? background = null,
    int[]? abilityScores = null,
    int? hitPoints = null,
    int? omens = null
  ) : base() 
  {
    Name = name ?? Name;
    Background.Details = background ?? Background.Details;
    if(abilityScores is not null && abilityScores.Length == 4)
    {
      AbilityScores[MorkBorg.AbilityScores.Agility  ] = new AbilityScore(MorkBorg.AbilityScores.Agility  ,abilityScores[0]);
      AbilityScores[MorkBorg.AbilityScores.Strength ] = new AbilityScore(MorkBorg.AbilityScores.Strength ,abilityScores[1]);
      AbilityScores[MorkBorg.AbilityScores.Toughness] = new AbilityScore(MorkBorg.AbilityScores.Toughness,abilityScores[2]);
      AbilityScores[MorkBorg.AbilityScores.Presence ] = new AbilityScore(MorkBorg.AbilityScores.Presence ,abilityScores[3]);
    }
    HitPoints.Maximum = hitPoints ?? HitPoints.Maximum;
    HitPoints.Current = hitPoints ?? HitPoints.Current;
    Omens.Maximum = omens ?? Omens.Maximum;
    Omens.Current = omens ?? Omens.Current;
  }
  #endregion

  #region P(+)

  [DataMember]
  internal Dictionary<AbilityScores,AbilityScore> AbilityScores { get; private set; } = [];

  [DataMember]
  internal TextContainer Background { get; private set; } = new ();

  [DataMember]
  internal CreatorCredit Credit { get; private set; } = new(string.Empty,string.Empty);

  [DataMember]
  internal Inventory Equipment { get; private set; } = new ();

  [DataMember]
  internal List<Feat>  Feats { get; private set; } = [];

  [DataMember]
  internal Resource HitPoints { get; private set; } = new (Resources.HitPoints,DefaultValues.HitPointsMaximum,DefaultValues.HitPointsCurrent);

  [DataMember]
  internal string Name { get; set; } = DefaultValues.ScumName;

  [DataMember]
  internal Resource Omens { get; private set; } = new (Resources.Omens,DefaultValues.OmensMaximum,DefaultValues.OmensCurrent);

  [DataMember]
  internal int Version { get; private set; } = DefaultValues.CurrentVersion;
  #endregion

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    Version = reader.ReadInt16();
    Name = reader.ReadString();
    (Background as IBinarySerializable).Deserialize(reader);
    (Credit as IBinarySerializable).Deserialize(reader);
    for(int i = reader.ReadInt16(); i > 0; i--)
    {
      IBinarySerializable abilityScore = new AbilityScore();
      abilityScore.Deserialize(reader);
      AbilityScores[((AbilityScore)abilityScore).EnumReference] = (AbilityScore)abilityScore;
    }
    (HitPoints as IBinarySerializable).Deserialize(reader);
    (Omens as IBinarySerializable).Deserialize(reader);
    (Equipment as IBinarySerializable).Deserialize(reader);
    for(int i = reader.ReadInt16(); i > 0; i--)
    {
      IBinarySerializable feat = new Feat();
      reader.ReadInt16();
      feat.Deserialize(reader);
      Feats.Add((Feat)feat);
    }
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write((short)Version);
    writer.Write(Name);
    (Background as IBinarySerializable).Serialize(writer);
    (Credit as IBinarySerializable).Serialize(writer);
    writer.Write((short)AbilityScores.Count);
    foreach(IBinarySerializable abilityScore in AbilityScores.Values.OfType<IBinarySerializable>())
      abilityScore.Serialize(writer);
    (HitPoints as IBinarySerializable).Serialize(writer);
    (Omens as IBinarySerializable).Serialize(writer);
    (Equipment as IBinarySerializable).Serialize(writer);
    writer.Write((short)Feats.Count);
    foreach(IBinarySerializable feat in Feats.OfType<IBinarySerializable>())
      feat.Serialize(writer);
  }
  #endregion

  #region ToString()
  public override string ToString()
  {
    StringBuilder sb = new();
    sb.AppendLine($"# {Name}");
    sb.AppendLine($"> *Class by [{Credit})*");
    sb.AppendLine($"Background: {Background}");
    sb.Append($"Strength {(AbilityScores[MorkBorg.AbilityScores.Strength].Value>=0?'+':string.Empty)}{AbilityScores[MorkBorg.AbilityScores.Strength].Value}, ");
    sb.Append($"Agility {(AbilityScores[MorkBorg.AbilityScores.Agility].Value>=0?'+':string.Empty)}{AbilityScores[MorkBorg.AbilityScores.Agility].Value}, ");
    sb.Append($"Presence {(AbilityScores[MorkBorg.AbilityScores.Presence].Value>=0?'+':string.Empty)}{AbilityScores[MorkBorg.AbilityScores.Presence].Value}, ");
    sb.AppendLine($"Toughness {(AbilityScores[MorkBorg.AbilityScores.Toughness].Value>=0?'+':string.Empty)}{AbilityScores[MorkBorg.AbilityScores.Toughness].Value}");
    sb.AppendLine($"{Omens.Current} Omen{(Omens.Current!=1?'s':string.Empty)} (d{Omens.Maximum}), {Equipment.Silver} Silver");
    sb.AppendLine($"Weapon {Equipment.Weapons}, Armor {Equipment.Armors}");
    return sb.ToString();
  }
  #endregion
}
