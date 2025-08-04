using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoDeconstruct.Completions.Extensions;

internal static class SemanticModelExtensions
{
	internal static bool HasTargetAutoDeconstructAttributeDefinition(this SemanticModel self, INamedTypeSymbol targetTypeSymbol)
	{
		var assemblyAttributeListSyntaxNodes = self.SyntaxTree.GetRoot()
			.DescendantNodes(_ => true)
			.OfType<AttributeListSyntax>()
			.Where(_ => _.DescendantTokens(_ => true)
				.Any(_ => _.RawKind == (int)SyntaxKind.AssemblyKeyword))
			.ToArray();

		if (assemblyAttributeListSyntaxNodes.Length > 0)
		{
			foreach (var assemblyAttributeList in assemblyAttributeListSyntaxNodes)
			{
				foreach (var attributeSyntax in assemblyAttributeList.Attributes)
				{
					var attributeCtor = self.GetSymbolInfo(attributeSyntax).Symbol as IMethodSymbol;

					if (attributeCtor is not null &&
						attributeCtor.ContainingType.Name == "TargetAutoDeconstruct" &&
						attributeCtor.ContainingType.ContainingNamespace.ToDisplayString() == "AutoDeconstruct" &&
						attributeCtor.ContainingType.ContainingAssembly.ToDisplayString().StartsWith("AutoDeconstruct"))
					{
						if (attributeSyntax.ArgumentList?.Arguments[0].Expression is TypeOfExpressionSyntax attributeTypeOf)
						{
							if (self.GetSymbolInfo(attributeTypeOf.Type).Symbol is INamedTypeSymbol attributeTypeOfSymbol &&
								SymbolEqualityComparer.Default.Equals(attributeTypeOfSymbol, targetTypeSymbol))
							{
								return true;
							}
						}
					}
				}
			}
		}

		return false;
	}
}
