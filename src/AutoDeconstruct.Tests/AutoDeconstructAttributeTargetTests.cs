﻿using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class AutoDeconstructAttributeTargetTests
{
	[Test]
	public static async Task GenerateWhenStaticPropertyExistsAsync()
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

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @namespace)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@namespace = @self.Namespace;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "TestSpace.Test_AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateAtTypeLevelAsync()
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
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @namespace)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@namespace = @self.Namespace;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "TestSpace.Test_AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateAtNestedTypeLevelAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Namespace { get; set; }

					[AutoDeconstruct]
					public class NestedTest
					{ 
						public string? NestedNamespace { get; set; }
					}
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static class NestedTestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test.NestedTest @self, out string? @nestedNamespace)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@nestedNamespace = @self.NestedNamespace;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "TestSpace.Test.NestedTest_AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}
}