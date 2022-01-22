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

	// TODO: Once the testing packages have ref assemblies for .NET 6.0,
	// we can change the gen'd code to be file-scoped namespace,
	// as well as use ArgumentNullException.ThrowIfNull().
	private static SourceText Build(ConfigurationValues configurationValues,
		INamedTypeSymbol type, ImmutableArray<IPropertySymbol> properties)
	{
		using var writer = new StringWriter();
		using var indentWriter = new IndentedTextWriter(writer,
			configurationValues.IndentStyle == IndentStyle.Tab ? "\t" : new string(' ', (int)configurationValues.IndentSize));

		var namespaces = new NamespaceGatherer();

		if (!type.ContainingNamespace.IsGlobalNamespace)
		{
			indentWriter.WriteLine($"namespace {type.ContainingNamespace.ToDisplayString()}");
			indentWriter.WriteLine("{");
			indentWriter.Indent++;
		}

		indentWriter.WriteLine($"public static partial class {type.Name}Extensions");
		indentWriter.WriteLine("{");
		indentWriter.Indent++;

		// TODO: I'd like to not call ToCamelCase() three different times,
		// so this may be a spot to optimize.
		var outParameters = string.Join(", ", properties.Select(_ =>
		{
			namespaces.Add(_.Type.ContainingNamespace);
			return $"out {_.Type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)} {_.Name.ToCamelCase()}";
		}));

		indentWriter.WriteLine($"public static void Deconstruct(this {type.Name} self, {outParameters})");
		indentWriter.WriteLine("{");
		indentWriter.Indent++;

		if (!type.IsValueType)
		{
			namespaces.Add(typeof(ArgumentNullException));
			indentWriter.WriteLine("if(self is null) { throw new ArgumentNullException(nameof(self)); }");
		}

		if (properties.Length == 1)
		{
			indentWriter.WriteLine($"{properties[0].Name.ToCamelCase()} = self.{properties[0].Name};");
		}
		else
		{
			indentWriter.WriteLine($"({string.Join(", ", properties.Select(_ => _.Name.ToCamelCase()))}) =");
			indentWriter.Indent++;
			indentWriter.WriteLine($"({string.Join(", ", properties.Select(_ => $"self.{_.Name}"))});");
			indentWriter.Indent--;
		}

		indentWriter.Indent--;
		indentWriter.WriteLine("}");
		indentWriter.Indent--;
		indentWriter.WriteLine("}");

		if (!type.ContainingNamespace.IsGlobalNamespace)
		{
			indentWriter.Indent--;
			indentWriter.WriteLine("}");
		}

		var code = namespaces.Values.Count > 0 ?
			string.Join(Environment.NewLine,
				string.Join(Environment.NewLine, namespaces.Values.Select(_ => $"using {_};")),
				string.Empty, "#nullable enable", string.Empty, writer.ToString()) :
			string.Join(Environment.NewLine, "#nullable enable", string.Empty, writer.ToString());

		return SourceText.From(code, Encoding.UTF8);
	}

	public SourceText Text { get; private set; }
}