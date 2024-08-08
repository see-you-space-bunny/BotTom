using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FChatApi.Core;
using FChatApi.Objects;
using RoleplayingGame.Objects;

namespace RoleplayingGame.Systems;

public class CharacterTracker
{
#region Fields
	private readonly ConcurrentDictionary<User,CharacterSheet> PlayerCharacters;
	private readonly ConcurrentDictionary<string,CharacterSheet> OrphanCharacters;
#endregion


#region Properties
	public int Count => PlayerCharacters.Count + OrphanCharacters.Count;
#endregion


#region Lookup > By User
	public CharacterSheet SingleByUser(User value)
	{
		if (!PlayerCharacters.TryGetValue(value,out CharacterSheet ?result))
		{
			if (OrphanCharacters.TryGetValue(value.Name,out result!))
			{
				TryAdoptCharacter(value);
			}
		}
		if (result is null)
			throw new KeyNotFoundException();
		return result;
	}
	public bool TrySingleByUser(User value,out CharacterSheet result)
	{
		if (PlayerCharacters.TryGetValue(value,out result!))
		{
			return true;
		}
		else if (OrphanCharacters.TryGetValue(value.Name,out result!))
		{
			TryAdoptCharacter(value);
		}
		return false;
	}
#endregion


#region Lookup > By Id
	public CharacterSheet SingleById(ulong value)
	{
		CharacterSheet	result	=	PlayerCharacters.Values.FirstOrDefault(c=>c.ActorId==value)!;
		if (result is null)
		{
			result	=	OrphanCharacters.Values.FirstOrDefault(c=>c.ActorId==value)!;
			if (result is not null)
			{
				TryAdoptCharacter(result.User);
			}
		}
		if (result is null)
			throw new KeyNotFoundException();
		return result;
	}
	public bool TrySingleById(ulong value,out CharacterSheet result)
	{
		result	=	PlayerCharacters.Values.FirstOrDefault(c=>c.ActorId==value)!;
		if (result is not null)
		{
			return true;
		}
		if (result is not null)
		{
			result	=	OrphanCharacters.Values.FirstOrDefault(c=>c.ActorId==value)!;
			if (result is not null)
			{
				TryAdoptCharacter(result.User);
				return true;
			}
		}
		return false;
	}
#endregion


#region Lookup > By Name
	public CharacterSheet SingleByName(string value)
	{
		var result = PlayerCharacters.Values.SingleOrDefault(pc=>pc.CharacterName == value);
		if (result is null)
		{
			OrphanCharacters.TryGetValue(value,out result);
			if (result is not null)
			{
				TryAdoptCharacter(ApiConnection.Users.SingleByName(value));
			}
		}
		if (result is null)
			throw new KeyNotFoundException();
		return result;
	}
	public bool TrySingleByName(string value,out CharacterSheet result)
	{
		result = PlayerCharacters.Values.SingleOrDefault(pc=>pc.CharacterName == value)!;
		if (result is not null)
		{
			return true;
		}
		else if (OrphanCharacters.TryGetValue(value,out result!))
		{
			TryAdoptCharacter(ApiConnection.Users.SingleByName(value));
			return true;
		}
		return false;
	}
#endregion


#region ContainsKey
	public bool ContainsKey(User value) =>
		PlayerCharacters.ContainsKey(value) || OrphanCharacters.ContainsKey(value.Name);

	public bool ContainsKey(string value) =>
		ApiConnection.Users.TrySingleByName(value,out User user) && ContainsKey(user);
#endregion


#region Adoption / Creation
	public bool TryCreateCharacter(User user)
	{
		if (!PlayerCharacters.ContainsKey(user))
		{
			if (!TryAdoptCharacter(user))
			{
				PlayerCharacters.AddOrUpdate(user,(k)=>new CharacterSheet(user),(k,v)=>new CharacterSheet(user));
				return true;
			}
		}
		return false;
	}

	private bool TryAdoptCharacter(User user)
	{
		if (OrphanCharacters.ContainsKey(user.Name))
		{
			OrphanCharacters.Remove(user.Name,out CharacterSheet ?adoptedCharacter);
			PlayerCharacters.AddOrUpdate(user,(k)=>adoptedCharacter?.BecomeAdopted(user)!,(k,v)=>adoptedCharacter?.BecomeAdopted(user)!);
			return true;
		}
		return false;
	}
#endregion


#region Initialize
	internal void Initialize()
	{
		foreach (CharacterSheet characterSheet in PlayerCharacters.Values)
		{
			characterSheet.Initialize(this);
		}
		foreach (CharacterSheet characterSheet in OrphanCharacters.Values)
		{
			characterSheet.Initialize(this);
		}
	}
#endregion


#region Serialization
	internal static CharacterTracker Deserialize(BinaryReader reader,CharacterClassTracker CharacterClasses)
	{
		CharacterTracker result = new ();
		for (int n=0;n<2;n++)
			for (int i=0;i<reader.ReadUInt32();i++)
			{
				var playerCharacter = CharacterSheet.Deserialize(reader,CharacterClasses);
				if (ApiConnection.Users.TrySingleByName(playerCharacter.CharacterName,out User user))
				{
					result.PlayerCharacters.AddOrUpdate(user,(k)=>playerCharacter,(k,v)=>playerCharacter);
				}
				else
				{
					result.OrphanCharacters.AddOrUpdate(playerCharacter.CharacterName,(k)=>playerCharacter,(k,v)=>playerCharacter);
				}
			}
		return result;
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write((uint)	PlayerCharacters.Count);
		foreach (CharacterSheet playerCharacter in PlayerCharacters.Values)
		{
			playerCharacter.Serialize(writer);
		}
		writer.Write((uint)	OrphanCharacters.Count);
		foreach (CharacterSheet orphanCharacter in OrphanCharacters.Values)
		{
			orphanCharacter.Serialize(writer);
		}
	}
#endregion


#region Constructor
	internal CharacterTracker()
	{
		PlayerCharacters	= new ConcurrentDictionary<User,CharacterSheet>();
		OrphanCharacters	= new ConcurrentDictionary<string,CharacterSheet>(StringComparer.InvariantCultureIgnoreCase);
	}
#endregion
}