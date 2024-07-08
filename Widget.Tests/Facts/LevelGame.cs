using Xunit.Abstractions;
using Widget.LevelGame;

namespace Widget.Tests.Facts;

public class @LevelGame(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;
	
		[Fact]
		public void TestHumanXmlDeserialization()
		{
				string filePath = Path.Combine(Environment.CurrentDirectory,"xml","CharacterClasses.xml");
				var characterClassInfo = HumanXmlDeserializer.GetClasses(filePath);
				Assert.NotEmpty(characterClassInfo);
		}
}