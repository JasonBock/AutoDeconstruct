using NUnit.Framework;

namespace AutoDeconstruct.IntegrationTests;

internal static class AutoDeconstructAttributeTests
{
	[Test]
	public static void RunDeconstruct()
	{
		var name = "Joe";
		var id = Guid.NewGuid();

		var target = new Customer
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

		var target = new IncludedCustomer
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

		var target = new ExcludedCustomer
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

[AutoDeconstruct]
internal sealed class Customer
{
	public Guid Id { get; set; }
	public required string? Name { get; set; }
}

[AutoDeconstruct(Filtering.Include, [nameof(IncludedCustomer.Age), nameof(IncludedCustomer.Id)])]
internal sealed class IncludedCustomer
{
	public uint Age { get; set; }
	public Guid Id { get; set; }
	public required string? Name { get; set; }
}

[AutoDeconstruct(Filtering.Exclude, [nameof(ExcludedCustomer.Name)])]
internal sealed class ExcludedCustomer
{
	public uint Age { get; set; }
	public Guid Id { get; set; }
	public required string? Name { get; set; }
}