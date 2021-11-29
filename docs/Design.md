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