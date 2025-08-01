# An existing Deconstruct() method already exists on the target type

## Issue

This analyzer is tripped if you try to use either `[AutoDeconstruct]` or `[TargetAutoDeconstruct]` when a `Deconstruct()` method already exists:

```csharp
[AutoDeconstruct]
public sealed class Thing 
{ 
  public string? Name { get; set; }    
  public Guid Id { get; set; }

  public void Deconstruct(out string? name, out Guid id) =>
    (name, id) = (this.Name, this.Id);
}
```

## Code Fix

No code fix is available. Simply remove the attribute in error.