using System;
using System.Security.Permissions;
using System.Security.Policy;
using GenericTypeType.Tests.SubWrapper;
using Xunit.Abstractions;
using Xunit;

namespace GenericTypeType.Tests
{
  public class ResolveableType<T>
  {
  }

  public class DoubleResolveableType<T, TV>
  {
  }

  public class HardType
  {
    public class NestedHardType
    {
    }
  }

  namespace SubWrapper
  {
    public class SubResolveableType<T>
    {
    }

    public class SubDoubleResolveableType<T, TV>
    {
    }

    public class SubHardType
    {
    }
  }

  public class TypeResolverTests
  {
    private readonly ITestOutputHelper _testOutputHelper;

    private readonly TypeResolver _resolver;

    public TypeResolverTests(ITestOutputHelper testOutputHelper)
    {
      _testOutputHelper = testOutputHelper;
      _resolver = new TypeResolver
      {
        WellKnownTypes =
        {
          { "int", typeof(int) },
          { "float", typeof(float) }
        },
        WellKnownAssemblies =
        {
          new TypeResolver.WellKnownAssembly(typeof(ResolveableType<>).Assembly,
            "GenericTypeType.Tests.",
            "GenericTypeType.Tests.HardType+"),
          new TypeResolver.WellKnownAssembly(typeof(int).Assembly,
            "System.")
        },
        FallbackResolver = FallbackResolve
      };
    }

    private static Type? FallbackResolve(string typeName) => Type.GetType(typeName);

    [Theory]
    [InlineData("[System.Int32]", typeof(int))]
    [InlineData("int?", typeof(int?))]
    [InlineData(" int ? ", typeof(int?))]
    [InlineData("System.TimeSpan", typeof(TimeSpan))]
    [InlineData("TimeSpan", typeof(TimeSpan))]
    [InlineData("Single", typeof(float))]
    [InlineData("[GenericTypeType.Tests.DoubleResolveableType`2[System.Int32,System.Int32]]",
      typeof(DoubleResolveableType<int, int>))]
    [InlineData("[System.Nullable`1[[System.Int32]]]", typeof(int?))]
    [InlineData(
      "[System.Nullable`1[[System.Security.Permissions.FileDialogPermissionAccess]]]",
      typeof(FileDialogPermissionAccess?))]
    [InlineData(
      "[System.Security.Permissions.FileDialogPermissionAccess]",
      typeof(FileDialogPermissionAccess))]
    [InlineData("int", typeof(int))]
    [InlineData("HardType", typeof(HardType))]
    [InlineData("NestedHardType", typeof(HardType.NestedHardType))]
    [InlineData("ResolveableType<int>", typeof(ResolveableType<int>))]
    [InlineData(" ResolveableType<int>", typeof(ResolveableType<int>))]
    [InlineData("ResolveableType <int>", typeof(ResolveableType<int>))]
    [InlineData("ResolveableType< int>", typeof(ResolveableType<int>))]
    [InlineData("ResolveableType<int >", typeof(ResolveableType<int>))]
    [InlineData("DoubleResolveableType<HardType, System.Int32>", typeof(DoubleResolveableType<HardType, int>))]
    [InlineData(
      "DoubleResolveableType<int, [System.Security.Policy.Url]>",
      typeof(DoubleResolveableType<int, Url>))]
    [InlineData("SubWrapper.SubHardType", typeof(SubHardType))]
    [InlineData("GenericTypeType.Tests.SubWrapper.SubDoubleResolveableType<DoubleResolveableType<" +
                "[System.Security.Policy.Url]," +
                "[System.Nullable`1[[System.Int32]]]>," +
                "SubWrapper.SubResolveableType<ResolveableType<" +
                "[System.Security.Permissions.FileDialogPermissionAccess]?>>>",
      typeof(SubDoubleResolveableType<DoubleResolveableType<Url, int?>,
        SubResolveableType<ResolveableType<FileDialogPermissionAccess?>>>))]
    public void TestSuccessfulKnownResolves(string name, Type? type)
    {
      _testOutputHelper.WriteLine($"Resolving {name} to {type}");

      var result = _resolver.ParseType(name);
      Assert.Equal(type, result);
    }

    [Theory]
    [InlineData("bad")]
    [InlineData("[bad]")]
    [InlineData("[System.Nullable`1[[bad]]]")]
    [InlineData("IAssetProvider<float3>")]
    [InlineData("DoubleResolveableType<int>")] // generic count mismatch
    public void TestFailedResolution(string name)
    {
      var result = _resolver.ParseType(name);
      Assert.Null(result);
    }

    [Theory]
    [InlineData(typeof(int))]
    [InlineData(typeof(ResolveableType<int>))]
    [InlineData(typeof(DoubleResolveableType<int, int>))]
    [InlineData(typeof(SubHardType))]
    [InlineData(
      typeof(SubDoubleResolveableType<DoubleResolveableType<Url, int?>,
        SubResolveableType<ResolveableType<FileDialogPermissionAccess?>>>))]
    public void TestFullyQualifiedResolution(Type expected)
    {
      var path = expected.AssemblyQualifiedName!;
      var result = _resolver.ParseType($"[{path}]");

      Assert.Equal(expected, result);
    }
  }
}
