# Introduction

These are my notes on what this package is all about.

## What is a Deconstructor?

Object deconstruction was added in C# 7.0. The documentation is [here](https://github.com/dotnet/roslyn/blob/main/docs/features/deconstruction.md), and there's another article [here](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct#user-defined-types). Basically, a deconstructor can be defined on either a type or as an extension method. In both cases, it has to be named "Deconstruct", it has to return `void`, and all of its parameters must be `out` parameters (the exception is with the extension method, where the first parameter is the object being extended). Furthermore, you can overload `Deconstruct` methods, but all `Deconstruct` methods must have a unique number of `out` parameters. Here's two examples:
```
public sealed class Customer
{
	public void Deconstruct(out Guid id, out string name)
	{
		id = this.Id;
		name = this.Name;
	}

	public Guid Id { get; }
	public string Name { get; }
}

var customer = new Customer(Guid.NewGuid(), "Jason");
var (id, name) = customer;

public struct Point
{
	public Point(int x, int y) =>
		(this.X, this.Y) = (x, y);
		
	public int X { get; }
	public int Y { get; }
}

public static class PointExtensions
{
	public static void Deconstruct(this Point self, out int x, out int y)
	{
		x = self.X;
		y = self.Y;
	}
}

var point = new Point(2, 3);
var (x, y) = point;
```

## AutoDeconstruct Rationale

AutoDeconstruct will look at type and method definitions, and look to see if the type has any `Deconstruct` methods, either as instance or extension methods. If none exist, then the library will look to see how many public, readable, instance properties exist (that are also (probably) not `ref` properties). If there's at least 1, the library generates a `Deconstruct` extension method in a static class defined in the same namespace as the target type. For example, if we have our `Point` type defined like this:
```
namespace Maths.Geometry;

public struct Point
{
	public Point(int x, int y) =>
		(this.X, this.Y) = (x, y);
		
	public int X { get; }
	public int Y { get; }
}
```
Then the library generates this:
```
namespace Maths.Geometry;

public static class PointExtensions
{
	public static void Deconstruct(this Point self, out int x, out int y)
	{
		x = self.X;
		y = self.Y;
	}
}
```
Now, the name of the `static` class may need to change to be something that is guaranteed to be unique. **TODO: Finish**

One other option is to create the extension method if a `Deconstruct` method doesn't exist with the number of properties found. For example, let's say we have this:
```
namespace Models;

public sealed class Person
{
	public void Deconstruct(out Guid id)
	{
		id = this.Id;
	}

	public uint Age { get; init; }
	public Guid Id { get; init; }
	public string Name { get; init; }
}

public static class PersonExtensions
{
	public static void Deconstruct(this Person self, out Guid id, out string name)
	{
		id = self.Id;
		name = self.Name;
	}
}
```
AutoDeconstruct would see that there are three properties that could be used for a generated `Deconstruct`. The two `Deconstruct` methods that exist have one and two `out` parameters, so it can generate one that has all three properties as `out` parameters:
```
namespace Models;

public static class PersonExtensions
{
	public static void Deconstruct(this Person self, out Guid id, out string name, out uint age)
	{
		id = self.Id;
		name = self.Name;
		age = self.Age;
	}
}
```
This also illustrates the issue that we have to generate a unique name for the extensions class that is guaranteed not to collide with any existing types. A simple way to do this is to generate a GUID, but this is also kind of ugly. Maybe what I can do is "reserve" a name, like `AutoDeconstructExtensions`, and create one for each needed namespace (or make them all `partial` so I don't have to think about combining them myself). Similar to what I did in PartiallyApplied, create a configuration file that would let the user define a different name for this class. I like that, I think that'll address any issues (i.e. the user could say the class name would be `MySuperNamedClassForGeneratedDeconstructMethods`).

So, what do I need to look for? I want to find types that do not have a `Deconstruct` for all their accessible property getters (excluding indexers). So, for the first pass, we're only going to look for either `TypeDeclarationSyntax` or `MethodDeclarationSyntax` nodes. If it's a `MethodDeclarationSyntax`, we only care if it's an extension method that has the name `Deconstruct`, and all of the parameters are `out` (except for the first one). The second pass will just return the node. The real work will happen in the source output method.

The first thing I'll do is find all the methods, and group them by their extension target type (i.e. the type of the first parameter). Then, for each `TypeDeclarationSyntax`, I'll get its `ITypeSymbol` (or `INamedTypeSymbol`), and do this:

* First, see if there are any accessible property getters.
* If there are, then see if there are any `Deconstruct` methods on the type or as an extension method that has the same number of parameters as found properties.
* If none exist that match the property count I just found, then I can generate a `Deconstruct`.

To generate the `Deconstruct`, I need to know the target type, and the list of accesible property getters. That should be sufficient.

The name of the file could be `AutoDeconstructExtensions_{TargetTypeNamespaceDotsToUnderscore}_{TargetTypeName}.g.cs`