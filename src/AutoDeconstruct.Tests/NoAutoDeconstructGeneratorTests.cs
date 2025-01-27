using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class NoAutoDeconstructGeneratorTests
{
	[Test]
	public static async Task GenerateWhenAttributeExistsOnClassAsync()
	{
		var code =
			"""
			using AutoDeconstruct;

			namespace TestSpace
			{
				[NoAutoDeconstruct]
				public class Test
				{
					public string Id { get; set; }
				}
			}
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[],
			[]);
	}

	[Test]
	public static async Task GenerateWhenAttributeExistsOnStructAsync()
	{
		var code =
			"""
			using AutoDeconstruct;

			namespace TestSpace
			{
				[NoAutoDeconstruct]
				public struct Test
				{
					public string Id { get; set; }
				}
			}
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[],
			[]);
	}

	[Test]
	public static async Task GenerateWhenAttributeExistsOnInterfaceAsync()
	{
		var code =
			"""
			using AutoDeconstruct;

			namespace TestSpace
			{
				[NoAutoDeconstruct]
				public interface ITest
				{
					string Id { get; set; }
				}
			}
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[],
			[]);
	}
}