* Update docs
* I added `internal` properties to the list, as it's possible someone may want to deconstruct internally. But, now I need to make the static class and the extension method `internal` if either the target type is `internal`, or any of the properties are `internal`.
* Finding matching `Deconstruct()` extension methods isn't correct. We need to also look at the parameter types.
* Create an integration test project for the assembly-level `[AutoDeconstruct]` version with `[NoAutoDeconstruct]`
* Consider making the generated extension method part of the type declaration if the type is `partial`
* In `CompilationTypesCollector`, does `GetMembers()` also get nested types?
* Add a flag to `[AutoDeconstruct]` to only do a full assembly search of extension method, something like `[AutoDeconstruct(SearchForExtensionMethods.Yes)]`. It would be `SearchForExtensionMethods.No` by default, you need to opt-in to do the search. Most of the time, the search is unnecessary.
* Make an analyzer to yell at the developer if `[AutoDeconstruct]` and `[NoAutoDeconstruct]` exist on the same type.
