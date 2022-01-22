using NUnit.Framework;

namespace AutoDeconstruct.IntegrationTests;

public static class NoDeconstructExistsTests
{
	[Test]
	public static void RunDeconstruct()
	{
		var id = Guid.NewGuid();
		var value = 3;

		var target = new NoDeconstructExists { Id = id, Value = value };
		var (newId, newValue) = target;

		Assert.Multiple(() =>
		{
			Assert.That(newId, Is.EqualTo(id));
			Assert.That(newValue, Is.EqualTo(value));
		});
	}
}

public sealed class NoDeconstructExists
{
	public Guid Id { get; set; }
	public int Value { get; set; }
}