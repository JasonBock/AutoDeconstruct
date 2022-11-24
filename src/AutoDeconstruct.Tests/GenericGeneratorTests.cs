using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace AutoDeconstruct.Tests;

public static class GenericGeneratorTests
{
	[Test]
	public static async Task GenerateWhenTypeHasGenericsAsync()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test<T>
				{ 
					public T Id { get; set; }
					public T Value { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace;
			
			public static partial class TestExtensions
			{
				public static void Deconstruct<T>(this global::TestSpace.Test<T> self, out T id, out T value)
				{
					global::System.ArgumentNullException.ThrowIfNull(self);
					(id, value) =
						(self.Id, self.Value);
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "Test_AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenTypeHasGenericsWithConstraintsAsync()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test<T>
					where T : class
				{ 
					public T Id { get; set; }
					public T Value { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace;
			
			public static partial class TestExtensions
			{
				public static void Deconstruct<T>(this global::TestSpace.Test<T> self, out T id, out T value)
					where T : class
				{
					global::System.ArgumentNullException.ThrowIfNull(self);
					(id, value) =
						(self.Id, self.Value);
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "Test_AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}