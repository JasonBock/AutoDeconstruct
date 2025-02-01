using AutoDeconstruct.Extensions;
using Microsoft.CodeAnalysis;

namespace AutoDeconstruct;

internal sealed class IsTypeExcludedVisitor
	: SymbolVisitor
{
	private readonly CancellationToken cancellationToken;
	private readonly INamedTypeSymbol targetType;

	public IsTypeExcludedVisitor(IAssemblySymbol assembly, INamedTypeSymbol targetType,
		CancellationToken cancellation)
	{
		this.targetType = targetType;
		this.cancellationToken = cancellation;
		this.VisitAssembly(assembly);
	}

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

		if (!this.IsExcluded)
		{
			foreach (var typeMember in type.GetTypeMembers())
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				typeMember.Accept(this);
			}

			foreach (var typeMember in type.GetMembers(Shared.DeconstructName).OfType<IMethodSymbol>())
			{
				this.cancellationToken.ThrowIfCancellationRequested();
				typeMember.Accept(this);
			}
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
			if (symbol.Parameters[0].Type is INamedTypeSymbol parameterType &&
				SymbolEqualityComparer.Default.Equals(parameterType, this.targetType))
			{
				var accessibleProperties = parameterType.GetAccessibleProperties();

				if (accessibleProperties.Length == symbol.Parameters.Length - 1)
				{
					// Note: Deconstruct methods cannot be overloaded if they have
					// the same number of out parameters, so if we find one
					// that has the same number of parameters as accessible properties,
					// that's enough to make it an excluded type target.
					this.IsExcluded = true;
				}
			}
		}
	}

	internal bool IsExcluded { get; private set; }
}
