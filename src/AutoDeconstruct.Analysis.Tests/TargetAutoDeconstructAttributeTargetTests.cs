using NUnit.Framework;

namespace AutoDeconstruct.Analysis.Tests;

internal static class TargetAutoDeconstructAttributeTargetTests
{
	[Test]
	public static async Task GenerateAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: TargetAutoDeconstruct(typeof(TestSpace.Test))]
			
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
				public static class TestExtensions
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
			[(typeof(AutoDeconstructGenerator), "TestSpace.Test_TargetAutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenIncludeFilteringExistsAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: TargetAutoDeconstruct(typeof(TestSpace.Test), Filtering.Include, [nameof(TestSpace.Test.Id)])]
			
			namespace TestSpace
			{
				public class Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
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
					public static void Deconstruct(this global::TestSpace.Test @self, out global::System.Guid @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "TestSpace.Test_TargetAutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenExcludeFilteringExistsAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: TargetAutoDeconstruct(typeof(TestSpace.Test), Filtering.Exclude, [nameof(TestSpace.Test.Name)])]
			
			namespace TestSpace
			{
				public class Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
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
					public static void Deconstruct(this global::TestSpace.Test @self, out global::System.Guid @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "TestSpace.Test_TargetAutoDeconstruct.g.cs", generatedCode)],
			[]);
	}
}