using System.Runtime.Serialization;
using System.Xml;

namespace FileManip;

public static class XmlContractSerializer
{
  #region Xml Conversion
  public static void ConvertBinaryToXml<T>(T serializable, string fileNameBinary,string fileNameXml) where T : IBinarySerializable
  {
    BinarySerializer.Deserialize<T>(serializable, fileNameBinary);
    serializable.XmlSerialize<T>(fileNameXml);
  }
  #endregion

  #region Xml Serialization
  public static T Deserialize<T>(string fileName)
  {
    T serializable;

    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
    using(FileStream fileStream = new FileStream(fileName, FileMode.Open))
    {
        serializable = (T) (serializer.ReadObject(fileStream) ?? throw new NullReferenceException($"Failed to deserialize {typeof(T)} from {fileName}"));
    }
    return serializable;
  }

  public static void XmlSerialize<T>(this T serializable, string fileName) => Serialize(serializable, fileName);

  public static void Serialize<T>(T serializable, string fileName)
  {
    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
    using(var writer = XmlWriter.Create(fileName,new XmlWriterSettings(){Indent = true, IndentChars = "\t"}))
    {
      serializer.WriteObject(writer, serializable);
    }
  }
  #endregion
}
