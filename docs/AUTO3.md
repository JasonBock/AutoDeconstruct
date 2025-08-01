# An existing extension Deconstruct() method already exists for the target type

## Issue

This analyzer is tripped if you try to use either `[AutoDeconstruct]` or `[TargetAutoDeconstruct]` when a `Deconstruct()` method already exists:

```csharp
[AutoDeconstruct(SearchForExtensionMethods.Yes)]
public sealed class Thing 
{ 
  public string? Name { get; set; }
  public Guid Id { get; set; }
}

public static class ThingExtensions
{
  public static void Deconstruct(this Thing self, out string? name, out Guid id) =>
    (name, id) = (self.Name, self.Id);
}
```

Note that `SearchForExtensionMethods.Yes` must be used for this analyzer to search for extension methods.

## Code Fix

No code fix is available. Simply remove the attribute in error.