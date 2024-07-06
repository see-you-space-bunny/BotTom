using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Engine.Serialization.Interfaces;

public interface IBinarySerializable
{
  public void Deserialize(BinaryReader reader);
  public void Serialize(BinaryWriter writer);
}

