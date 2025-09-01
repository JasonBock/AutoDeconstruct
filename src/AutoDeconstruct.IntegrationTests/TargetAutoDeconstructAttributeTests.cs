using AutoDeconstruct;
using AutoDeconstruct.IntegrationTests;
using NUnit.Framework;

[assembly: TargetAutoDeconstruct(typeof(TargetCustomer))]
[assembly: TargetAutoDeconstruct(typeof(TargetIncludedCustomer), 
	Filtering.Include, [nameof(TargetIncludedCustomer.Age), nameof(TargetIncludedCustomer.Id)])]
[assembly: TargetAutoDeconstruct(typeof(TargetExcludedCustomer),
	Filtering.Exclude, [nameof(TargetExcludedCustomer.Name)])]

namespace AutoDeconstruct.IntegrationTests;

internal static class TargetAutoDeconstructAttributeTests
{
	[Test]
	public static void RunDeconstruct()
	{
		var name = "Joe";
		var id = Guid.NewGuid();

		var target = new TargetCustomer
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

		var target = new TargetIncludedCustomer
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

		var target = new TargetExcludedCustomer
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

internal sealed class TargetCustomer
{
	public Guid Id { get; set; }
	public required string? Name { get; set; }
}

internal sealed class TargetIncludedCustomer
{
	public uint Age { get; set; }
	public Guid Id { get; set; }
	public required string? Name { get; set; }
}

internal sealed class TargetExcludedCustomer
{
	public uint Age { get; set; }
	public Guid Id { get; set; }
	public required string? Name { get; set; }
}