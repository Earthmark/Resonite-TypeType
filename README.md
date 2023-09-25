# GenericTypeType

**Note:** _Since an official mod loader is not in place, this one will link to the mod loader created by Rucio and Stiefel. Once an official one comes out (perhaps a collab between all forks), then we will replace the URL below._

A [ResoniteModLoader](https://github.com/bontebok/ResoniteModLoader) mod for [Resonite](https://resonite.com/) that adds additional resolution steps when adding a generic in the `AttachComponent` dialog. The original Resonite resolution pattern is still intact with this mod as this only comes into play if the built-in type resolver failed to find a candidate type.

The additional resolution steps for a type name are as follows:

1. If a name starts and ends with `[` and `]`, the Resonite default resolver is used.
2. The name is compared against well known types.<br>
  _ex: `float`, `sbyte`, `int`_<br>
3. If the name ends with `?`, the inner type is resolved using these steps recusively. If the resolved type is a struct, that type wrapped in `System.Nullable<T>` is returned, else no type is resolved.
4. The name is checked against well known assemblies:<br>
  4.1. The name is a strongly named type in the assembly, without needing the assembly name itself (this solves needing Element.Core everywhere)<br>
    _ex: `Element.Core.float4`, `FrooxEngine.ITextField`, `System.String`_<br>
  4.2. The name is prepended with a list of well known namespaces in each assembly.<br>
    _ex: names will be prepended with `Element.Core` as part of checking the `Element.Core` assembly, `FrooxEngine.` and `FrooxEngine.ProtoFlux.` when checking the `FrooxEngine` assembly. This strongly resolves types like `floatQ` as `Element.Core.floatQ`, or some core flux nodes._
5. If the name itself contains generic arguments wrapped with `<` and `>`, all steps are repeated recursively.<br>
  _ex: `IField<IField<floatQ>` resolves to `FrooxEngine.IField<FrooxEngine.IField<Element.Core.floatQ>`._

## Installation
1. Install [ResoniteModLoader](https://github.com/bontebok/ResoniteModLoader).
1. Place GenericTypeType.dll into your `rml_mods` folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\Resonite\rml_mods` for a default install. You can create it if it's missing, or if you launch the game once with ResoniteModLoader installed it will create the folder for you.
1. Start the game. If you want to verify that the mod is working you can check your Resonite logs.