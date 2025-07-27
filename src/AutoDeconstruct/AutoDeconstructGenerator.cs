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
		var types = context.SyntaxProvider.ForAttributeWithMetadataName(
			"AutoDeconstruct.AutoDeconstructAttribute", (_, _) => true,
			(generatorContext, token) =>
			{
				var types = new List<TypeSymbolModel>();

				var compilation = generatorContext.SemanticModel.Compilation;

				for (var i = 0; i < generatorContext.Attributes.Length; i++)
				{
					// Is the attribute on the assembly, or a type?
					var attributeClass = generatorContext.Attributes[i];

					var targetType = generatorContext.TargetSymbol is IAssemblySymbol ?
						(attributeClass.ConstructorArguments[0].Value as INamedTypeSymbol) :
						generatorContext.TargetSymbol as INamedTypeSymbol;

					if (targetType is not null)
					{
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