using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class AutoDeconstructGeneratorExtensionMethodTests
{
	[Test]
	public static async Task GenerateWhereThisTypeDoesNotMatch()
	{
		var code =
			"""
			using System;

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
						id = "3";
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			[]);
	}

	[Test]
	public static async Task GenerateWhereOutParameterCountDoesNotMatch()
	{
		var code =
			"""
			using System;

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
						(id, value) = ("3", 3);
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static partial class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			new[] { (typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode) },
			[]);
	}

	[Test]
	public static async Task GenerateWhereOutParameterCountMatches()
	{
		var code =
			"""
			using System;

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
						id = "3";
				}
			}
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[],
			[]);
	}
}