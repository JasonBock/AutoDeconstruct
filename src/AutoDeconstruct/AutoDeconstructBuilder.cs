using AutoDeconstruct.Configuration;
using AutoDeconstruct.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.CodeDom.Compiler;
using System.Collections.Immutable;
using System.Text;

namespace AutoDeconstruct;

internal sealed class AutoDeconstructBuilder
{
	public AutoDeconstructBuilder(ConfigurationValues configurationValues,
		INamedTypeSymbol type, ImmutableArray<IPropertySymbol> properties) =>
			this.Text = AutoDeconstructBuilder.Build(configurationValues, type, properties);

	private static SourceText Build(ConfigurationValues configurationValues,
		INamedTypeSymbol type, ImmutableArray<IPropertySymbol> properties)
	{
		using var writer = new StringWriter();
		using var indentWriter = new IndentedTextWriter(writer,
			configurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)configurationValues.IndentSize));

		indentWriter.WriteLines(
			"""
			#nullable enable

			""");

		if (!type.ContainingNamespace.IsGlobalNamespace)
		{
			indentWriter.WriteLines(
				$"""
				namespace {type.ContainingNamespace.ToDisplayString()};
				
				""");
		}

		indentWriter.WriteLines(
			$$"""
			public static partial class {{type.Name}}Extensions
			{
			""");
		indentWriter.Indent++;

		// TODO: I'd like to not call ToCamelCase() three different times,
		// so this may be a spot to optimize.
		var outParameters = string.Join(", ", properties.Select(_ =>
		{
			return $"out {_.Type.GetFullyQualifiedName()} @{_.Name.ToCamelCase()}";
		}));

		var namingContext = new VariableNamingContext(properties.Select(_ => _.Name.ToCamelCase()).ToImmutableArray());
		var genericParameters = type.TypeParameters.Length > 0 ?
			$"<{string.Join(", ", type.TypeParameters.Select(_ => _.Name))}>" :
			string.Empty;

		indentWriter.WriteLine(
			$$"""public static void Deconstruct{{genericParameters}}(this {{type.GetFullyQualifiedName()}} @{{namingContext["self"]}}, {{outParameters}})""");

		var constraints = type.GetConstraints();

		if(constraints.Length > 0) 
		{
			indentWriter.Indent++;
			indentWriter.WriteLine(constraints);
			indentWriter.Indent--;
		}

		indentWriter.WriteLine("{");
		indentWriter.Indent++;

		if (!type.IsValueType)
		{
			indentWriter.WriteLine($"global::System.ArgumentNullException.ThrowIfNull(@{namingContext["self"]});");
		}

		if (properties.Length == 1)
		{
			indentWriter.WriteLine($"@{properties[0].Name.ToCamelCase()} = @{namingContext["self"]}.{properties[0].Name};");
		}
		else
		{
			indentWriter.WriteLine($"({string.Join(", ", properties.Select(_ => $"@{_.Name.ToCamelCase()}"))}) =");
			indentWriter.Indent++;
			indentWriter.WriteLine($"({string.Join(", ", properties.Select(_ => $"@{namingContext["self"]}.{_.Name}"))});");
			indentWriter.Indent--;
		}

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
		indentWriter.Indent--;
		indentWriter.WriteLine("}");

		return SourceText.From(writer.ToString(), Encoding.UTF8);
	}

	public SourceText Text { get; private set; }
}