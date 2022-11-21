using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;

namespace AutoDeconstruct.Tests;

public static class AutoDeconstructGeneratorInstanceTests
{
	[Test]
	public static async Task GenerateWithReferenceTypeAndOneProperty()
	{
		var code =
			"""
			using System;

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Id { get; set; }
				}
			}
			""";

		var generatedCode =
			"""
			#nullable enable

			namespace TestSpace;

			public static partial class TestExtensions
			{
				public static void Deconstruct(this global::TestSpace.Test self, out string? id)
				{
					global::System.ArgumentNullException.ThrowIfNull(self);
					id = self.Id;
				}
			}

			""";

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

	[Test]
	public static async Task GenerateWithValueTypeAndOneProperty()
	{
		var code =
@"using System;

namespace TestSpace
{
	public struct Test
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
	public static async Task GenerateWithValueTypeAndMultipleProperties()
	{
		var code =
@"using System;

namespace TestSpace
{
	public struct Test
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

	[Test]
	public static async Task GenerateWithRecord()
	{
		var code =
@"using System;

namespace TestSpace
{
	public record Test(string? Id);
}";

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithNoAccesibleProperties()
	{
		var code =
@"using System;

namespace TestSpace
{
	public struct Test
	{ 
		private string? Name { get; set; }
		private Guid Id { get; set; }
		private int Value { get; set; }
	}
}";

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithNoDeconstructMatch()
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

		public void Deconstruct(out int value, out string? name) =>
			(value, name) = (this.Value, this.Name);
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

	[Test]
	public static async Task GenerateWithDeconstructNotReturningVoid()
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

		public int Deconstruct(out int value, out string? name, out Guid id)
		{
			(value, name, id) = (this.Value, this.Name, this.Id);
			return 3;
		}
	}
}";

		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWithExistingDeconstructButWithNonOutParameters()
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

		public void Deconstruct(out int value, out string? name, int[] values) =>
			(value, name) = (this.Value, this.Name);
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

	[Test]
	public static async Task GenerateWithMatchingDeconstruct()
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

		public void Deconstruct(out int value, out string? name, out Guid id) =>
			(value, name, id) = (this.Value, this.Name, this.Id);
	}
}";
		await TestAssistants.RunAsync(code,
			Enumerable.Empty<(Type, string, string)>(),
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}

	[Test]
	public static async Task GenerateWhenPartialDefinitionsExist()
	{
		var code =
@"using System;

namespace TestSpace
{
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

	[Test]
	public static async Task GenerateWhenPropertiesExistInInheritanceHierarchy()
	{
		var code =
@"using System;

namespace TestSpace
{
	public class BaseTest
	{ 
		public int Id { get; set; }
	}

	public class Test
		: BaseTest
	{ 
		public string? Name { get; set; }
	}
}";

		var generatedBaseTestCode =
@"using System;

#nullable enable

namespace TestSpace
{
	public static partial class BaseTestExtensions
	{
		public static void Deconstruct(this BaseTest self, out int id)
		{
			if(self is null) { throw new ArgumentNullException(nameof(self)); }
			id = self.Id;
		}
	}
}
";

		var generatedTestCode =
@"using System;

#nullable enable

namespace TestSpace
{
	public static partial class TestExtensions
	{
		public static void Deconstruct(this Test self, out string? name, out int id)
		{
			if(self is null) { throw new ArgumentNullException(nameof(self)); }
			(name, id) =
				(self.Name, self.Id);
		}
	}
}
";

		await TestAssistants.RunAsync(code,
			new[] 
			{ 
				(typeof(AutoDeconstructGenerator), "BaseTest_AutoDeconstruct.g.cs", generatedBaseTestCode),
				(typeof(AutoDeconstructGenerator), "Test_AutoDeconstruct.g.cs", generatedTestCode) 
			},
			Enumerable.Empty<DiagnosticResult>()).ConfigureAwait(false);
	}
}