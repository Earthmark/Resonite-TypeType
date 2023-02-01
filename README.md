# GenericTypeType

A neos mod that adds additional resolution steps when adding a generic in the `AttachComponent` dialog. The origional neos resolution pattern is intact with this mod, this only comes into play if the built in type resolver failed to find a candidate type.

The additional resolution steps for a type name are as follows:

1. If a name starts and ends with `[` and `]`, the Neos default resolver is used.
2. The name is compared against well known types.<br>
  _ex: `float`, `sbyte`, `int`_<br>
3. If the name ends with `?`, the inner type is resolved using these steps recusively. If the resolved type is a struct, that type wrapped in `System.Nullable<T>` is returned, else no type is resolved.
4. The name is checked against well known assemblies:<br>
  4.1. The name is a strongly named type in the assembly, without needing the assembly name itself (this solves needing BaseX everywhere)<br>
    _ex: `BaseX.float4`, `FrooxEngine.ITextField`, `System.String`_<br>
  4.2. The name is prepended with a list of well known namespaces in each assembly.<br>
    _ex: names will be prepended with `BaseX` as part of checking the `BaseX` assembly, `FrooxEngine.` and `FrooxEngine.LogiX.` when checking the `FrooxEngine` assembly. This strongly resolves types like `floatQ` as `BaseX.floatQ`, or logix nodes._
5. If the name itself contains generic arguments wrapped with `<` and `>`, all steps are repeated recursively.<br>
  _ex: `IField<IField<floatQ>` resolves to `FrooxEngine.IField<FrooxEngine.IField<BaseX.floatQ>`._
