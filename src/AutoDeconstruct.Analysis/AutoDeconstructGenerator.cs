using AutoDeconstruct.Analysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Text;

namespace AutoDeconstruct.Analysis;

[Generator]
internal sealed class AutoDeconstructGenerator
	: IIncrementalGenerator
{
	public void Initialize(IncrementalGeneratorInitializationContext context)
	{
		var typeType = context.SyntaxProvider.ForAttributeWithMetadataName(
			"AutoDeconstruct.AutoDeconstructAttribute", (_, _) => true,
			(generatorContext, token) =>
			{
				var compilation = generatorContext.SemanticModel.Compilation;

				var attributeClass = generatorContext.Attributes[0];

				var targetType = (generatorContext.TargetSymbol as INamedTypeSymbol)!;
				var (model, _) = attributeClass.ConstructorArguments.Length == 0 ?
					TypeSymbolModel.GetModel(compilation, targetType,
						Filtering.None,
						[]) :
					TypeSymbolModel.GetModel(compilation, targetType,
						(Filtering)attributeClass.ConstructorArguments[0].Value!,
						[.. attributeClass.ConstructorArguments[1].Values.Select(_ => (string)_.Value!)]);

				return model;
			});

		var assemblyTypes = context.SyntaxProvider.ForAttributeWithMetadataName(
			"AutoDeconstruct.TargetAutoDeconstructAttribute", (_, _) => true,
			(generatorContext, token) =>
			{
				var types = new HashSet<TypeSymbolModel>();

				var compilation = generatorContext.SemanticModel.Compilation;

				for (var i = 0; i < generatorContext.Attributes.Length; i++)
				{
					var attributeClass = generatorContext.Attributes[i];
					var targetType = (attributeClass.ConstructorArguments[0].Value as INamedTypeSymbol)!;

					var (model, _) = attributeClass.ConstructorArguments.Length == 1 ?
						TypeSymbolModel.GetModel(compilation, targetType,
							Filtering.None,
							[]) :
						TypeSymbolModel.GetModel(compilation, targetType,
							(Filtering)attributeClass.ConstructorArguments[1].Value!,
							[.. attributeClass.ConstructorArguments[2].Values.Select(_ => (string)_.Value!)]);

					if (model is not null)
					{
						types.Add(model);
					}
				}

				return new EquatableArray<TypeSymbolModel>([.. types]);
			});

		context.RegisterSourceOutput(typeType,
			(context, source) =>
			{
				if (source is not null)
				{
					CreateOutput(new EquatableArray<TypeSymbolModel>([source]), context, "AutoDeconstruct.g.cs");
				}
			});
		context.RegisterSourceOutput(assemblyTypes,
			(context, source) => CreateOutput(source, context, "TargetAutoDeconstruct.g.cs"));
	}

	private static void CreateOutput(EquatableArray<TypeSymbolModel> types, SourceProductionContext context,
		string fileName)
	{
		if (types.Length > 0)
		{
			foreach (var type in types)
			{
				using var writer = new StringWriter();
				using var indentWriter = new IndentedTextWriter(writer, "\t");
				indentWriter.WriteLines(
					"""
					#nullable enable

					""");

				var accessibleProperties = type.AccessibleProperties;

				AutoDeconstructBuilder.Build(indentWriter, type, accessibleProperties);
				context.AddSource($"{type.FullyQualifiedName.GenerateFileName()}_{fileName}",
					SourceText.From(writer.ToString(), Encoding.UTF8));
			}
		}
	}
}