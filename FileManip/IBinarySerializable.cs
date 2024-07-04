namespace FileManip;

public interface IBinarySerializable
{
  public static virtual T? Deserialize<T>(BinaryReader reader) => default!;
  public void Serialize(BinaryWriter writer);
}

[AttributeUsage(AttributeTargets.Property)]
public class BSerializeThisAttribute(uint inOrder) : Attribute
{
  private readonly uint positionInSerialization = inOrder;
  public uint PositionInSerialization => positionInSerialization;
  public BSerializeThisAttribute() : this(0) { }
}

