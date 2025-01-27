using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using NuGet.Frameworks;

namespace AutoDeconstruct.Tests;

using GeneratorTest = CSharpSourceGeneratorTest<AutoDeconstructGenerator, DefaultVerifier>;

internal static class TestAssistants
{
	internal static async Task RunGeneratorAsync(string code,
		IEnumerable<(Type, string, string)> generatedSources,
		IEnumerable<DiagnosticResult> expectedDiagnostics)
   {
		var test = new GeneratorTest
		{
			ReferenceAssemblies = TestAssistants.GetNet90(),
			TestState =
			{
				Sources = { code },
			},
		};

		foreach (var generatedSource in generatedSources)
		{
			test.TestState.GeneratedSources.Add(generatedSource);
		}

		test.TestState.AdditionalReferences.Add(typeof(AutoDeconstructGenerator).Assembly);
		test.TestState.ExpectedDiagnostics.AddRange(expectedDiagnostics);
		await test.RunAsync();
	}

	private static ReferenceAssemblies GetNet90()
	{
	  // Always look here for the latest version of a particular runtime:
	  // https://www.nuget.org/packages/Microsoft.NETCore.App.Ref
		if (!NuGetFramework.Parse("net9.0").IsPackageBased)
		{
			// The NuGet version provided at runtime does not recognize the 'net9.0' target framework
			throw new NotSupportedException("The 'net9.0' target framework is not supported by this version of NuGet.");
		}

		return new ReferenceAssemblies(
			 "net9.0",
			 new PackageIdentity(
				  "Microsoft.NETCore.App.Ref",
				  "9.0.1"),
			 Path.Combine("ref", "net9.0"));
	}
}