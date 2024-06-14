using FileManip;

namespace BotTom.SessionManager;

internal enum ValidSerialTypes
{
    None = 0x00,
    SessionLibrary = 0x01,
    Scum = 0x02,
}

internal partial class SessionLibrary : IBinarySerializable
{
  internal bool TryValidateSerialType(Type type, out ValidSerialTypes serialType)
  {
    if(type == typeof(SessionLibrary))
    {
      serialType = ValidSerialTypes.SessionLibrary;
      return true;
    }

    if(type == typeof(Charsheet.MorkBorg.Scum))
    {
      serialType = ValidSerialTypes.Scum;
      return true;
    }

    serialType = ValidSerialTypes.None;
    return false;
  }
}
