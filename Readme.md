# Forwarder

A common problem with the Composition pattern (over Inheritance) is that you cannot directly access encapsulated member APIs of composed members from the composite / container class / struct. Typically, this involves manually duplicating any or all APIs you want in the container to forward from the composite to the composed member.

This package aims to make that process simpler: Easily forward to member APIs without needing to duplicate them manually by using the `[Forward]` attribute instead.

Ex: 
```csharp
public class Composed
{
    private int _integer;
    public int GetStored() => _integer;
    public void Store(int value) => _integer = value;
}
```

Previous:
```csharp
public class Composite
{
    private readonly Composed c = new();
    public int GetStored() => c.GetStored();
    public void Store(int value) => c.Store(value);
}
```

To:
```csharp
public partial class Composite
{
    [Forward] 
    private readonly Composed c = new();
}
```

This works using Roslyn Source Generators. Current requirements: C# version ? / .NET version ?

## Usage
1. Composites must be declared `partial` to enable adding APIs to it.
2. `[Forward]` can only be declared on fields.
3. The field must declare a single member only (ex: `[Forward] MyClass a, b;` is not permitted).
4. (Intended, currently broken) If the Composite already has an API with an identical signature to a forwarded composed member API, that API is not forwarded.
5. Nested `[Forward]`s are allowed (Ex: A -> B -> C), and circular forward references are handled safely.

## Done

* Enhancement: allow forwarding of `internal` methods.

## Todo

* Analyzer: error when using the `[Forward]` attribute on a field with multiple declarations
* Bugfix + Analyzer: error when using the `[Forward]` attribute on multiple composed members of the same type (on separate lines)
* Enhancement: augment `[Forward]` to allow selective forwarding of specific APIs.
* Bugfix: skip composed APIs that already exist in the composite.
* Bugfix: make sure all parameter modifiers work as expected
