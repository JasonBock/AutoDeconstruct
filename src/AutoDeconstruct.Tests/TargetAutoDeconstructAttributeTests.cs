﻿using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class TargetAutoDeconstructAttributeTests
{
	[Test]
	public static void Create()
	{
		var targetType = typeof(NotGenericType);
		var attribute = new TargetAutoDeconstructAttribute(targetType);

		using (Assert.EnterMultipleScope())
		{
			Assert.That(attribute.TargetType, Is.EqualTo(targetType));
			Assert.That(attribute.Search, Is.EqualTo(SearchForExtensionMethods.No));
		}
	}

   [Test]
   public static void CreateWhenTargetTypeIsNull() => 
		Assert.That(() => new TargetAutoDeconstructAttribute(null!), 
			Throws.TypeOf<ArgumentNullException>());

	[Test]
	public static void CreateWhenTargetTypeIsClosedGeneric() =>
		Assert.That(() => new TargetAutoDeconstructAttribute(typeof(GenericType<NotGenericType>)), 
			Throws.TypeOf<ArgumentException>());
}

internal sealed class NotGenericType { }

internal sealed class GenericType<T> { }