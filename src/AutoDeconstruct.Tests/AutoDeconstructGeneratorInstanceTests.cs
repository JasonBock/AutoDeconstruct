using NUnit.Framework;

namespace AutoDeconstruct.Tests;

internal static class AutoDeconstructGeneratorInstanceTests
{
	[Test]
	public static async Task GenerateWithInternalPropertyUsingPublicTypeAsync()
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
					internal string? Namespace { get; set; }
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
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWithInternalPropertyUsingInternalTypeAsync()
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
					internal InternalType InternalValue { get; set; }
				}

				internal class InternalType { }
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				internal static class TestExtensions
				{
					internal static void Deconstruct(this global::TestSpace.Test @self, out global::TestSpace.InternalType @internalValue)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@internalValue = @self.InternalValue;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWithInternalTypeAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				internal class Test
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
				internal static class TestExtensions
				{
					internal static void Deconstruct(this global::TestSpace.Test @self, out string? @namespace)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@namespace = @self.Namespace;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWithPropertyNameThatIsAKeywordAsync()
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
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWithReferenceTypeAndOnePropertyAsync()
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
					public string? Id { get; set; }
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
	public static async Task GenerateWithReferenceTypeAndMultiplePropertiesAsync()
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
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }
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
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWithValueTypeAndOnePropertyAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public struct Test
				{ 
					public string? Id { get; set; }
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
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @id)
					{
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
	public static async Task GenerateWithValueTypeAndMultiplePropertiesAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public struct Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }
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
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWithRecordAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public record Test()
				{
					public string? Id { get; init; }
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
	public static async Task GenerateWithRecordThatHasDeconstructAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public record Test(string Id);
			}
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[],
			[]);
	}

	[Test]
	public static async Task GenerateWithNoAccesiblePropertiesAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public struct Test
				{ 
					private string? Name { get; set; }
					private Guid Id { get; set; }
					private int Value { get; set; }
				}
			}
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[],
			[]);
	}

	[Test]
	public static async Task GenerateWithNoDeconstructMatchAsync()
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
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }

					public void Deconstruct(out int value, out string? name) =>
						(value, name) = (this.Value, this.Name);
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
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWithDeconstructNotReturningVoidAsync()
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
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }

					public int Deconstruct(out int value, out string? name, out Guid id)
					{
						(value, name, id) = (this.Value, this.Name, this.Id);
						return 3;
					}
				}
			}
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[],
			[]);
	}

	[Test]
	public static async Task GenerateWithExistingDeconstructButWithNonOutParametersAsync()
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
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }

					public void Deconstruct(out int value, out string? name, int[] values) =>
						(value, name) = (this.Value, this.Name);
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
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWithMatchingDeconstructAsync()
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
					public string? Name { get; set; }
					public Guid Id { get; set; }
					public int Value { get; set; }

					public void Deconstruct(out int value, out string? name, out Guid id) =>
						(value, name, id) = (this.Value, this.Name, this.Id);
				}
			}
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[],
			[]);
	}

	[Test]
	public static async Task GenerateWhenPartialDefinitionsExistAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public partial class Test
				{ 
					public string? Name { get; set; }
				}

				public partial class Test
				{ 
					public Guid Id { get; set; }
					public int Value { get; set; }

					public void Deconstruct(out int value, out string? name, int[] values) =>
						(value, name) = (this.Value, this.Name);
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
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out global::System.Guid @id, out int @value)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id, @value) =
							(@self.Name, @self.Id, @self.Value);
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenPropertiesExistInInheritanceHierarchyAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public class BaseTest
				{ 
					public int Id { get; set; }
				}

				[AutoDeconstruct]
				public class Test
					: BaseTest
				{ 
					public string? Name { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable
			
			namespace TestSpace
			{
				public static class BaseTestExtensions
				{
					public static void Deconstruct(this global::TestSpace.BaseTest @self, out int @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						@id = @self.Id;
					}
				}
			}
			namespace TestSpace
			{
				public static class TestExtensions
				{
					public static void Deconstruct(this global::TestSpace.Test @self, out string? @name, out int @id)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self);
						(@name, @id) =
							(@self.Name, @self.Id);
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}

	[Test]
	public static async Task GenerateWhenSelfPropertyExistsAsync()
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
					public string? Self { get; set; }
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
					public static void Deconstruct(this global::TestSpace.Test @self1, out string? @self)
					{
						global::System.ArgumentNullException.ThrowIfNull(@self1);
						@self = @self1.Self;
					}
				}
			}
			
			""";

		await TestAssistants.RunGeneratorAsync(code,
			[(typeof(AutoDeconstructGenerator), "AutoDeconstruct.g.cs", generatedCode)],
			[]);
	}
}