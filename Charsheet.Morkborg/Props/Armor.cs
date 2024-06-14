using System.Runtime.Serialization;
using FileManip;

namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
internal struct Armor() : IItem
{
  internal ItemTypes ItemType = ItemTypes.Armor;
  internal string Name = string.Empty;
  internal string Description = string.Empty;
  internal int ProtectionDieCount = 1;
  internal int ProtectionDieSize = 2;
  internal int ProtectionModifier = 0;
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
      ProtectionDieCount = (int)reader.ReadInt16();
      ProtectionDieSize = (int)reader.ReadInt16();
      ProtectionModifier = (int)reader.ReadInt16();
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
      writer.Write((Int16)ProtectionDieCount);
      writer.Write((Int16)ProtectionDieSize);
      writer.Write((Int16)ProtectionModifier);
    }
    else
    {
      writer.Write((Int32)ReferenceUID);
    }
  }
  #endregion
}
