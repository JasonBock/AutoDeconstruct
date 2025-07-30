using NUnit.Framework;

namespace AutoDeconstruct.Tests;

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
}