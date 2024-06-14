using System.Runtime.Serialization;
using FileManip;

namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
internal struct Feat() : IItem
{
  internal ItemTypes ItemType = ItemTypes.Feat;
  internal string Name = string.Empty;
  internal string Description = string.Empty;
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
    }
    else
    {
      writer.Write((Int32)ReferenceUID);
    }
  }
  #endregion
}
