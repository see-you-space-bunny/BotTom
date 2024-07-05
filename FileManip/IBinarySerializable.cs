namespace FileManip;

public interface IBinarySerializable
{
  public void Deserialize(BinaryReader reader);
  public void Serialize(BinaryWriter writer);
}

[AttributeUsage(AttributeTargets.Property)]
public class BSerializeThisAttribute(uint inOrder) : Attribute
{
  private readonly uint positionInSerialization = inOrder;
  public uint PositionInSerialization => positionInSerialization;
  public BSerializeThisAttribute() : this(0) { }
}

