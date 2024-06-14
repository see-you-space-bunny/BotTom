using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace FileManip;

public static class BinarySerializer
{
  #region Binary Conversion
  public static void ConvertBinaryToXml<T>(string fileNameXml,string fileNameBinary)
  {
    var serializable = (IBinarySerializable)(XmlContractSerializer.Deserialize<T>(fileNameBinary) ?? throw new NullReferenceException($"Could not deserialize {fileNameXml}"));
    Serialize(serializable,fileNameXml);
  }
  #endregion

  #region Binary Serialization
  public static void Deserialize<T>(T serializable,string fileName)
  {
    if(!typeof(T).GetInterfaces().Contains(typeof(IBinarySerializable)))
      throw new ArgumentException($"{typeof(T)} {nameof(serializable)} is does not implement {typeof(IBinarySerializable)}");

    using(var reader = new BinaryReader(File.Open(fileName,FileMode.Open),Encoding.UTF8,false))
    {
      (serializable as IBinarySerializable)!.Deserialize(reader);
    }
  }
  public static void Serialize(IBinarySerializable serializable,string fileName)
  {
    try
    {
      using(BinaryWriter writer = new BinaryWriter(File.Open(fileName,FileMode.Create),Encoding.UTF8,false))
      {
        serializable.Serialize(writer);
      }
    }
    catch (Exception e)
    {
      Debug.Write(e);
    }
  }
  #endregion
}

