using NUnit.Framework;

namespace AutoDeconstruct.IntegrationTests;

internal static class InheritanceTests
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

		using (Assert.EnterMultipleScope())
		{
			Assert.That(newData, Is.EqualTo(target.Data));
			Assert.That(newId, Is.EqualTo(target.Id));
		}
	}
}

[AutoDeconstruct]
internal class BaseInheritance
{
	internal int Id { get; set; }
}

[AutoDeconstruct]
internal sealed class DerivedInheritance
	: BaseInheritance
{
	internal string? Data { get; set; }
}