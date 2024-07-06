using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace BotTom.Tests.Facts;

public class DiscordNET(ITestOutputHelper output)
{
  private readonly ITestOutputHelper _output = output;

  [Fact]
  public void BasicPass()
  {
    Assert.True(true);
  }
}