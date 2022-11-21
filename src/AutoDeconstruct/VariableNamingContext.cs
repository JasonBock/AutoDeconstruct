using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace AutoDeconstruct;

internal sealed class VariableNamingContext
{
	private readonly ImmutableArray<string> parameterNames;
	private readonly Dictionary<string, string> variables = new();

	internal VariableNamingContext(IMethodSymbol method) :
		this(method.Parameters)
	{ }

	internal VariableNamingContext(ImmutableArray<IParameterSymbol> parameters) :
		this(parameters.Select(_ => _.Name).ToImmutableArray())
	{ }

	internal VariableNamingContext(ImmutableArray<string> parameterNames) =>
		this.parameterNames = parameterNames;

	internal string this[string variableName]
	{
		get
		{
			if (!this.variables.ContainsKey(variableName))
			{
				var uniqueName = variableName;
				var id = 1;

				while (this.parameterNames.Any(_ => _ == uniqueName) ||
					this.variables.ContainsKey(uniqueName))
				{
					uniqueName = $"{variableName}{id++}";
				}

				this.variables.Add(variableName, uniqueName);
			}

			return this.variables[variableName];
		}
	}
}