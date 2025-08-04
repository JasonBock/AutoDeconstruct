using NUnit.Framework;

namespace AutoDeconstruct.Completions.Tests;

internal static class AddAttributeRefactoringTests
{
	[Test]
	public static async Task RunWhenTargetIsTypeDefinitionAsync()
	{
		var source =
			"""
			public class [|T|]argetUsage { }
			""";

		var fixedDefinitionSource =
			"""
			using AutoDeconstruct;

			[assembly: TargetAutoDeconstruct(typeof(TargetUsage))]

			public class TargetUsage { }
			""";

		var fixedSource =
			"""
			using AutoDeconstruct;

			[AutoDeconstruct]
			public class TargetUsage { }
			""";

		await TestAssistants.RunRefactoringAsync<AddAttributeRefactoring>(
			[("Source.cs", source)],
			[("Source.cs", fixedDefinitionSource)], 0, false, [], []);

		await TestAssistants.RunRefactoringAsync<AddAttributeRefactoring>(
			[("Source.cs", source)],
			[("Source.cs", fixedSource)], 1, false, [], []);
	}
}