using AutoDeconstruct.Configuration;
using AutoDeconstruct.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;

namespace AutoDeconstruct;

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
			node is TypeDeclarationSyntax typeNode ||
				(node is MethodDeclarationSyntax methodNode &&
					methodNode.Modifiers.Any(SyntaxKind.StaticKeyword) &&
					methodNode.Identifier.ValueText == AutoDeconstructGenerator.DeconstructName &&
					methodNode.ParameterList.Parameters.Count > 1 &&
					methodNode.ParameterList.Parameters[0].Modifiers.Any(SyntaxKind.ThisKeyword) &&
					(methodNode.ParameterList.Parameters.Count(parameter => parameter.Modifiers.Any(SyntaxKind.OutKeyword)) ==
						methodNode.ParameterList.Parameters.Count - 1));

		static ISymbol TransformTargets(GeneratorSyntaxContext context, CancellationToken token) =>
			context.SemanticModel.GetDeclaredSymbol(context.Node, token)!;

		var provider = context.SyntaxProvider
			 .CreateSyntaxProvider(IsSyntaxTargetForGeneration, TransformTargets)
			 .Where(static _ => _ is not null);
		var compilationNodes = context.CompilationProvider.Combine(provider.Collect());
		var output = context.AnalyzerConfigOptionsProvider.Combine(compilationNodes);

		context.RegisterSourceOutput(output,
			(context, source) => CreateOutput(source.Right.Left, source.Right.Right, source.Left, context));
	}

	private static void CreateOutput(Compilation compilation,
		ImmutableArray<ISymbol> symbols, AnalyzerConfigOptionsProvider options, SourceProductionContext context)
	{
		if (symbols.Length > 0)
		{
			var noAutoDeconstructAttribute = compilation.GetTypeByMetadataName(typeof(NoAutoDeconstructAttribute).FullName);

			var types = symbols.Where(_ => _ is INamedTypeSymbol)
				.Distinct(SymbolEqualityComparer.Default)
				.Cast<INamedTypeSymbol>()
				.Where(_ => !_.GetAttributes().Any(data =>
					data.AttributeClass is not null &&
					data.AttributeClass.Equals(noAutoDeconstructAttribute, SymbolEqualityComparer.Default)));

			var methods = symbols.Where(_ => _ is IMethodSymbol).Cast<IMethodSymbol>()
				.ToLookup(_ => _.Parameters[0].Type, _ => _, SymbolEqualityComparer.Default);

			var configurationValues = new ConfigurationValues(options, symbols[0].DeclaringSyntaxReferences[0].SyntaxTree);
			using var writer = new StringWriter();
			using var indentWriter = new IndentedTextWriter(writer,
				configurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)configurationValues.IndentSize));
			indentWriter.WriteLines(
				"""
				#nullable enable

				""");

			var wasBuildInvoked = false;

			foreach (var type in types)
			{
				var accessiblePropertiesBuilder = ImmutableArray.CreateBuilder<IPropertySymbol>();

				var targetType = type;

				while (targetType is not null)
				{
					accessiblePropertiesBuilder.AddRange(targetType.GetMembers().OfType<IPropertySymbol>()
						.Where(_ => !_.IsIndexer && _.GetMethod is not null &&
							_.GetMethod.DeclaredAccessibility == Accessibility.Public));
					targetType = targetType.BaseType;
				}

				var accessibleProperties = accessiblePropertiesBuilder.ToImmutable();

				if (accessibleProperties.Length > 0 &&
					!type.GetMembers().OfType<IMethodSymbol>()
						.Any(_ => _.Name == AutoDeconstructGenerator.DeconstructName &&
							!_.IsStatic &&
							_.Parameters.Count(p => p.RefKind == RefKind.Out) == _.Parameters.Length &&
							_.Parameters.Length == accessibleProperties.Length) &&
					(!methods[type].Any(_ => _.Parameters.Length - 1 == accessibleProperties.Length)))
				{
					AutoDeconstructBuilder.Build(indentWriter, type, accessibleProperties);
					wasBuildInvoked = true;
				}
			}

			if(wasBuildInvoked)
			{
				context.AddSource("AutoDeconstruct.g.cs", 
					SourceText.From(writer.ToString(), Encoding.UTF8));
			}
		}
	}
}