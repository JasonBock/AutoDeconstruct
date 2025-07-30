using Microsoft.CodeAnalysis;

namespace AutoDeconstruct.Descriptors;

internal static class ExtensionDeconstructExistsDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(ExtensionDeconstructExistsDescriptor.Id,
			ExtensionDeconstructExistsDescriptor.Title,
			ExtensionDeconstructExistsDescriptor.Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				ExtensionDeconstructExistsDescriptor.Id));

	internal const string Id = "AUTO3";
	internal const string Message = "An existing extension Deconstruct() method already exists for the target type.";
	internal const string Title = "Adding Deconstruct Attributes";
}