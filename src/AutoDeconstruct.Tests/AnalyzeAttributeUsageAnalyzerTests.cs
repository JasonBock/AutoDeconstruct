using AutoDeconstruct.Descriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class AnalyzeAttributeUsageAnalyzerTests
{
	[Test]
	public static async Task AnalyzeWhenAssemblyAttributeIsGoodAsync()
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
					public static Guid Id { get; set; }
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenAssemblyAttributeFindsNoAccessiblePropertiesAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: TargetAutoDeconstruct(typeof(TestSpace.Test))]

			namespace TestSpace
			{
				public class Test { }
			}
			""";

		var diagnostic = new DiagnosticResult(NoAccessiblePropertiesDescriptor.Id, DiagnosticSeverity.Error)
			.WithSpan(4, 12, 4, 57);
		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenTypeAttributeIsGoodAsync()
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
					public static Guid Id { get; set; }
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenTypeAttributeFindsNoAccessiblePropertiesAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public class Test { }
			}
			""";

		var diagnostic = new DiagnosticResult(NoAccessiblePropertiesDescriptor.Id, DiagnosticSeverity.Error)
			.WithSpan(6, 3, 6, 18);
		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, [diagnostic]);
	}
}