using Xunit.Abstractions;

using FChatApi.Attributes;

using CardGame.Enums;
using CardGame.Attributes;

namespace Widget.Tests.Facts;

public class @CardGame(ITestOutputHelper output)
{
	private readonly ITestOutputHelper _output = output;

		[Fact]
		public void TestAttributeHandler()
		{
				StatAliasAttribute statAliasLevel = CharacterStat.LVL.GetEnumAttribute<CharacterStat, StatAliasAttribute>();
				Assert.Equal(["Level","Lv"],statAliasLevel.Alias);

				StatAliasAttribute statAliasLuck = CharacterStat.LUC.GetEnumAttribute<CharacterStat, StatAliasAttribute>();
				Assert.Equal(["Luck"],statAliasLuck.Alias);
		}
}