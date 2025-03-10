# Table of Contents
- [Motivation](#motivation)
- [What is a Deconstructor?](#what-is-a-deconstructor)
- [AutoDeconstruct Features](#autodeconstruct-features)
	- [Marking Types](#marking-types)
	- [Assembly-Level Support](#assembly-level-support)
	- [Ignore Deconstruction Creation](#ignore-deconstruction-creation)

## Motivation

The idea started with [this tweet](https://twitter.com/buhakmeh/status/1462106117564207104) - specifically, [this reply](https://twitter.com/dave_peixoto/status/1462181358248374278). I thought...how automatic can I make object deconstruction in C#? That's what this source generator is all about.

## What is a Deconstructor?

Object deconstruction was added in C# 7.0. The documentation is [here](https://github.com/dotnet/roslyn/blob/main/docs/features/deconstruction.md), and there's another article [here](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/functional/deconstruct#user-defined-types). Basically, a deconstructor can be defined on either a type or as an extension method. In both cases, it has to be named "Deconstruct", it has to return `void`, and all of its parameters must be `out` parameters (the exception is with the extension method, where the first parameter is the type being extended). Furthermore, `Deconstruct()` methods can overloaded, but all `Deconstruct()` methods must have a unique number of `out` parameters. Here are two examples:

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

Note that what values are deconstructed is up to the developer. That is, deconstruction does not require one to deconstruct all property or field values.

## AutoDeconstruct Features

### Marking Types
AutoDeconstruct looks to see if the target type has any `Deconstruct()` methods, either as instance or extension methods (if extension methods are searched for - this is discussed later in this document). If none exist, then AutoDeconstruct looks to see how many accessible, readable, instance properties exist. If there's at least 1, the library generates a `Deconstruct()` extension method in a static class defined in the same namespace as the target type. For example, if we have our `Point` type defined like this:

```csharp
namespace Maths.Geometry;

[AutoDeconstruct]
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

If the target type is a reference type, a null check will be generated. Furthermore, the `Deconstruct()` extension method will also be created if a `Deconstruct()` doesn't exist with the number of properties found. For example, let's say we have this:

```csharp
using AutoDeconstruct;

namespace Models;

[AutoDeconstruct]
public sealed class Person
{
	public uint Age { get; init; }
	public Guid Id { get; init; }
	public string Name { get; init; }

	public void Deconstruct(out Guid id) =>
		id = this.Id;
}
```

AutoDeconstruct would see that there are three properties that could be used for a generated `Deconstruct()`. The `Deconstruct()` method that exists has one `out` parameter, so it will generate one that has all three properties as `out` parameters:

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

### Assembly-Level Support

`[AutoDeconstruct]` can also be defined at the assembly level to inform AutoDeconstruct to add `Deconstruct()` extension methods for **every** type in the assembly:

```csharp
using AutoDeconstruct;

[assembly: AutoDeconstruct]

namespace Models;

public sealed class Person
{
	public uint Age { get; init; }
	public Guid Id { get; init; }
	public string Name { get; init; }

	public void Deconstruct(out Guid id) =>
		id = this.Id;
}
```

While AutoDeconstruct will search the target type to see if an existing `Deconstruct()` method exists that matches what AutoDeconstruct would do, a user may want to opt int to a search to look through the entire assembly for `Deconstruct()` extension methods. By default, this is turned off as it's not common to create deconstruction methods this way, but if this completeness is desired, `SearchForExtensionMethods.Yes` can be passed into `[AutoDeconstruct]`:

```csharp
using AutoDeconstruct;

[assembly: AutoDeconstruct(SearchForExtensionMethods.Yes)]

namespace Models;

public sealed class Person
{
	public uint Age { get; init; }
	public Guid Id { get; init; }
	public string Name { get; init; }

	public void Deconstruct(out Guid id) =>
		id = this.Id;
}

public static partial class PersonExtensions
{
	public static void Deconstruct(this Person @self, out Guid @id, out string @name, out uint @age) =>
		(@id, @name, @age) = (@self.Id, @self.Name, @self.Age);
}
```

In this case, AutoDeconstruct will detect an existing `Deconstruct()` method that already does what AutoDeconstruct would generate, so no code is generated. This search flag also works when `[AutoDeconstruct]` is defined on a specific type.

### Ignore Deconstruction Creation

The `[NoAutoDeconstruct]` attribute can be added to a type will tell AutoDeconstrct to ignore it. Note that this is only relevant when `[AutoDeconstruct]` is added at the assembly level:

```csharp
namespace AutoDeconstruct;
namespace Models;

[assembly: AutoDeconstruct]

[NoAutoDeconstruct]
public sealed class Person
{
	public uint Age { get; init; }
	public Guid Id { get; init; }
	public string Name { get; init; }
}

public struct Point
{
	public Point(int x, int y) =>
		(this.X, this.Y) = (x, y);
		
	public int X { get; }
	public int Y { get; }
}
```

In this case, AutoDeconstruct will not generate a `Deconstruct` method for `Person`, but it will for `Point`. If a type has `[AutoDeconstruct]` and `[NoAutoDeconstruct]`, `[AutoDeconstruct]` "wins".