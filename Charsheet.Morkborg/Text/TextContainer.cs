using System.Runtime.Serialization;
using FileManip;

namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
internal class TextContainer : IBinarySerializable
{
  internal string Details = string.Empty;

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    Details = reader.ReadString();
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write(Details);
  }
  #endregion
}
