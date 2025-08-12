using Microsoft.CodeAnalysis;

namespace AutoDeconstruct.Analysis.Descriptors;

internal static class InstanceDeconstructExistsDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(Id,
			Title,
			Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				Id));

	internal const string Id = "AUTO2";
	internal const string Message = "An existing Deconstruct() method already exists on the target type.";
	internal const string Title = "Adding Deconstruct Attributes";
}