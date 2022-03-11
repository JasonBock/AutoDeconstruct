using NUnit.Framework;

namespace AutoDeconstruct.IntegrationTests;

public static class InheritanceTests
{
	[Test]
	public static void RunDeconstruct()
	{
		var id = 3;
		var data = "data";

		var target = new DerivedInheritance
		{
			Id = id,
			Data = data
		};

		var (newData, newId) = target;

		Assert.Multiple(() =>
		{
			Assert.That(newData, Is.EqualTo(target.Data));
			Assert.That(newId, Is.EqualTo(target.Id));
		});
	}
}

public class BaseInheritance
{
	public int Id { get; set; }
}

public class DerivedInheritance
	: BaseInheritance
{
	public string? Data { get; set; }
}