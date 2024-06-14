using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.VisualBasic;
using FileManip;

namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
internal struct AbilityScore(AbilityScores abilityScore, int value) : IBinarySerializable
{

  #region P[~]
  [DataMember]
  internal AbilityScores EnumReference { get; set; } = abilityScore;

  internal readonly string Name => EnumReference.ToString();

  [DataMember]
  internal int Value { get; set; } = value;
  #endregion

  #region Operator ++
  public static AbilityScore operator ++(AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,abilityScore.Value+1);
  #endregion

  #region Operator --
  public static AbilityScore operator --(AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,abilityScore.Value+1);
  #endregion

  #region Operator +
  public static AbilityScore operator +(AbilityScore abilityScore,int value) =>
    new (abilityScore.EnumReference,abilityScore.Value+value);
  public static AbilityScore operator +(int value,AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,abilityScore.Value+value);
  #endregion

  #region Operator -
  public static AbilityScore operator -(AbilityScore abilityScore,int value) =>
    new (abilityScore.EnumReference,abilityScore.Value-value);
  public static AbilityScore operator -(int value,AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,value-abilityScore.Value);
  #endregion

  #region Operator *
  public static AbilityScore operator *(AbilityScore abilityScore,int value) =>
    new (abilityScore.EnumReference,abilityScore.Value*value);
  public static AbilityScore operator *(int value,AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,abilityScore.Value*value);
  #endregion

  #region Operator /
  public static AbilityScore operator /(AbilityScore abilityScore,int value) =>
    new (abilityScore.EnumReference,abilityScore.Value/value);
  public static AbilityScore operator /(int value,AbilityScore abilityScore) =>
    new (abilityScore.EnumReference,value/abilityScore.Value);
  #endregion

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    EnumReference = (AbilityScores)reader.ReadInt16();
    Value = (int)reader.ReadByte()-32;
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write((Int16)EnumReference);
    writer.Write((byte)(Value+32));
  }
  #endregion
}
