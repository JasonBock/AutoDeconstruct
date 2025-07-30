using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class AttributeTargetTests
{
	[Test]
	public static async Task GenerateWhenDuplicationExistsAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: TargetAutoDeconstruct(typeof(TestSpace.Test))]
			
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

		var typeDefinitionDiagnostic = new DiagnosticResult("CS0101", DiagnosticSeverity.Error)
			.WithSpan(@"AutoDeconstruct\AutoDeconstruct.AutoDeconstructGenerator\TestSpace.Test_TargetAutoDeconstruct.g.cs", 5, 22, 5, 36)
			.WithArguments("TestExtensions", "TestSpace");
		var memberDefinitionDiagnostic = new DiagnosticResult("CS0111", DiagnosticSeverity.Error)
			.WithSpan(@"AutoDeconstruct\AutoDeconstruct.AutoDeconstructGenerator\TestSpace.Test_TargetAutoDeconstruct.g.cs", 7, 22, 7, 33)
			.WithArguments("Deconstruct", "TestSpace.TestExtensions");

		await TestAssistants.RunGeneratorAsync(code,
			[
				(typeof(AutoDeconstructGenerator), "TestSpace.Test_AutoDeconstruct.g.cs", generatedCode),
				(typeof(AutoDeconstructGenerator), "TestSpace.Test_TargetAutoDeconstruct.g.cs", generatedCode)
			],
			[typeDefinitionDiagnostic, memberDefinitionDiagnostic]);
	}
}
