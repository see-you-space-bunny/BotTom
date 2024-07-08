using System.Diagnostics;
using System.Text;
using Engine.Serialization.Interfaces;

namespace Engine.Serialization;

public static class MyBinarySerializer
{
	#region Binary Conversion
	public static void ConvertBinaryToXml<T>(string fileNameXml,string fileNameBinary) where T : IBinarySerializable
	{
		var serializable = XmlContractSerializer.Deserialize<T>(fileNameBinary) ?? throw new NullReferenceException($"Could not deserialize {fileNameXml}");
		Serialize(serializable,fileNameXml);
	}
	#endregion

	#region Binary Serialization
	public static void Deserialize<T>(T serializable,string fileName) where T : IBinarySerializable
	{
		using(var reader = new BinaryReader(File.Open(fileName,FileMode.Open),Encoding.UTF8,false))
		{
			serializable.Deserialize(reader);
		}
	}

	public static void Serialize<T>(T serializable,string fileName) where T : IBinarySerializable
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

