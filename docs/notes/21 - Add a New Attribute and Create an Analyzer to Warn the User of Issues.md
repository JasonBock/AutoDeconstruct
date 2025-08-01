To create the analyzer, update `TypeSymbolMode.GetModel()` to return a tuple: `(TypeSymbolModel?, SomethingThatExplainsWhyTSMIsNull)`. That way, the analyzer could use it for both attributes, and determine if the target type either has no accessible properties, or already has a `Deconstruct()` method.

I will also need to refactor `new IsTypeExcludedVisitor(compilation.Assembly, targetType, token)` from the generator and put it into `GetModel()` so I can report if an extension `Deconstruct()` already exists.

TODO:
* DONE - Add a `VerifySupportedDiagnostics()` test
* DONE - Finish out analzyer tests
* DONE - Add analyzer ID docs