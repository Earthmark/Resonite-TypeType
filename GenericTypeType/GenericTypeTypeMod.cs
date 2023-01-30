using System.Runtime.CompilerServices;
using HarmonyLib;
using NeosModLoader;

namespace GenericTypeType;

public class GenericTypeTypeMod : NeosMod
{
  public static ModConfiguration? Config;

  [AutoRegisterConfigKey]
  public static ModConfigurationKey<bool> IsEnabled = new("is_enabled", "A toggle for the user, if the mod should run", () => true);

  public override string Name => nameof(GenericTypeType);
  public override string Author => "Earthmark";
  public override string Version => "0.0.1";

  public override void OnEngineInit()
  {
    Config = GetConfiguration();
    HarmonyPatch();
  }

  [MethodImpl(MethodImplOptions.NoInlining)]
  public static void HarmonyPatch()
  {
    var harmony = new Harmony("net.earthmark.GenericTypeType");
    harmony.PatchAll();
  }
}