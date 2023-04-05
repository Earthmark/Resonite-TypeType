﻿using FrooxEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseX;
using NeosModLoader;
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

public class WorkerManagerGetTypePatchTests : IClassFixture<HarmonyRewriteSetup>
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
  [InlineData("IAssetProvider<float3>")] // generic constraint failure
  public void ResolveFails(string name)
  {
    Assert.Null(WorkerManager.GetType(name));
  }
}
