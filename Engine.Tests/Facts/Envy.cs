using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BotTom.Tests.Facts;

public class @Envy
{
  private readonly ITestOutputHelper _output = output;

  [Fact]
  public void BasicPass()
  {
    Assert.True(true);
  }
}