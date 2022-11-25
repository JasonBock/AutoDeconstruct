# Table of Contents
- [What is a Deconstructor?](#what-is-a-deconstructor?)
- [AutoDeconstruct Features](#autodeconstruct-rationale)

## What is a Deconstructor?

Object deconstruction was added in C# 7.0. The documentation is [here](https://github.com/dotnet/roslyn/blob/main/docs/features/deconstruction.md), and there's another article [here](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct#user-defined-types). Basically, a deconstructor can be defined on either a type or as an extension method. In both cases, it has to be named "Deconstruct", it has to return `void`, and all of its parameters must be `out` parameters (the exception is with the extension method, where the first parameter is the object being extended). Furthermore, you can overload `Deconstruct` methods, but all `Deconstruct` methods must have a unique number of `out` parameters. Here are two examples:

```csharp
using System;

public sealed class Customer
{
	public Customer(Guid id, string name) =>
		(this.Id, this.Name) = (id, name);

	public void Deconstruct(out Guid id, out string name) =>
		(id, name) = (this.Id, this.Name);

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
	public static void Deconstruct(this Point self, out int x, out int y) =>
		(x, y) = (self.X, self.Y);
}

var point = new Point(2, 3);
var (x, y) = point;
```

## AutoDeconstruct Features

AutoDeconstruct finds all type and method definitions, and it'll look to see if the type has any `Deconstruct` methods, either as instance or extension methods. If none exist, then AutoDeconstruct looks to see how many public, readable, instance properties exist. If there's at least 1, the library generates a `Deconstruct` extension method in a static class defined in the same namespace as the target type. For example, if we have our `Point` type defined like this:

```csharp
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

```csharp
#nullable enable

namespace Maths.Geometry
{
	public static partial class PointExtensions
	{
		public static void Deconstruct(this global::Maths.Geometry.Point @self, out int @x, out int @y) =>
			(@x, @y) = (@self.X, @self.Y);
	}
}
```

If the target type is a reference type, a null check will be generated. Furthermore, the `Deconstruct` extension method will also be created if a `Deconstruct` doesn't exist with the number of properties found. For example, let's say we have this:

```csharp
namespace Models;

public sealed class Person
{
	public void Deconstruct(out Guid id) =>
		id = this.Id;

	public uint Age { get; init; }
	public Guid Id { get; init; }
	public string Name { get; init; }
}

public static class PersonExtensions
{
	public static void Deconstruct(this Person self, out Guid id, out string name) =>
		(id, name) = (self.Id, self.Name);
}
```

AutoDeconstruct would see that there are three properties that could be used for a generated `Deconstruct`. The two `Deconstruct` methods that exist have one and two `out` parameters, so it will generate one that has all three properties as `out` parameters:

```csharp
#nullable enable

namespace Models
{
	public static partial class PersonExtensions
	{
		public static void Deconstruct(this global::Models.Person @self, out global::System.Guid @id, out string @name, out uint @age)
		{
			global::System.ArgumentNullException.ThrowIfNull(@self);
			(@id, @name, @age) = (@self.Id, @self.Name, @self.Age);
		}
	}
}
```

While AutoDeconstruct will do a complete search for types to generate `Deconstruct` methods, a user may want to opt out of this search for specific types. Adding the `[NoAutoDeconstruct]` attribute to a type will tell AutoDeconstrct to ignore it:

```csharp
namespace AutoDeconstruct;
namespace Models;

[NoAutoDeconstruct]
public sealed class Person
{
	public uint Age { get; init; }
	public Guid Id { get; init; }
	public string Name { get; init; }
}
```

In this case, AutoDeconstruct will not generate a `Deconstruct` method for `Person`.