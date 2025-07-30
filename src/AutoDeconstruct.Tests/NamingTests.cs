using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class NamingTests
{
	[Test]
	public static async Task GenerateWithDuplicateExtensionNameAsync()
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
					public int Id { get; set; }
				}

				public class TestExtensions { }
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static class TestExtensions2
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out int @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "TestSpace.Test_AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}
}
