using NUnit.Framework;

namespace AutoDeconstruct.IntegrationTests;

public static class GenericTests
{
	[Test]
	public static void DeconstructWithGenerics()
	{
		var thing = new GenericThings<int, string> { Thing1 = 1, Thing2 = "2" };
		(var newThing1, var newThing2) = thing;

		Assert.Multiple(() =>
		{
			Assert.That(newThing1, Is.EqualTo(1));
			Assert.That(newThing2, Is.EqualTo("2"));
		});
	}
}

public class GenericThings<T1, T2>
{
	public required T1 Thing1 { get; set; }
	public required T2 Thing2 { get; set; }
}