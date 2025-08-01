# No accessible properties were found

## Issue

This analyzer is tripped if you try to use either `[AutoDeconstruct]` or `[TargetAutoDeconstruct]` when there are no accessible properties:

```csharp
[AutoDeconstruct]
public sealed class Thing { }
```

An accessible property in AutoDeconstruct is one that is public, is not static, is not an indexer, and has a getter that is accessible.

## Code Fix

No code fix is available. Simply remove the attribute in error.