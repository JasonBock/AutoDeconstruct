using AutoDeconstruct.Analysis.Descriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Testing;
using NUnit.Framework;
using System.Globalization;

namespace AutoDeconstruct.Analysis.Tests;

internal static class AnalyzeAttributeUsageAnalyzerTests
{
	[Test]
	public static void VerifySupportedDiagnostics()
	{
		var analyzer = new AnalyzeAttributeUsageAnalyzer();
		var diagnostics = analyzer.SupportedDiagnostics;

		using (Assert.EnterMultipleScope())
		{
			Assert.That(diagnostics, Has.Length.EqualTo(2), nameof(diagnostics.Length));
			Assert.That(diagnostics.All(_ => _.Category == DescriptorConstants.Usage), Is.True, nameof(diagnostics));
			Assert.That(diagnostics.All(_ => _.DefaultSeverity == DiagnosticSeverity.Error), Is.True, nameof(diagnostics));
			Assert.That(diagnostics.All(_ => _.IsEnabledByDefault), Is.True, nameof(diagnostics));

			var noAccessiblePropertiesDiagnostic = diagnostics.Single(_ => _.Id == NoAccessiblePropertiesDescriptor.Id);
			Assert.That(noAccessiblePropertiesDiagnostic.Title.ToString(CultureInfo.CurrentCulture),
				Is.EqualTo(NoAccessiblePropertiesDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(noAccessiblePropertiesDiagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture),
				Is.EqualTo(NoAccessiblePropertiesDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(noAccessiblePropertiesDiagnostic.HelpLinkUri,
				Is.EqualTo(HelpUrlBuilder.Build(NoAccessiblePropertiesDescriptor.Id)),
				nameof(DiagnosticDescriptor.HelpLinkUri));

			var instanceDeconstructDiagnostic = diagnostics.Single(_ => _.Id == InstanceDeconstructExistsDescriptor.Id);
			Assert.That(instanceDeconstructDiagnostic.Title.ToString(CultureInfo.CurrentCulture),
				Is.EqualTo(InstanceDeconstructExistsDescriptor.Title),
				nameof(DiagnosticDescriptor.Title));
			Assert.That(instanceDeconstructDiagnostic.MessageFormat.ToString(CultureInfo.CurrentCulture),
				Is.EqualTo(InstanceDeconstructExistsDescriptor.Message),
				nameof(DiagnosticDescriptor.MessageFormat));
			Assert.That(instanceDeconstructDiagnostic.HelpLinkUri,
				Is.EqualTo(HelpUrlBuilder.Build(InstanceDeconstructExistsDescriptor.Id)),
				nameof(DiagnosticDescriptor.HelpLinkUri));
		}
	}

	[Test]
	public static async Task AnalyzeWhenAssemblyAttributeIsGoodAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: TargetAutoDeconstruct(typeof(TestSpace.Test))]

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Namespace { get; set; }
					public static Guid Id { get; set; }
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenAssemblyAttributeFindsNoAccessiblePropertiesAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: TargetAutoDeconstruct(typeof(TestSpace.Test))]

			namespace TestSpace
			{
				public class Test { }
			}
			""";

		var diagnostic = new DiagnosticResult(NoAccessiblePropertiesDescriptor.Id, DiagnosticSeverity.Error)
			.WithSpan(4, 12, 4, 57);
		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenAssemblyAttributeFindsInstanceDeconstructAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: TargetAutoDeconstruct(typeof(TestSpace.Test))]

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }

					public void Deconstruct(out Guid id, out string name) =>
						(id, name) = (this.Id, this.Name);
				}
			}
			""";

		var diagnostic = new DiagnosticResult(InstanceDeconstructExistsDescriptor.Id, DiagnosticSeverity.Error)
			.WithSpan(4, 12, 4, 57);
		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenAssemblyAttributeFindsExtensionDeconstructAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			[assembly: TargetAutoDeconstruct(typeof(TestSpace.Test))]

			namespace TestSpace
			{
				public class Test
				{ 
					public string? Name { get; set; }
					public Guid Id { get; set; }
				}

				public static class TestExtensions
				{
					public static void Deconstruct(this Test self, out Guid id, out string name) =>
						(id, name) = (self.Id, self.Name);
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenTypeAttributeIsGoodAsync()
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

		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, []);
	}

	[Test]
	public static async Task AnalyzeWhenTypeAttributeFindsNoAccessiblePropertiesAsync()
	{
		var code =
			"""
			using AutoDeconstruct;
			using System;

			namespace TestSpace
			{
				[AutoDeconstruct]
				public class Test { }
			}
			""";

		var diagnostic = new DiagnosticResult(NoAccessiblePropertiesDescriptor.Id, DiagnosticSeverity.Error)
			.WithSpan(6, 3, 6, 18);
		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenTypeAttributeFindsInstanceDeconstructAsync()
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
			
					public void Deconstruct(out Guid id, out string name) =>
						(id, name) = (this.Id, this.Name);				
				}
			}
			""";

		var diagnostic = new DiagnosticResult(InstanceDeconstructExistsDescriptor.Id, DiagnosticSeverity.Error)
			.WithSpan(6, 3, 6, 18);
		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, [diagnostic]);
	}

	[Test]
	public static async Task AnalyzeWhenTypeAttributeFindsExtensionDeconstructAsync()
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
				}

				public static class TestExtensions
				{
					public static void Deconstruct(this Test self, out Guid id, out string name) =>
						(id, name) = (self.Id, self.Name);				
				}
			}
			""";

		await TestAssistants.RunAnalyzerAsync<AnalyzeAttributeUsageAnalyzer>(code, []);
	}
}