using AutoDeconstruct.Extensions;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace AutoDeconstruct;

// Lifted/inspired from:
// https://stackoverflow.com/questions/64623689/get-all-types-from-compilation-using-roslyn
internal sealed class CompilationTypesCollector
	: SymbolVisitor
{
	private readonly CancellationToken cancellationToken;
	private readonly HashSet<INamedTypeSymbol> types = [];
	private readonly HashSet<INamedTypeSymbol> excludedTypes = [];

	public CompilationTypesCollector(IAssemblySymbol assemblySymbol, CancellationToken cancellation)
	{
		this.cancellationToken = cancellation;
		this.types = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
		this.VisitAssembly(assemblySymbol);

		this.types.RemoveWhere(_ => this.excludedTypes.Contains(_));

		(this.Types, this.ExcludedTypes) = ([.. this.types], [.. this.excludedTypes]);
	}

	internal ImmutableArray<INamedTypeSymbol> GetTypes() => [.. this.types];

	public override void VisitAssembly(IAssemblySymbol symbol)
	{
		this.cancellationToken.ThrowIfCancellationRequested();
		symbol.GlobalNamespace.Accept(this);
	}

	public override void VisitNamespace(INamespaceSymbol symbol)
	{
		foreach (var namespaceOrType in symbol.GetMembers())
		{
			this.cancellationToken.ThrowIfCancellationRequested();
			namespaceOrType.Accept(this);
		}
	}

	public override void VisitNamedType(INamedTypeSymbol type)
	{
		this.cancellationToken.ThrowIfCancellationRequested();

		if (this.types.Add(type))
		{
			foreach(var typeMember in type.GetMembers())
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				typeMember.Accept(this);
			}

			// TODO: Does GetMembers() also get nested types?
			//var nestedTypes = type.GetTypeMembers();

			//if (!nestedTypes.IsDefaultOrEmpty)
			//{
			//	foreach (var nestedType in nestedTypes)
			//	{
			//		this.cancellationToken.ThrowIfCancellationRequested();
			//		nestedType.Accept(this);
			//	}
			//}
		}
	}

	public override void VisitMethod(IMethodSymbol symbol)
	{
		if (symbol.IsExtensionMethod &&
			symbol.Name == Shared.DeconstructName &&
			symbol.ReturnsVoid &&
			symbol.Parameters.Length > 1 &&
			symbol.Parameters.Count(_ => _.RefKind == RefKind.Out) == symbol.Parameters.Length - 1)
		{
			if (symbol.Parameters[0].Type is INamedTypeSymbol parameterType)
			{
				var accessibleProperties = parameterType.GetAccessibleProperties();

				if (accessibleProperties.Length == symbol.Parameters.Length - 1)
				{
					this.excludedTypes.Add(parameterType);
				}
			}
		}
	}

	internal ImmutableHashSet<INamedTypeSymbol> ExcludedTypes { get; }
	internal ImmutableHashSet<INamedTypeSymbol> Types { get; }
}