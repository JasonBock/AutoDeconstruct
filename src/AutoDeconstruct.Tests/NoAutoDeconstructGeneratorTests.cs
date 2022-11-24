using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace AutoDeconstruct.Tests;

public static class NoAutoDeconstructGeneratorTests
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

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
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

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
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

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}