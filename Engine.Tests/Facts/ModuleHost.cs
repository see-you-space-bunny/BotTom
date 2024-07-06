using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Engine.ModuleHost;

namespace Engine.Tests.Facts;

public class @ModuleHost
{
  private readonly ITestOutputHelper _output = output;

  [Fact]
  public void BasicPass()
  {
    Assert.True(true);
  }
}