using AutoDeconstruct.Extensions;
using Microsoft.CodeAnalysis;

namespace AutoDeconstruct;

internal sealed record TypeSymbolModel(
	Accessibility Accessibility,
	string? ContainingNamespace,
	string Name,
	string ExtensionsName,
	string GenericParameters,
	string FullyQualifiedName,
	string Constraints,
	bool IsValueType,
	EquatableArray<PropertySymbolModel> AccessibleProperties)
{
	internal static TypeSymbolModel? GetModel(Compilation compilation, INamedTypeSymbol type)
	{
		var accessibleProperties = type.GetAccessibleProperties();

		if (accessibleProperties.IsEmpty ||
			type.GetMembers().OfType<IMethodSymbol>().Any(
				m => m.Name == Shared.DeconstructName &&
					!m.IsStatic && m.Parameters.Length == accessibleProperties.Length &&
					m.Parameters.All(p => p.RefKind == RefKind.Out)))
		{
			// There is an existing instance deconstruct.
			return null;
		}

		var containingNamespace = type.ContainingNamespace is not null ?
			!type.ContainingNamespace.IsGlobalNamespace ?
				type.ContainingNamespace.ToDisplayString() :
				null :
			null;

		// Create the unique name for the extension type.
		var extensionName = $"{type.Name}Extensions";
		var extensionFQN = containingNamespace is null ?
			 $"{extensionName}" :
			 $"{containingNamespace}.{extensionName}";
		var existingType = compilation.GetTypeByMetadataName(extensionFQN);

		int? id = null;

		while (existingType is not null)
		{
			id = id is null ? 2 : id++;
			extensionName = $"{type.Name}Extensions{id}";
			extensionFQN = containingNamespace is null ?
				$"{extensionName}Extensions" :
				$"{containingNamespace}.{extensionName}Extensions";
			existingType = compilation.GetTypeByMetadataName(extensionFQN);
		}

		return new TypeSymbolModel(type.DeclaredAccessibility,
			containingNamespace, type.Name,
			extensionName,
			type.GetGenericParameters(), type.GetFullyQualifiedName(),
			type.GetConstraints(), type.IsValueType, accessibleProperties);
	}
}