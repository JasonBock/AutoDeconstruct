using AutoDeconstruct.Extensions;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace AutoDeconstruct;

internal static class AutoDeconstructBuilder
{
	internal static void Build(IndentedTextWriter writer,
		INamedTypeSymbol type, ImmutableArray<IPropertySymbol> properties)
	{
		if (!type.ContainingNamespace.IsGlobalNamespace)
		{
			writer.WriteLines(
				$$"""
				namespace {{type.ContainingNamespace.ToDisplayString()}}
				{
				""");
			writer.Indent++;
		}

		writer.WriteLines(
			$$"""
			public static partial class {{type.Name}}Extensions
			{
			""");
		writer.Indent++;

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

		writer.WriteLine(
			$$"""public static void Deconstruct{{genericParameters}}(this {{type.GetFullyQualifiedName()}} @{{namingContext["self"]}}, {{outParameters}})""");

		var constraints = type.GetConstraints();

		if(constraints.Length > 0) 
		{
			writer.Indent++;
			writer.WriteLine(constraints);
			writer.Indent--;
		}

		writer.WriteLine("{");
		writer.Indent++;

		if (!type.IsValueType)
		{
			writer.WriteLine($"global::System.ArgumentNullException.ThrowIfNull(@{namingContext["self"]});");
		}

		if (properties.Length == 1)
		{
			writer.WriteLine($"@{properties[0].Name.ToCamelCase()} = @{namingContext["self"]}.{properties[0].Name};");
		}
		else
		{
			writer.WriteLine($"({string.Join(", ", properties.Select(_ => $"@{_.Name.ToCamelCase()}"))}) =");
			writer.Indent++;
			writer.WriteLine($"({string.Join(", ", properties.Select(_ => $"@{namingContext["self"]}.{_.Name}"))});");
			writer.Indent--;
		}

		writer.Indent--;
		writer.WriteLine("}");
		writer.Indent--;
		writer.WriteLine("}");

		if (!type.ContainingNamespace.IsGlobalNamespace)
		{
			writer.Indent--;
			writer.WriteLine("}");
		}
	}
}