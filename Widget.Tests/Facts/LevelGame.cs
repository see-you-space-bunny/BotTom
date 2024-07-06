using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Widget.LevelGame;
using Widget.LevelGame.Enums;

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