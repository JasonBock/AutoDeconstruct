using Microsoft.CodeAnalysis;

namespace AutoDeconstruct.Analysis;

internal sealed record PropertySymbolModel(
	string Name,
	string CamelCaseName,
	string TypeFullyQualifiedName,
	Accessibility Accesibility, 
	Accessibility TypeAccessibility);
