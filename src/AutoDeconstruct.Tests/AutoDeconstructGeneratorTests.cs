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

namespace TestSpace
{
	public static partial class TestExtensions
	{
		public static void Deconstruct(this Test self, out string? id)
		{
			if(self is null) { throw new ArgumentNullException(nameof(self)); }
			id = self.Id;
		}
	}
}
";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "Test_AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithReferenceTypeAndMultipleProperties()
	{
		var code =
@"using System;

namespace TestSpace
{
	public class Test
	{ 
		public string? Name { get; set; }
		public Guid Id { get; set; }
		public int Value { get; set; }
	}
}";

		var generatedCode =
@"using System;

#nullable enable

namespace TestSpace
{
	public static partial class TestExtensions
	{
		public static void Deconstruct(this Test self, out string? name, out Guid id, out int value)
		{
			if(self is null) { throw new ArgumentNullException(nameof(self)); }
			(name, id, value) =
				(self.Name, self.Id, self.Value);
		}
	}
}
";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "Test_AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}