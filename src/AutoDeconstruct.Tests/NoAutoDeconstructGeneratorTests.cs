using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class NoAutoDeconstructGeneratorTests
{
	[Test]
	public static async Task GenerateWhenBothAttributeExistsOnClassAsync()
	{
		var code =
			"""
			using AutoDeconstruct;

			namespace TestSpace
			{
				[AutoDeconstruct]
				[NoAutoDeconstruct]
				public class Test
				{
					public string Id { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenAttributeExistsOnClassAsync()
	{
		var code =
			"""
			using AutoDeconstruct;

			[assembly: AutoDeconstruct]

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

			[assembly: AutoDeconstruct]
			
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

			[assembly: AutoDeconstruct]
			
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