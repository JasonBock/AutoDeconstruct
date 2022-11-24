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
				public static void Deconstruct<T>(this global::TestSpace.Test<T> @self, out T @id, out T @value)
				{
					global::System.ArgumentNullException.ThrowIfNull(@self);
					(@id, @value) =
						(@self.Id, @self.Value);
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
				public static void Deconstruct<T>(this global::TestSpace.Test<T> @self, out T @id, out T @value)
					where T : class
				{
					global::System.ArgumentNullException.ThrowIfNull(@self);
					(@id, @value) =
						(@self.Id, @self.Value);
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "Test_AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenTypeHasMultipleGenericsAsync()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test<T1, T2>
				{ 
					public T1 Id { get; set; }
					public T2 Value { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace;
			
			public static partial class TestExtensions
			{
				public static void Deconstruct<T1, T2>(this global::TestSpace.Test<T1, T2> @self, out T1 @id, out T2 @value)
				{
					global::System.ArgumentNullException.ThrowIfNull(@self);
					(@id, @value) =
						(@self.Id, @self.Value);
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "Test_AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenTypeHasMultipleGenericsWithMultipleConstraintsAsync()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public interface IAmForStruct { }

				public class Test<T1, T2>
					where T1 : struct, IAmForStruct
					where T2 : unmanaged, IAmForStruct
				{ 
					public T1 Id { get; set; }
					public T2 Value { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace;
			
			public static partial class TestExtensions
			{
				public static void Deconstruct<T1, T2>(this global::TestSpace.Test<T1, T2> @self, out T1 @id, out T2 @value)
					where T1 : struct, global::TestSpace.IAmForStruct where T2 : unmanaged, global::TestSpace.IAmForStruct
				{
					global::System.ArgumentNullException.ThrowIfNull(@self);
					(@id, @value) =
						(@self.Id, @self.Value);
				}
			}
			
			""";

		await TestAssistants.RunAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "Test_AutoDeconstruct.g.cs", generatedCode) },
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}