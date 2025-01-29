using Microsoft.CodeAnalysis;

namespace AutoDeconstruct;

internal sealed record TypeSymbolModel(
	Accessibility Accessibility,
	string? ContainingNamespace,
	string Name,
	string GenericParameters,
	string FullyQualifiedName,
	string Constraints,
	bool IsValueType,
	EquatableArray<PropertySymbolModel> AccessibleProperties);
