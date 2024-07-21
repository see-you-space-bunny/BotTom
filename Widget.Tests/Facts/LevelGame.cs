using Xunit.Abstractions;

using LevelGame.Objects;
using LevelGame.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using LevelGame.SheetComponents;
using LevelGame.Enums;

namespace Widget.Tests.Facts;

public class @LevelGame(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;

    static readonly string XmlDirectory;
	
	[Fact]
	public void TestReadableDeserializer()
	{
		string className = "Adventurer";

		IEnumerable<CharacterClass> characterClasses = HumanXmlDeserializer.GetClasses(Path.Combine(XmlDirectory,$"CharacterClasses.xml"));

		Assert.Contains(characterClasses, c => c.TextName == className);
		
		CharacterClass @class = characterClasses.First(c => c.TextName == className);

		Assert.Equal(100.0f,	@class.ResourceModifiers[Resource.Health][ResourceModifier.BaseValue]);
		Assert.Equal(0.75f,		@class.ResourceModifiers[Resource.Health][ResourceModifier.SoftLimit]);
		Assert.Equal(2.00f,		@class.ResourceModifiers[Resource.Health][ResourceModifier.HardLimit]);

		Assert.Equal(0.25f,		@class.ResourceAbilityScales[Resource.Health][Ability.Power]);
		Assert.Equal(0.65f,		@class.ResourceAbilityScales[Resource.Health][Ability.Body]);
		Assert.Equal(0.10f,		@class.ResourceAbilityScales[Resource.Health][Ability.Reflex]);
	}

	private static void DirectorySanityCheck()
	{
		if (!Directory.Exists(XmlDirectory))
			Directory.CreateDirectory(XmlDirectory);
	}

	private static void Serialize<T>(T serializable, string fileName)
	{
		DataContractSerializer serializer = new DataContractSerializer(typeof(T));
		using(var writer = XmlWriter.Create(fileName,new XmlWriterSettings(){Indent = true, IndentChars = "\t"}))
		{
			serializer.WriteObject(writer, serializable);
		}
	}

	static @LevelGame()
	{
		XmlDirectory = Path.Combine(Environment.CurrentDirectory,"xml");
		DirectorySanityCheck();
	}
}