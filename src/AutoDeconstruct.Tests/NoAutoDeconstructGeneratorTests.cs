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
	public static void GenerateWhenAttributeExistsOnStruct()
	{

	}

	[Test]
	public static void GenerateWhenAttributeExistsOnInterface()
	{

	}
}