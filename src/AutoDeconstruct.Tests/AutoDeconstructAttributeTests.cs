using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class AutoDeconstructAttributeTests
{
	[Test]
	public static void Create()
	{
		var attribute = new AutoDeconstructAttribute();

		using (Assert.EnterMultipleScope())
		{
			Assert.That(attribute.Filtering, Is.EqualTo(Filtering.None));
			Assert.That(attribute.Properties, Is.Empty);
		}
	}

   [Test]
   public static void CreateWithNoFilteringAndPropertiesProvided() => 
		Assert.That(() => new AutoDeconstructAttribute(Filtering.None, ["prop"]),
		   Throws.TypeOf<ArgumentException>());

	[Test]
	public static void CreateWithFilteringAndNoPropertiesProvided() =>
		Assert.That(() => new AutoDeconstructAttribute(Filtering.Include, []),
			Throws.TypeOf<ArgumentException>());
}