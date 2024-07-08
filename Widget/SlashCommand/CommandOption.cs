namespace Widget.SlashCommand;
/**
internal class CommandOption<T>(string keyWord, string description, object? defaultValue, bool isRequired = false)
{
	internal bool IsRequired = isRequired;
	internal string KeyWord = keyWord;
	internal string Description => DefaultValue != null ? string.Format(DescriptionText, DefaultValue.ToString()) : DescriptionText;
	internal ApplicationCommandOptionType OptionType = typeof(T) switch {
		var type when type == typeof(double)          => ApplicationCommandOptionType.Number,
		var type when type == typeof(long)            => ApplicationCommandOptionType.Integer,
		var type when type == typeof(string)          => ApplicationCommandOptionType.String,
		var type when type == typeof(bool)            => ApplicationCommandOptionType.Boolean,
		var type when type == typeof(SocketGuildUser) => ApplicationCommandOptionType.User,
		_ => throw new Exception($"Option value type ({typeof(T)}) not accounted for in CommandOption"),
	};
	internal string DescriptionText = description;
	internal object? DefaultValue = defaultValue;

	internal bool Defaults(SocketSlashCommand command) => !command.Data.Options.Any((o)=>o.Name==KeyWord&&o.Value!=null);
	internal bool Defaults(SocketSlashCommandData data) => !data.Options.Any((o)=>o.Name==KeyWord&&o.Value!=null);
	internal bool Defaults(SocketSlashCommandDataOption option) => !option.Options.Any((o)=>o.Name==KeyWord&&o.Value!=null);
	
	internal T? GetValue(SocketSlashCommand command) => command.Data.Options
		.Where(o => o.Name == KeyWord)
		.Select(o => (T?)(o.Value??DefaultValue))
		.FirstOrDefault();

	internal T? GetValue(SocketSlashCommandData data) => data.Options
		.Where(o => o.Name == KeyWord)
		.Select(o => (T?)(o.Value??DefaultValue))
		.FirstOrDefault();
	
	internal T? GetValue(SocketSlashCommandDataOption option) => option.Options
		.Where(o => o.Name == KeyWord)
		.Select(o => (T?)(o.Value??DefaultValue))
		.FirstOrDefault();

	internal void AddOption(SlashCommandBuilder command) => command.AddOption(KeyWord,OptionType,Description,isRequired: IsRequired);

	internal void AddOption(SlashCommandOptionBuilder command) => command.AddOption(KeyWord,OptionType,Description,isRequired: IsRequired);
*/