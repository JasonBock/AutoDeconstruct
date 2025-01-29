using Microsoft.CodeAnalysis;

namespace AutoDeconstruct;

internal record PropertySymbolModel(string Name, string TypeFullyQualifiedName,
	Accessibility Accesibility, Accessibility TypeAccessibility);
