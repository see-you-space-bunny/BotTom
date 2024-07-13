#if DEBUG
using System;
using System.Threading.Tasks;
using FChatApi.Objects;
#endif

namespace FChatApi.Core;

public partial class ApiConnection
{
	internal const string GenericBanner	= "//////////////////////////////";
	internal const string ErrorBanner	= "////////////// ERROR   ///////";
	
#if DEBUG
	internal const string WarningBanner	= "////////////// WARNING ///////";
	internal const string NoticeBanner	= "////////////// NOTICE  ///////";

	private static async Task DebugSendAsync(string toSend)
	{
		if (!await Client.SendAsync(toSend))
		{
			Console.WriteLine(WarningBanner);
			Console.WriteLine($"Unable to send message: {toSend}");
			Console.WriteLine(GenericBanner);
		}
	}

	public static void DebugSetCharacterName(string value) =>
		CharacterName = value;

	public static void DebugAddCharacters(string[] value)
	{
		foreach (string name in value)
			Users.Add(new User() { Name = name, });
	}
#endif
}