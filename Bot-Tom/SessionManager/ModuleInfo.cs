using FileManip;

namespace BotTom.SessionManager;

internal class ModuleInfo(UserInfo parent) : IBinarySerializable
{
  private UserInfo _parent = parent;

  internal ValidSerialTypes ModuleType = ValidSerialTypes.None;
  internal int SaveSlotsMaximum = 5;
  internal int SaveSlotsCurrent = 0;
  internal IEnumerable<KeyValuePair<string,(ValidSerialTypes ModuleType,string SearchableName,object Object)>> Index =>
    _parent.Index.Where((li)=>li.Value.ModuleType==ModuleType);

  #region IBinarySerializable
  void IBinarySerializable.Deserialize(BinaryReader reader)
  {
    ModuleType = (ValidSerialTypes)reader.ReadInt16();
    SaveSlotsMaximum = reader.ReadInt16();
    SaveSlotsCurrent = reader.ReadInt16();
  }

  void IBinarySerializable.Serialize(BinaryWriter writer)
  {
    writer.Write((Int16)ModuleType);
    writer.Write((Int16)SaveSlotsMaximum);
    writer.Write((Int16)SaveSlotsCurrent);
  }
  #endregion
}
