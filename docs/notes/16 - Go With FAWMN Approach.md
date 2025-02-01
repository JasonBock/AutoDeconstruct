* Update docs
* DONE - I added `internal` properties to the list, as it's possible someone may want to deconstruct internally. But, now I need to make the static class and the extension method `internal` if either the target type is `internal`, or any of the properties are `internal`.
* DONE - Finding matching `Deconstruct()` extension methods isn't correct. We need to also look at the parameter types.
* DONE - I think I overdid it. If the `Deconstruct()` has the same number of property as parameters, then it has to be excluded, because deconstruct methods can't be overloaded with the same number of parmaters.
* DONE - In `CompilationTypesCollector`, does `GetMembers()` also get nested types?
* DONE - Optimize calling `ToCamelCase()`
* DONE - In `StringExtensions.ToCamelCase()`, use a source generator for the regex...maybe?
* Make an analyzer to yell at the developer if `[AutoDeconstruct]` and `[NoAutoDeconstruct]` exist on the same type.
* Consider making the generated extension method part of the type declaration if the type is `partial`. See this for details: https://stackoverflow.com/questions/68906372/roslyn-analyzer-is-class-marked-as-partial
* Add a flag to `[AutoDeconstruct]` to only do a full assembly search of extension method, something like `[AutoDeconstruct(SearchForExtensionMethods.Yes)]`. It would be `SearchForExtensionMethods.No` by default, you need to opt-in to do the search. Most of the time, the search is unnecessary.
* Create an integration test project for the assembly-level `[AutoDeconstruct]` version with `[NoAutoDeconstruct]`


Also...https://discord.com/channels/143867839282020352/598678594750775301/1068691525615239168