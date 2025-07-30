using Microsoft.CodeAnalysis;

namespace AutoDeconstruct.Descriptors;

internal static class NoAccessiblePropertiesDescriptor
{
	internal static DiagnosticDescriptor Create() =>
		new(NoAccessiblePropertiesDescriptor.Id,
			NoAccessiblePropertiesDescriptor.Title,
			NoAccessiblePropertiesDescriptor.Message,
			DescriptorConstants.Usage, DiagnosticSeverity.Error, true,
			helpLinkUri: HelpUrlBuilder.Build(
				NoAccessiblePropertiesDescriptor.Id));

	internal const string Id = "AUTO1";
	internal const string Message = "No accessible properties were found.";
	internal const string Title = "Adding Deconstruct Attributes";
}