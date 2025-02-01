# AutoDeconstruct

A library that automatically adds support for object deconstruction in C#.

## Getting Started

First, reference the `AutoDeconstruct` [NuGet package](https://www.nuget.org/packages/AutoDeconstruct).

Then, add `[AutoDeconstuct]` to a type so you can deconstruct it:

```c#
using AutoDeconstruct;

[AutoDeconstruct]
public sealed class Person
{
	public uint Age { get; set; }
	public required string Name { get; set; }
}

var person = new Person { Age = 22, Name = "Joe" };
var (age, name) = person;
```

Read [the overview document](docs/Overview.md) for further details.

### Prerequisites

The Rocks package targets .NET Standard 2.0 for host flexibility.

## Overview

The idea started with [this tweet](https://twitter.com/buhakmeh/status/1462106117564207104) - specifically, [this reply](https://twitter.com/dave_peixoto/status/1462181358248374278). I thought...how automatic can I make object deconstruction in C#? That's what this source generator is all about.

Read [the overview document](docs/Overview.md) for further details.