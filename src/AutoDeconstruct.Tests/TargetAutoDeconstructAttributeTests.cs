using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class TargetAutoDeconstructAttributeTests
{
	[Test]
	public static void Create()
	{
		var targetType = typeof(NotGenericType);
		var attribute = new TargetAutoDeconstructAttribute(targetType);

		Assert.That(attribute.TargetType, Is.EqualTo(targetType));
	}

	[Test]
   public static void CreateWhenTargetTypeIsNull() => 
		Assert.That(() => new TargetAutoDeconstructAttribute(null!), 
			Throws.TypeOf<ArgumentNullException>());

	[Test]
	public static void CreateWhenTargetTypeIsClosedGeneric() =>
		Assert.That(() => new TargetAutoDeconstructAttribute(typeof(GenericType<NotGenericType>)), 
			Throws.TypeOf<ArgumentException>());

	[Test]
	public static void CreateWithFilteringAndPropertiesProvided()
	{
		var targetType = typeof(NotGenericType);
		const Filtering filtering = Filtering.Include;
		var properties = new string[] { "prop" };

		var attribute = new TargetAutoDeconstructAttribute(targetType, filtering, properties);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(attribute.TargetType, Is.EqualTo(targetType));
			Assert.That(attribute.Filtering, Is.EqualTo(filtering));
			Assert.That(attribute.Properties, Is.EquivalentTo(properties));
		}
	}

	[Test]
	public static void CreateWithNoFilteringAndPropertiesProvided() =>
		Assert.That(() => new TargetAutoDeconstructAttribute(typeof(NotGenericType), Filtering.None, ["prop"]),
			Throws.TypeOf<ArgumentException>());

	[Test]
	public static void CreateWithFilteringAndNoPropertiesProvided() =>
		Assert.That(() => new TargetAutoDeconstructAttribute(typeof(NotGenericType), Filtering.Include, []),
			Throws.TypeOf<ArgumentException>());
}

internal sealed class NotGenericType { }

internal sealed class GenericType<T> { }