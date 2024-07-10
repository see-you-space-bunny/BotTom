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


	private static void DeserializeUsers()
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

	private static void SerializeUsers()
	{
		if (Users.KnownUsers.Count == 0)
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