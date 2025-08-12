using Microsoft.CodeAnalysis;

namespace AutoDeconstruct.Analysis.Descriptors;

internal static class NoAccessiblePropertiesDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(Id,
			Title,
			Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				Id));

	internal const string Id = "AUTO1";
	internal const string Message = "No accessible properties were found.";
	internal const string Title = "Adding Deconstruct Attributes";
}