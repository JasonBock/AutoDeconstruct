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
		var typeTypes = context.SyntaxProvider.ForAttributeWithMetadataName(
			"AutoDeconstruct.AutoDeconstructAttribute", (_, _) => true,
			(generatorContext, token) =>
			{
				var types = new List<TypeSymbolModel>();

				var compilation = generatorContext.SemanticModel.Compilation;

				for (var i = 0; i < generatorContext.Attributes.Length; i++)
				{
					var attributeClass = generatorContext.Attributes[i];

					var targetType = (generatorContext.TargetSymbol as INamedTypeSymbol)!;
					var search = (SearchForExtensionMethods)attributeClass.ConstructorArguments[0].Value!;

					if (search == SearchForExtensionMethods.No ||
						!new IsTypeExcludedVisitor(compilation.Assembly, targetType, token).IsExcluded)
					{
						var typeModel = TypeSymbolModel.GetModel(compilation, targetType);

						if (typeModel is not null)
						{
							types.Add(typeModel);
						}
					}
				}

				return types;
			})
			.SelectMany((names, _) => names);

		var assemblyTypes = context.SyntaxProvider.ForAttributeWithMetadataName(
			"AutoDeconstruct.TargetAutoDeconstructAttribute", (_, _) => true,
			(generatorContext, token) =>
			{
				var types = new List<TypeSymbolModel>();

				var compilation = generatorContext.SemanticModel.Compilation;

				for (var i = 0; i < generatorContext.Attributes.Length; i++)
				{
					var attributeClass = generatorContext.Attributes[i];

					var targetType = (attributeClass.ConstructorArguments[0].Value as INamedTypeSymbol)!;
					var search = (SearchForExtensionMethods)attributeClass.ConstructorArguments[1].Value!;

					if (search == SearchForExtensionMethods.No ||
						!new IsTypeExcludedVisitor(compilation.Assembly, targetType, token).IsExcluded)
					{
						var typeModel = TypeSymbolModel.GetModel(compilation, targetType);

						if (typeModel is not null)
						{
							types.Add(typeModel);
						}
					}
				}

				return types;
			})
			.SelectMany((names, _) => names);

		context.RegisterSourceOutput(typeTypes.Collect(),
			(context, source) => CreateOutput(source, context, "AutoDeconstruct.g.cs"));
		context.RegisterSourceOutput(assemblyTypes.Collect(),
			(context, source) => CreateOutput(source, context, "TargetAutoDeconstruct.g.cs"));
	}

	private static void CreateOutput(ImmutableArray<TypeSymbolModel> types, SourceProductionContext context,
		string fileName)
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
				context.AddSource(fileName,
					SourceText.From(writer.ToString(), Encoding.UTF8));
			}
		}
	}
}