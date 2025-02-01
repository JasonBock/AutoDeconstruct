using NUnit.Framework;

namespace AutoDeconstruct.IntegrationTests;

internal static class GenericTests
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

[AutoDeconstruct]
internal sealed class GenericThings<T1, T2>
{
	internal required T1 Thing1 { get; set; }
	internal required T2 Thing2 { get; set; }
}