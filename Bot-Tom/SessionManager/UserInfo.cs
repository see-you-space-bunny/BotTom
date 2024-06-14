using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using Charsheet.MorkBorg;
using FileManip;

namespace BotTom.SessionManager;

internal class UserInfo() : IBinarySerializable
{
  internal UInt64 UserID = 0;
  internal Dictionary<ValidSerialTypes,ModuleInfo> ModuleInfo = [];
  internal Dictionary<string,(ValidSerialTypes ModuleType,string SearchableName,object Object)> Index = [];


  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    UserID = reader.ReadUInt64();
    for(int i = reader.ReadInt16();i>0;i--)
    {
      var moduleInfo = new ModuleInfo(this);
      (moduleInfo as IBinarySerializable).Deserialize(reader);
      ModuleInfo.Add(moduleInfo.ModuleType,moduleInfo);
    }

    for(int i = reader.ReadInt32();i>0;i--)
    {
      ValidSerialTypes moduleType = (ValidSerialTypes)reader.ReadInt16();
      string referenceUID = reader.ReadString();
      object obj; // TODO
      switch(moduleType)
      {
        case ValidSerialTypes.Scum:
        obj = new Scum(null,null,new int[]{});
        BinarySerializer.Deserialize<Scum>((Scum)obj,Path.Combine(Environment.CurrentDirectory,DefaultValues.SessionData,UserID.ToString(),referenceUID));
        break;
        default:
        obj = default!;
        break;
      }
      Index.Add(referenceUID,(
        moduleType,
        reader.ReadString(),
        obj
      ));
    }
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write((UInt64)UserID);
    writer.Write((Int16)ModuleInfo.Count);
    foreach(var key in ModuleInfo.Keys)
    {
      (ModuleInfo[key] as IBinarySerializable).Serialize(writer);
    }

    writer.Write((Int32)Index.Count);
    foreach(string key in Index.Keys)
    {
      writer.Write((Int16)Index[key].ModuleType);
      writer.Write(key);
      writer.Write(Index[key].SearchableName);
    }
  }
  #endregion
}