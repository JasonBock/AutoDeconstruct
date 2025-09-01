using AutoDeconstruct;
using AutoDeconstruct.IntegrationTests;
using NUnit.Framework;

[assembly: TargetAutoDeconstruct(typeof(AssemblyLevelCustomer))]
[assembly: TargetAutoDeconstruct(typeof(IncludedAssemblyLevelCustomer), 
	Filtering.Include, [nameof(IncludedAssemblyLevelCustomer.Age), nameof(IncludedAssemblyLevelCustomer.Id)])]
[assembly: TargetAutoDeconstruct(typeof(ExcludedAssemblyLevelCustomer),
	Filtering.Exclude, [nameof(IncludedAssemblyLevelCustomer.Name)])]

namespace AutoDeconstruct.IntegrationTests;

internal static class TargetAutoDeconstructAttributeTests
{
	[Test]
	public static void RunDeconstruct()
	{
		var name = "Joe";
		var id = Guid.NewGuid();

		var target = new AssemblyLevelCustomer
		{
			Name = name,
			Id = id
		};

		var (newId, newName) = target;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newName, Is.EqualTo(name));
			Assert.That(newId, Is.EqualTo(id));
		}
	}

	[Test]
	public static void RunDeconstructWithIncludedFiltering()
	{
		var name = "Joe";
		var id = Guid.NewGuid();
		var age = 30u;

		var target = new IncludedAssemblyLevelCustomer
		{
			Name = name,
			Id = id,
			Age = age
		};

		var (newAge, newId) = target;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newAge, Is.EqualTo(age));
			Assert.That(newId, Is.EqualTo(id));
		}
	}

	[Test]
	public static void RunDeconstructWithExcludedFiltering()
	{
		var name = "Joe";
		var id = Guid.NewGuid();
		var age = 30u;

		var target = new ExcludedAssemblyLevelCustomer
		{
			Name = name,
			Id = id,
			Age = age
		};

		var (newAge, newId) = target;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newAge, Is.EqualTo(age));
			Assert.That(newId, Is.EqualTo(id));
		}
	}
}

internal sealed class AssemblyLevelCustomer
{
	public Guid Id { get; set; }
	public required string? Name { get; set; }
}

internal sealed class IncludedAssemblyLevelCustomer
{
	public uint Age { get; set; }
	public Guid Id { get; set; }
	public required string? Name { get; set; }
}

internal sealed class ExcludedAssemblyLevelCustomer
{
	public uint Age { get; set; }
	public Guid Id { get; set; }
	public required string? Name { get; set; }
}