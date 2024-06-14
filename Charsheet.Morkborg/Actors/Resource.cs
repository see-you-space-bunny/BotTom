using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using FileManip;

namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
internal class Resource(Resources resourceType, int maximum, int? current = null) : IBinarySerializable
{
  [DataMember(Name = "Type")]
  internal Resources ResourceType { get; private set; } = resourceType;

  [DataMember]
  internal int Maximum = maximum;

  [DataMember]
  internal int Current = current ?? maximum;

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    ResourceType = (Resources)reader.ReadInt16();
    Maximum = reader.ReadInt32();
    Current = reader.ReadInt32();
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write((Int16)ResourceType);
    writer.Write((Int32)Maximum);
    writer.Write((Int32)Current);
  }
  #endregion
}
