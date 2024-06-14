using System.Runtime.Serialization;
using FileManip;

namespace Charsheet.MorkBorg;

[DataContract(Namespace = "")]
internal struct CreatorCredit(string name, string url, string? comment = null) : IBinarySerializable
{
  string Name = name;
  string Url = url;
  string Comment = comment ?? string.Empty;

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    Name = reader.ReadString();
    Url = reader.ReadString();
    Comment = reader.ReadString();
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write(Name);
    writer.Write(Url);
    writer.Write(Comment);
  }
  #endregion
}
