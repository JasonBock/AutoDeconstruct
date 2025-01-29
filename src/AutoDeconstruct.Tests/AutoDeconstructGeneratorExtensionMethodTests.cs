using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class AutoDeconstructGeneratorExtensionMethodTests
{
	[Test]
	public static async Task GenerateWhereThisTypeDoesNotMatchAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				public class NotATest { }

				[AutoDeconstruct]
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
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhereOutParameterCountDoesNotMatchAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				public class NotATest { }

				[AutoDeconstruct]
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
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhereOutParameterCountMatchesAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				public class NotATest { }

				[AutoDeconstruct]
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

	[Test]
	public static async Task GenerateWhereOutParameterCountMatchesAndImplicitConversionExistsAsync()
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
					public DerivedType Id { get; set; }
				}

				public class BaseType { }

				public class DerivedType 
					: BaseType { }

				public static class MyTestExtensions
				{
					public static void Deconstruct(this Test self, out BaseType id) =>
						id = self.Id;
				}
			}
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[],
			[]);
	}

	[Test]
	public static async Task GenerateWhereOutParameterCountMatchesAndImplicitConversionDoesNotExistAsync()
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
					public BaseType Id { get; set; }
				}

				public class BaseType { }

				public class DerivedType 
					: BaseType { }

				public static class MyTestExtensions
				{
					public static void Deconstruct(this Test self, out DerivedType id) =>
						id = new DerivedType();
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
					public static void Deconstruct(this global::TestSpace.Test @self, out global::TestSpace.BaseType @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}
}