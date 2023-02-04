using AutoDeconstruct.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;

namespace AutoDeconstruct;

[Generator]
public sealed class AutoDeconstructGenerator : IIncrementalGenerator
{
	private const string DeconstructName = "Deconstruct";

	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var typeProvider = context.SyntaxProvider
			.CreateSyntaxProvider(
				static (node, _) => node is TypeDeclarationSyntax,
				static (context, token) =>
				{
					var symbol = context.SemanticModel.GetDeclaredSymbol((TypeDeclarationSyntax)context.Node, token);
					if (symbol is null)
					{
						return null;
					}
					else if (symbol is INamedTypeSymbol namedTypeSymbol &&
						namedTypeSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "AutoDeconstruct.NoAutoDeconstructAttribute"))
					{
						return null;
					}

					var accessibleProperties = symbol.GetAccessibleProperties();
					if (accessibleProperties.IsEmpty ||
						symbol.GetMembers().OfType<IMethodSymbol>().Any(
							m => m.Name == AutoDeconstructGenerator.DeconstructName &&
							!m.IsStatic && m.Parameters.Length == accessibleProperties.Length &&
							m.Parameters.All(p => p.RefKind == RefKind.Out)))
					{
						// There is an existing instance deconstruct.
						return null;
					}

					return new TypeSymbolModel(symbol.ContainingNamespace.ToString(), symbol.Name, symbol.GetGenericParameters(), symbol.GetFullyQualifiedName(), symbol.GetConstraints(), symbol.IsValueType, accessibleProperties);
				})
			.Where(static _ => _ is not null);

		var excludedTypesHavingExtensionDeconstruct = context.SyntaxProvider
			.CreateSyntaxProvider(
				static (node, _) => node is MethodDeclarationSyntax methodNode &&
					methodNode.Modifiers.Any(SyntaxKind.StaticKeyword) &&
					methodNode.Identifier.ValueText == AutoDeconstructGenerator.DeconstructName &&
					methodNode.ParameterList.Parameters.Count > 1 &&
					methodNode.ParameterList.Parameters[0].Modifiers.Any(SyntaxKind.ThisKeyword) &&
					(methodNode.ParameterList.Parameters.Count(parameter => parameter.Modifiers.Any(SyntaxKind.OutKeyword)) ==
						methodNode.ParameterList.Parameters.Count - 1),
				static (context, token) =>
				{
					var methodSymbol = context.SemanticModel.GetDeclaredSymbol((MethodDeclarationSyntax)context.Node, token);
					if (methodSymbol?.Parameters[0].Type is not INamedTypeSymbol type)
					{
						return null;
					}

					var accessibleProperties = type.GetAccessibleProperties();
					if (accessibleProperties.Length == methodSymbol.Parameters.Length - 1)
					{
						return new TypeSymbolModel(type.ContainingNamespace.ToString(), type.Name, type.GetGenericParameters(), type.GetFullyQualifiedName(), type.GetConstraints(), type.IsValueType, accessibleProperties);
					}

					return null;
				})
			.Where(static _ => _ is not null);

		context.RegisterSourceOutput(typeProvider.Collect().Combine(excludedTypesHavingExtensionDeconstruct.Collect()),
			(context, source) => CreateOutput(source.Left!, source.Right!, context));
	}

	private static void CreateOutput(ImmutableArray<TypeSymbolModel> types, ImmutableArray<TypeSymbolModel> excludedTypes, SourceProductionContext context)
	{
		if (types.Length > 0)
		{
			using var writer = new StringWriter();
			using var indentWriter = new IndentedTextWriter(writer, "\t");
			indentWriter.WriteLines(
				"""
				#nullable enable

				""");

			var wasBuildInvoked = false;

			foreach (var type in types.Distinct())
			{
				var accessibleProperties = type.AccessibleProperties;

				if (!excludedTypes.Contains(type))
				{
					AutoDeconstructBuilder.Build(indentWriter, type, accessibleProperties);
					wasBuildInvoked = true;
				}
			}

			if (wasBuildInvoked)
			{
				context.AddSource("AutoDeconstruct.g.cs",
					SourceText.From(writer.ToString(), Encoding.UTF8));
			}
		}
	}
}