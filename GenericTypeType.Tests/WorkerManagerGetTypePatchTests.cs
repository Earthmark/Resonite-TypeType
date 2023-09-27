using FrooxEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elements.Core;
using ResoniteModLoader;
using Xunit;
using Xunit.Abstractions;

namespace GenericTypeType.Tests;

public class HarmonyRewriteSetup
{
  public HarmonyRewriteSetup()
  {
    WorkerManagerGetTypePatch.ForceEnable = true;
    GenericTypeTypeMod.HarmonyPatch();
  }
}

#pragma warning disable xUnit1033 // Test classes decorated with 'Xunit.IClassFixture<TFixture>' or 'Xunit.ICollectionFixture<TFixture>' should add a constructor argument of type TFixture
public class WorkerManagerGetTypePatchTests : IClassFixture<HarmonyRewriteSetup>
#pragma warning restore xUnit1033 // Test classes decorated with 'Xunit.IClassFixture<TFixture>' or 'Xunit.ICollectionFixture<TFixture>' should add a constructor argument of type TFixture
{
  private readonly ITestOutputHelper _testOutputHelper;

  public WorkerManagerGetTypePatchTests(HarmonyRewriteSetup _, ITestOutputHelper testOutputHelper)
  {
    _testOutputHelper = testOutputHelper;
  }

  [Theory]
  [InlineData("[System.Int32]", typeof(int))]
  [InlineData("int?", typeof(int?))]
  [InlineData("floatQ?", typeof(floatQ?))]
  [InlineData("IField<int?>", typeof(IField<int?>))]
  [InlineData("IField<floatQ?>", typeof(IField<floatQ?>))]
  [InlineData("Collections.Generic.KeyValuePair<float, float>", typeof(KeyValuePair<float, float>))]
  [InlineData("IField<[System.Int32]?>", typeof(IField<int?>))]
  [InlineData(" int ? ", typeof(int?))]
  [InlineData("System.TimeSpan", typeof(TimeSpan))]
  [InlineData("TimeSpan", typeof(TimeSpan))]
  [InlineData("Single", typeof(float))]
  public void Succeeds(string name, Type? type)
  {
    _testOutputHelper.WriteLine($"Resolving {name} to {type}");

    var result = WorkerManager.GetType(name);
    Assert.Equal(type, result);
  }

  [Theory]
  [InlineData("[System.Int3212e]")]
  [InlineData("IField<floatQ, float2>")]
  [InlineData("/")]
  [InlineData("\\")]
  [InlineData("IAssetProvider<float3>")] // generic constraint failure
  public void ResolveFails(string name)
  {
    Assert.Null(WorkerManager.GetType(name));
  }

  [Fact]
  public void ResolveManyTypes()
  {
    WorkerManager.GetType("");
    WorkerManager.GetType("\\");
    WorkerManager.GetType("\\\\");
    WorkerManager.GetType("\\");
    WorkerManager.GetType("");
  }
}
