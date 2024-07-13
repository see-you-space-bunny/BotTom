using System;
using System.IO;
using System.Linq;
using FChatApi.Objects;

namespace FChatApi.Core;

public partial class ApiConnection
{
	private static void LoadCache()
	{

	}


	public static void DeserializeUsers()
	{
		if (!File.Exists(CacheAllKnownUsersURI))
			return;

		using FileStream stream	= File.OpenRead(CacheAllKnownUsersURI);
		BinaryReader reader		= new (stream);
		uint count = reader.ReadUInt32();

		if (count == 0)
			return;

		for (uint i = 0; i < count; i++)
		{
			Users.Add(User.Deserialize(reader));
		}
	}

	public static void SerializeUsers()
	{
		if (!Directory.Exists(CacheURL))
			Directory.CreateDirectory(CacheURL);

		if (Users.KnownUsers.IsEmpty)
		{
			File.Delete(CacheAllKnownUsersURI);
			return;
		}

        using FileStream stream	= File.Create(CacheAllKnownUsersURI);
        BinaryWriter writer		= new (stream);
        writer.Write((uint)Users.KnownUsers.Count);
        foreach (User user in Users.KnownUsers.Values)
        {
            user.Serialize(writer);
        }
    }
}