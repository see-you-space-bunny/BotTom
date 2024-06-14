using System.Runtime.Serialization;
using FileManip;

namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
internal struct Weapon() : IItem
{
  internal ItemTypes ItemType = ItemTypes.Weapon;
  internal string Name = string.Empty;
  internal string Description = string.Empty;
  internal int DamageDieCount = 1;
  internal int DamageDieSize = 6;
  internal int DamageModifier = 0;
  internal AbilityScores DamageStatUse = AbilityScores.Strength;
  internal bool IsCustom = false;
  internal int ReferenceUID = 0;

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    IsCustom = reader.ReadBoolean();
    if(!IsCustom)
    {
      Name = reader.ReadString();
      Description = reader.ReadString();
      DamageDieCount = (int)reader.ReadInt16();
      DamageDieSize = (int)reader.ReadInt16();
      DamageModifier = (int)reader.ReadInt16();
      DamageStatUse = (AbilityScores)reader.ReadInt16();
    }
    else
    {
      ReferenceUID = (int)reader.ReadInt32();
    }
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write((Int16)ItemType);
    writer.Write((bool)IsCustom);
    if(!IsCustom)
    {
      writer.Write(Name);
      writer.Write(Description);
      writer.Write((Int16)DamageDieCount);
      writer.Write((Int16)DamageDieSize);
      writer.Write((Int16)DamageModifier);
      writer.Write((Int16)DamageStatUse);
    }
    else
    {
      writer.Write((Int32)ReferenceUID);
    }
  }
  #endregion
}
