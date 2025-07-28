using AutoDeconstruct;
using AutoDeconstruct.IntegrationTests;
using NUnit.Framework;

[assembly: TargetAutoDeconstruct(typeof(AssemblyLevelCustomer))]

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
}

internal sealed class AssemblyLevelCustomer
{
	public required string? Name { get; set; }
	public Guid Id { get; set; }
}