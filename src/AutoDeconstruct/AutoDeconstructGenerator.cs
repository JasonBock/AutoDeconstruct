using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace AutoDeconstruct;

public static class StringExt
{
	public static void Do(this string self) { }
}


[Generator]
public sealed class AutoDeconstructGenerator
	: IIncrementalGenerator
{
   private const string DeconstructName = "Deconstruct";

   public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		static bool IsSyntaxTargetForGeneration(SyntaxNode node, CancellationToken token) =>
			// We're looking for either type definitions that are not records
			// or it's a method declaration that is an extension method with the name
			// "Deconstruct" and all of the parameters are outs
			node is TypeDeclarationSyntax typeNode && typeNode is not RecordDeclarationSyntax ||
				node is MethodDeclarationSyntax methodNode &&
					methodNode.Modifiers.Any(SyntaxKind.StaticKeyword) &&
					methodNode.Identifier.ValueText == AutoDeconstructGenerator.DeconstructName &&
					(methodNode.ReturnType is PredefinedTypeSyntax predefinedType && 
						predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword)) &&
					methodNode.ParameterList.Parameters.Count > 1 &&
					methodNode.ParameterList.Parameters[0].Modifiers.Any(SyntaxKind.ThisKeyword) &&
					(methodNode.ParameterList.Parameters.Count(parameter => parameter.Modifiers.Any(SyntaxKind.OutKeyword)) ==
						methodNode.ParameterList.Parameters.Count - 1);

		static ISymbol TransformTargets(GeneratorSyntaxContext context, CancellationToken token) =>
			context.SemanticModel.GetSymbolInfo(context.Node, token).Symbol!;

		var provider = context.SyntaxProvider
			.CreateSyntaxProvider(IsSyntaxTargetForGeneration, TransformTargets)
			.Where(static _ => _ is not null);

		var output = context.CompilationProvider.Combine(provider.Collect());

		context.RegisterSourceOutput(output,
			(context, source) => CreateOutput(source.Left, source.Right, context));
	}

	private static void CreateOutput(Compilation compilation,
		ImmutableArray<ISymbol> symbols, SourceProductionContext context)
	{
		var types = symbols.Where(_ => _ is INamedTypeSymbol).Distinct(SymbolEqualityComparer.Default)
			.Cast<INamedTypeSymbol>();

		var methods = symbols.Where(_ => _ is IMethodSymbol).Cast<IMethodSymbol>()
			.ToLookup(_ => _.Parameters[0].Type, _ => _, SymbolEqualityComparer.Default);

		foreach (var type in types)
		{
			var accessibleProperties = type.GetMembers().OfType<IPropertySymbol>()
				.Where(_ => _.GetMethod is not null && _.GetMethod.DeclaredAccessibility == Accessibility.Public).ToArray();

			if (accessibleProperties.Length > 0 && 
				!type.GetMembers().OfType<IMethodSymbol>()
					.Any(_ => _.Name == AutoDeconstructGenerator.DeconstructName &&
						!_.IsStatic && _.ReturnsVoid && 
						_.Parameters.Count(p => p.RefKind == RefKind.Out) == _.Parameters.Length &&
						_.Parameters.Length == accessibleProperties.Length) &&
				(!methods[type].Any(_ => _.Parameters.Length - 1 == accessibleProperties.Length)))
			{
			}
		}
	}
}