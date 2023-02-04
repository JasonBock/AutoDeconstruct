using AutoDeconstruct.Extensions;
using Microsoft.CodeAnalysis;
using System.CodeDom.Compiler;
using System.Collections.Immutable;

namespace AutoDeconstruct;

internal static class AutoDeconstructBuilder
{
	internal static void Build(IndentedTextWriter writer,
		TypeSymbolModel type, ImmutableArray<PropertySymbolModel> properties)
	{
		if (type.ContainingNamespace is not null)
		{
			writer.WriteLines(
				$$"""
				namespace {{type.ContainingNamespace}}
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
		var outParameters = string.Join(", ", properties.Select(p =>
		{
			return $"out {p.TypeFullyQualifiedName} @{p.Name.ToCamelCase()}";
		}));

		var namingContext = new VariableNamingContext(properties.Select(p => p.Name.ToCamelCase()).ToImmutableArray());

		writer.WriteLine(
			$$"""public static void Deconstruct{{type.GenericParameters}}(this {{type.FullyQualifiedName}} @{{namingContext["self"]}}, {{outParameters}})""");

		var constraints = type.Constraints;

		if (constraints.Length > 0)
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
			writer.WriteLine($"({string.Join(", ", properties.Select(p => $"@{p.Name.ToCamelCase()}"))}) =");
			writer.Indent++;
			writer.WriteLine($"({string.Join(", ", properties.Select(p => $"@{namingContext["self"]}.{p.Name}"))});");
			writer.Indent--;
		}

		writer.Indent--;
		writer.WriteLine("}");
		writer.Indent--;
		writer.WriteLine("}");

		if (type.ContainingNamespace is not null)
		{
			writer.Indent--;
			writer.WriteLine("}");
		}
	}
}