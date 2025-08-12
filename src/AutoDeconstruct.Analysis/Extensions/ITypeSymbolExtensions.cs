using Microsoft.CodeAnalysis;

namespace AutoDeconstruct.Analysis.Extensions;

internal static class ITypeSymbolExtensions
{
	internal static string GetFullyQualifiedName(this ITypeSymbol self)
	{
		var symbolFormatter = SymbolDisplayFormat.FullyQualifiedFormat.
			AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);
		return self.ToDisplayString(symbolFormatter);
	}
}