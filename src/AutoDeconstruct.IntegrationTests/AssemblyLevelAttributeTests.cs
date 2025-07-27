using AutoDeconstruct;
using AutoDeconstruct.IntegrationTests;
using NUnit.Framework;

[assembly: AutoDeconstruct(targetType: typeof(Customer))]

namespace AutoDeconstruct.IntegrationTests;

internal static class AssemblyLevelAttributeTests
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

		var (newName, newId) = target;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newName, Is.EqualTo(name));
			Assert.That(newId, Is.EqualTo(id));
		}
	}
}

internal sealed class Customer
{
	public required string? Name { get; set; }
	public Guid Id { get; set; }
}