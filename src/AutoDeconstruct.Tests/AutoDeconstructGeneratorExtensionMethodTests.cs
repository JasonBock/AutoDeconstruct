using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace AutoDeconstruct.Tests;

public static class AutoDeconstructGeneratorExtensionMethodTests
{
	[Test]
	public static async Task GenerateWhereThisTypeDoesNotMatch()
	{
		var code =
@"using System;

namespace TestSpace
{
	public class NotATest { }

	public class Test
	{ 
		public string? Id { get; set; }
	}

	public static class MyTestExtensions
	{
		public static void Deconstruct(this NotATest self, out string? id) =>
			id = ""3"";
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
	public static async Task GenerateWhereOutParameterCountDoesNotMatch()
	{
		var code =
@"using System;

namespace TestSpace
{
	public class NotATest { }

	public class Test
	{ 
		public string? Id { get; set; }
	}

	public static class MyTestExtensions
	{
		public static void Deconstruct(this Test self, out string? id, out int value) =>
			(id, value) = (""3"", 3);
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
	public static async Task GenerateWhereOutParameterCountMatches()
	{
		var code =
@"using System;

namespace TestSpace
{
	public class NotATest { }

	public class Test
	{ 
		public string? Id { get; set; }
	}

	public static class MyTestExtensions
	{
		public static void Deconstruct(this Test self, out string? id) =>
			id = ""3"";
	}
}";

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}