using Xunit.Abstractions;

using RoleplayingGame.Objects;
using RoleplayingGame.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using RoleplayingGame.SheetComponents;
using RoleplayingGame.Enums;

namespace Widget.Tests.Facts;

public class @RoleplayingGame(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;

    static readonly string CsvDirectory;
	
	[Fact]
	public void TestReadableDeserializer()
	{
		string className = "Adventurer";

		List<CharacterClass> characterClasses = DeserializeKommaVaues.GetClasses(Path.Combine(CsvDirectory,$"CharacterClasses - Export.csv"));

		Assert.Contains(characterClasses, c => c.ToString() == className);
		
		CharacterClass @class = characterClasses.First(c => c.ToString() == className);

		Assert.Equal(100.0f,	@class.ResourceModifiers	[Resource.Health][ResourceModifier.BaseValue]);
		Assert.Equal(1.75f,		@class.ResourceModifiers	[Resource.Health][ResourceModifier.HardLimit]);

		//Assert.Equal(0.25f,		@class.ResourceAbilityScales[Resource.Health][Ability.Power]);
		//Assert.Equal(0.70f,		@class.ResourceAbilityScales[Resource.Health][Ability.Body]);
		//Assert.Equal(0.00f,		@class.ResourceAbilityScales[Resource.Health][Ability.Reflex]);
		Assert.Equal(0.05f,		@class.ResourceAbilityScales[Resource.Health][Ability.Luck]);
	}

	private static void DirectorySanityCheck()
	{
		if (!Directory.Exists(CsvDirectory))
			Directory.CreateDirectory(CsvDirectory);
	}

	private static void Serialize<T>(T serializable, string fileName)
	{
		DataContractSerializer serializer = new DataContractSerializer(typeof(T));
		using(var writer = XmlWriter.Create(fileName,new XmlWriterSettings(){Indent = true, IndentChars = "\t"}))
		{
			serializer.WriteObject(writer, serializable);
		}
	}

	static @RoleplayingGame()
	{
		CsvDirectory = Path.Combine(Environment.CurrentDirectory,"csv");
		DirectorySanityCheck();
	}
}