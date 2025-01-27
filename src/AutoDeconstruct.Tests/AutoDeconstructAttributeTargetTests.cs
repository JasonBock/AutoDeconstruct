using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class AutoDeconstructAttributeTargetTests
{
	[Test]
	public static async Task GenerateAtAssemblyLevelAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: AutoDeconstruct]
			
			namespace TestSpace
			{
				public class Test
				{ 
					public string? Namespace { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @namespace)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@namespace = @self.Namespace;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateAtTypeLevelAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public class Test
				{ 
					public string? Namespace { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @namespace)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@namespace = @self.Namespace;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}
}