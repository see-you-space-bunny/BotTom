using System.Collections.Concurrent;
using FChatApi.Attributes;
using LevelGame.Attributes;
using LevelGame.Enums;
using LevelGame.Serialization;
using LevelGame.SheetComponents;

namespace LevelGame.Core;

public static class World
{
    static readonly string XmlDirectory;
    internal static readonly ConcurrentDictionary<ClassName,CharacterClass> CharacterClasses;


	public static void LoadClasses(string filePath) => LoadClasses(XmlDirectory,filePath);

	public static void LoadClasses(string directory,string filePath)
	{
		foreach (CharacterClass @class in HumanXmlDeserializer.GetClasses(Path.Combine(directory,filePath)))
		{
			CharacterClasses.AddOrUpdate(@class.Name,(k)=>@class,(k,v)=>@class);
		}
	}


	private static void DirectorySanityCheck()
	{
		if (!Directory.Exists(XmlDirectory))
			Directory.CreateDirectory(XmlDirectory);
	}

	static World()
	{
		AttributeEnumExtensions.ProcessEnumForAttribute<DefaultModifierAttribute>(typeof(ResourceModifier));
		AttributeEnumExtensions.ProcessEnumForAttribute<GameFlagsAttribute		>(typeof(Resource));
		AttributeEnumExtensions.ProcessEnumForAttribute<XmlKeyAttribute			>(typeof(Resource));
		CharacterClasses = [];
		XmlDirectory = Path.Combine(Environment.CurrentDirectory,"xml");
		DirectorySanityCheck();
	}
}
