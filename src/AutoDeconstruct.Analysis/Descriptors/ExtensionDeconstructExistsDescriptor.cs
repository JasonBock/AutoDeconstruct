using Microsoft.CodeAnalysis;

namespace AutoDeconstruct.Analysis.Descriptors;

internal static class ExtensionDeconstructExistsDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(Id,
			Title,
			Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				Id));

	internal const string Id = "AUTO3";
	internal const string Message = "An existing extension Deconstruct() method already exists for the target type.";
	internal const string Title = "Adding Deconstruct Attributes";
}