using AutoDeconstruct.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;

namespace AutoDeconstruct;

[Generator]
internal sealed class AutoDeconstructGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		static TypeSymbolModel? GetModel(INamedTypeSymbol type, bool isAttributeAtAssemblyLevel)
		{
			if (isAttributeAtAssemblyLevel && 
				type.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString() == "AutoDeconstruct.NoAutoDeconstructAttribute"))
			{
				return null;
			}

			var accessibleProperties = type.GetAccessibleProperties();
			if (accessibleProperties.IsEmpty ||
				type.GetMembers().OfType<IMethodSymbol>().Any(
					m => m.Name == Shared.DeconstructName &&
					!m.IsStatic && m.Parameters.Length == accessibleProperties.Length &&
					m.Parameters.All(p => p.RefKind == RefKind.Out)))
			{
				// There is an existing instance deconstruct.
				return null;
			}

			return new TypeSymbolModel(type.DeclaredAccessibility,
				type.ContainingNamespace.ToString(), type.Name,
				type.GetGenericParameters(), type.GetFullyQualifiedName(),
				type.GetConstraints(), type.IsValueType, accessibleProperties);
		}

		var types = context.SyntaxProvider.ForAttributeWithMetadataName(
			"AutoDeconstruct.AutoDeconstructAttribute", (_, _) => true,
			(generatorContext, token) =>
			{
				var types = new List<TypeSymbolModel>();

				var collectedTypes = new CompilationTypesCollector(
					generatorContext.SemanticModel.Compilation.Assembly, token);

				for (var i = 0; i < generatorContext.Attributes.Length; i++)
				{
					// Is the attribute on the assembly, or a type?
					var attributeClass = generatorContext.Attributes[i];

					if (generatorContext.TargetSymbol is IAssemblySymbol assemblySymbol)
					{
						foreach (var assemblyType in collectedTypes.Types)
						{
							var typeModel = GetModel(assemblyType, true);

							if (typeModel is not null)
							{
								types.Add(typeModel);
							}
						}
					}
					else if (generatorContext.TargetSymbol is INamedTypeSymbol typeSymbol)
					{
						if (!collectedTypes.ExcludedTypes.Contains(typeSymbol))
						{
							var typeModel = GetModel(typeSymbol, false);

							if (typeModel is not null)
							{
								types.Add(typeModel);
							}
						}
					}
				}

				return types;
			})
			.SelectMany((names, _) => names);


		context.RegisterSourceOutput(types.Collect(),
			(context, source) => CreateOutput(source, context));
	}

	private static void CreateOutput(ImmutableArray<TypeSymbolModel> types, SourceProductionContext context)
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

				AutoDeconstructBuilder.Build(indentWriter, type, accessibleProperties);
				wasBuildInvoked = true;
			}

			if (wasBuildInvoked)
			{
				context.AddSource("AutoDeconstruct.g.cs",
					SourceText.From(writer.ToString(), Encoding.UTF8));
			}
		}
	}
}