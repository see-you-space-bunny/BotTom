using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Charsheet.ForgedInTheDark;
using Xunit;

namespace TomDungeon;

public class UnitTestClockCabinet
{
    [Fact]
    public void TestSerialize()
    {
        
        ClockCabinet.Clocks.Add(659876098698,[]);
        ClockCabinet.Clocks[659876098698].Add("hello world",new Clock("hello world",4));
        Assert.True(ClockCabinet.Clocks.Count != 0);
        Assert.True(ClockCabinet.Clocks[659876098698].Count != 0);
        ClockCabinet.Serialize();
    }

    [Fact]
    public void TestDeserialize()
    {
        ClockCabinet.Deserialize();
        Assert.True(ClockCabinet.Clocks.Count != 0);
        Assert.True(ClockCabinet.Clocks[659876098698].Count != 0);
    }
}