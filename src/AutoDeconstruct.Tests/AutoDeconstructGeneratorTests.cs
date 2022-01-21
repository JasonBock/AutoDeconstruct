using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace AutoDeconstruct.Tests;

public static class AutoDeconstructGeneratorTests
{
	[Test]
	public static async Task GenerateWithReferenceTypeAndOneProperty()
	{
		var code =
@"using System;

namespace TestSpace
{
	public class Test
	{ 
		public string? Id { get; set; }
	}
}";

		var generatedCode =
@"using System;

#nullable enable

namespace TestSpace;

public static partial class TestExtensions
{
	public static void Deconstruct(this Test self, out string? id) =>
		self is null ? throw new ArgumentNullException(nameof(self)) :
			id = self.Id;
}
";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "DataSpace_AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}