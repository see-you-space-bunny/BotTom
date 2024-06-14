namespace BotTom.SessionManager;

public static class DefaultValues
{
  static DefaultValues()
  {
    SessionLibrary = new BotTom.SessionManager.SessionLibrary();
  }
  
  internal const string SessionData = "sessiondata";
  internal static BotTom.SessionManager.SessionLibrary SessionLibrary;
}