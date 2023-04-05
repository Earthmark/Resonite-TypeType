using FrooxEngine;
using HarmonyLib;
using System;
using System.Reflection;
using MonoMod.Utils;

namespace GenericTypeType;

[HarmonyPatch(typeof(WorkerManager), nameof(WorkerManager.GetType))]
public class WorkerManagerGetTypePatch
{
  public static bool ForceEnable { get; set; }

  private static readonly TypeResolver Resolver = new()
  {
    WellKnownTypes =
    {
      { "byte", typeof(byte) },
      { "sbyte", typeof(sbyte) },
      { "char", typeof(char) },
      { "short", typeof(short) },
      { "ushort", typeof(ushort) },
      { "int", typeof(int) },
      { "uint", typeof(uint) },
      { "long", typeof(long) },
      { "ulong", typeof(ulong) },
      { "float", typeof(float) },
      { "double", typeof(double) },
      { "string", typeof(string) },
    },
    WellKnownAssemblies =
    {
      new TypeResolver.WellKnownAssembly(typeof(WorkerManager).Assembly,
        "FrooxEngine.",
        "FrooxEngine.LogiX.",
        "FrooxEngine.UIX."),
      new TypeResolver.WellKnownAssembly(typeof(BaseX.float2).Assembly,
        "BaseX."),
      new TypeResolver.WellKnownAssembly(typeof(int).Assembly,
        "System.")
    },
  };

  private static void Prepare(MethodBase? original)
  {
    if (original != null)
    {
      Resolver.FallbackResolver = original.CreateDelegate<Func<string, Type?>>();
    }
  }

  private static void Prefix(out string __state, string typename)
  {
    __state = typename;
  }

  //private static void Postfix(ref Type? __result, string __state)
  //{
  //  if (__result != null)
  //  {
  //    return;
  //  }

  //  if (!ForceEnable &&
  //      (GenericTypeTypeMod.Config?.TryGetValue(GenericTypeTypeMod.IsEnabled, out var enabled) is null or false ||
  //       !enabled))
  //  {
  //    return;
  //  }

  //  __result = Resolver.ParseType(__state);
  //}

  private static Exception? Finalizer(Exception __exception, ref Type? __result, string __state)
  {
    if (__result != null)
    {
      return null;
    }

    if (!ForceEnable &&
        (GenericTypeTypeMod.Config?.TryGetValue(GenericTypeTypeMod.IsEnabled, out var enabled) is null or false ||
         !enabled))
    {
      return null;
    }

    __result = Resolver.ParseType(__state);
    return null;
  }
}
