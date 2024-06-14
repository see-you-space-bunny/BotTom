namespace FileManip;

public interface IBinarySerializable
{
  public void Deserialize(BinaryReader reader);
  public void Serialize(BinaryWriter writer);
}

