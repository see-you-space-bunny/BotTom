using Tommy;

namespace BotTom;

static class ControlPanel
{
	static ControlPanel()
	{
		_configFilePath = "settings.toml";
		PrivateGuilds = [];
	}

	#region F(-)
	private static readonly string _configFilePath;
	#endregion

	#region F(~)
	internal static readonly Dictionary<ulong,string> PrivateGuilds;
	#endregion

	#region P(~) DUMMY FIELDS
	internal static string FieldA { get; set; } = "test";
	internal static long FieldB { get; set; } = 10;
	internal static bool FieldC { get; set; } = true;
	#endregion

	#region M(~)
	internal static void LoadFileConfig()
	{
		if(!File.Exists(_configFilePath))
			return;
		
		using(StreamReader reader = File.OpenText(_configFilePath))
		{
			TomlTable table = TOML.Parse(reader);
			LoadGuilds(table);
		}
	}

	private static void LoadGuilds(TomlTable table)
	{
		foreach(TomlTable guild in table["guilds"])
		{
			var guildTable = guild.AsTable;
			if(guildTable["private"].AsBoolean)
			{
				ulong id = UInt64.Parse(guildTable["id"].AsString);
				string name = guildTable["name"].AsString;
				PrivateGuilds.Add(id,name);
			}
		}
	}
	#endregion
}