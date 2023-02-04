namespace AutoDeconstruct;

internal record TypeSymbolModel(
	string? ContainingNamespace,
	string Name,
	string GenericParameters,
	string FullyQualifiedName,
	string Constraints,
	bool IsValueType,
	EquatableArray<PropertySymbolModel> AccessibleProperties);
