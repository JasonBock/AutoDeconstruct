using AutoDeconstruct.Analysis.Extensions;
using Microsoft.CodeAnalysis;

namespace AutoDeconstruct.Analysis;

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
	internal static (TypeSymbolModel?, TypeSymbolModelIssue) GetModel(
		Compilation compilation, INamedTypeSymbol targetType, 
		Filtering filtering, string[] properties)
	{
		var accessibleProperties = targetType.GetAccessibleProperties(filtering, properties);

		if (accessibleProperties.IsEmpty)
		{
			// There are no accessible properties.
			return (null, TypeSymbolModelIssue.NoAccessibleProperties);
		}
		else if (targetType.GetMembers().OfType<IMethodSymbol>().Any(
			m => m.Name == Shared.DeconstructName &&
				!m.IsStatic && m.Parameters.Length == accessibleProperties.Length &&
				m.Parameters.All(p => p.RefKind == RefKind.Out)))
		{
			// There is an existing instance deconstruct.

			// Note: Deconstruct methods cannot be overloaded if they have
			// the same number of out parameters, so if we find one
			// that has the same number of parameters as accessible properties,
			// that's enough to make it an excluded type target.
			return (null, TypeSymbolModelIssue.InstanceDeconstructExists);
		}

		var containingNamespace = targetType.ContainingNamespace is not null ?
			!targetType.ContainingNamespace.IsGlobalNamespace ?
				targetType.ContainingNamespace.ToDisplayString() :
				null :
			null;

		// Create the unique name for the extension type.
		var extensionName = $"{targetType.Name}Extensions";
		var extensionFQN = containingNamespace is null ?
			 $"{extensionName}" :
			 $"{containingNamespace}.{extensionName}";
		var existingType = compilation.GetTypeByMetadataName(extensionFQN);

		int? id = null;

		while (existingType is not null)
		{
			id = id is null ? 2 : id++;
			extensionName = $"{targetType.Name}Extensions{id}";
			extensionFQN = containingNamespace is null ?
				$"{extensionName}Extensions" :
				$"{containingNamespace}.{extensionName}Extensions";
			existingType = compilation.GetTypeByMetadataName(extensionFQN);
		}

		var model = new TypeSymbolModel(targetType.DeclaredAccessibility,
			containingNamespace, targetType.Name,
			extensionName,
			targetType.GetGenericParameters(), targetType.GetFullyQualifiedName(),
			targetType.GetConstraints(), targetType.IsValueType, accessibleProperties);

		return (model, TypeSymbolModelIssue.None);
	}
}