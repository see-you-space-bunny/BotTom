using FileManip;

namespace BotTom.SessionManager;

internal partial class SessionLibrary : IBinarySerializable
{
  internal SessionLibrary()
  {
    string sessionDataPath = Path.Combine(Environment.CurrentDirectory, DefaultValues.SessionData);
    
    if(!Directory.Exists(sessionDataPath))
      Directory.CreateDirectory(sessionDataPath);

    FullFilePath = Path.Combine(sessionDataPath, FileName);
    
    if(File.Exists(FullFilePath))
    {
      BinarySerializer.Deserialize(this,FullFilePath);
    }
  }

  internal const string FileName = "SessionLibrary";
  internal string FullFilePath;

  internal Dictionary<UInt64,UserInfo> UserInfo = [];

  internal void Update() => BinarySerializer.Serialize(this,FullFilePath);
  internal void Save() => BinarySerializer.Serialize(this,FullFilePath);

  internal void RegisterAndSave(IBinarySerializable serializeable, UInt64 userID, string guid, string searchableName)
  {
    if(TryValidateSerialType(serializeable.GetType(), out var serialType))
    {
      string filePath = Path.Combine(Environment.CurrentDirectory,DefaultValues.SessionData,userID.ToString(),guid);
      UserInfo userInfo = new UserInfo(){UserID=userID};
      
      if(!UserInfo.Keys.Contains(userID))
        UserInfo[userID] = userInfo;
        
      if(UserInfo[userID].ModuleInfo.Keys.Contains(serialType))
        UserInfo[userID].ModuleInfo[serialType].SaveSlotsCurrent++;
      else
        UserInfo[userID].ModuleInfo.Add(serialType,new ModuleInfo(UserInfo[userID]){ModuleType=serialType,SaveSlotsCurrent=1});

      if(UserInfo[userID].Index.Keys.Contains(guid))
        UserInfo[userID].Index[guid] = (serialType,searchableName,serializeable);
      else
        UserInfo[userID].Index.Add(guid,(serialType,searchableName,serializeable));

      if(UserInfo[userID].ModuleInfo[serialType].SaveSlotsCurrent > UserInfo[userID].ModuleInfo[serialType].SaveSlotsMaximum)
      {
        UserInfo[userID].ModuleInfo[serialType].SaveSlotsCurrent = UserInfo[userID].ModuleInfo[serialType].SaveSlotsMaximum;
        throw new Exception($"User is at maximum allowable save slots {UserInfo[userID].ModuleInfo[serialType].SaveSlotsCurrent}/{UserInfo[userID].ModuleInfo[serialType].SaveSlotsMaximum}.\n> Data has been __discarded__!");
      }

      BinarySerializer.Serialize(serializeable,filePath);
      Save();
    }
    else
      throw new Exception($"Could not validate the SerialType of '{serializeable.GetType()} serializeable'.\n> Data has been __discarded__!");
  }

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    UserInfo.Clear();
    for(int i = reader.ReadInt32(); i > 0; i--)
    {
      UserInfo userInfo = new UserInfo();
      (userInfo as IBinarySerializable).Deserialize(reader);
      UserInfo.Add(userInfo.UserID,userInfo);
    }
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write((Int32)UserInfo.Count);
    foreach(UserInfo userInfo in UserInfo.Values)
    {
      (userInfo as IBinarySerializable).Serialize(writer);
    }
  }
  #endregion
}

