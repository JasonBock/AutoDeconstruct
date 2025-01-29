using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace AutoDeconstruct.Extensions;

internal static class INamedTypeSymbolExtensions
{
   internal static string GetGenericParameters(this INamedTypeSymbol self) => self.TypeParameters.Length > 0 ?
		   $"<{string.Join(", ", self.TypeParameters.Select(t => t.Name))}>" :
		   string.Empty;

	internal static EquatableArray<PropertySymbolModel> GetAccessibleProperties(this INamedTypeSymbol self)
	{
		var targetType = self;
		var accessiblePropertiesBuilder = ImmutableArray.CreateBuilder<PropertySymbolModel>();

		while (targetType is not null)
		{
			accessiblePropertiesBuilder.AddRange(targetType.GetMembers().OfType<IPropertySymbol>()
				.Where(p => !p.IsIndexer && p.GetMethod is not null &&
					(p.GetMethod.DeclaredAccessibility == Accessibility.Public ||
					p.GetMethod.DeclaredAccessibility == Accessibility.Internal))
				.Select(p => new PropertySymbolModel(
					p.Name, p.Type.GetFullyQualifiedName(), p.DeclaredAccessibility, p.Type.DeclaredAccessibility)));
			targetType = targetType.BaseType;
		}

		return accessiblePropertiesBuilder.ToImmutable();
	}

	internal static ImmutableArray<IPropertySymbol> GetAccessiblePropertySymbols(this INamedTypeSymbol self)
	{
		var targetType = self;
		var accessiblePropertiesBuilder = ImmutableArray.CreateBuilder<IPropertySymbol>();

		while (targetType is not null)
		{
			accessiblePropertiesBuilder.AddRange(targetType.GetMembers().OfType<IPropertySymbol>()
				.Where(p => !p.IsIndexer && p.GetMethod is not null &&
					(p.GetMethod.DeclaredAccessibility == Accessibility.Public ||
					p.GetMethod.DeclaredAccessibility == Accessibility.Internal)));
			targetType = targetType.BaseType;
		}

		return accessiblePropertiesBuilder.ToImmutable();
	}

	internal static string GetConstraints(this INamedTypeSymbol self)
	{
		if (self.TypeParameters.Length == 0)
		{
			return string.Empty;
		}
		else
		{
			var constraints = new List<string>(self.TypeParameters.Length);

			foreach (var parameter in self.TypeParameters)
			{
				var parameterConstraints = parameter.GetConstraints();

				if (parameterConstraints.Length > 0)
				{
					constraints.Add(parameterConstraints);
				}
			}

			return constraints.Count > 0 ?
				string.Join(" ", constraints) :
				string.Empty;
		}
	}

	private static string GetConstraints(this ITypeParameterSymbol self)
	{
		var constraints = new List<string>();

		// Based on what I've read here:
		// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/classes#1425-type-parameter-constraints
		// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/where-generic-type-constraint
		// ...
		// Things like notnull and unmanaged should go first

		// According to CS0449, if any of these constraints exist: 
		// 'class', 'struct', 'unmanaged', 'notnull', and 'default'
		// they should not be duplicated.
		// Side note, I don't know how to find if the 'default'
		// constraint exists.
		if (self.HasUnmanagedTypeConstraint)
		{
			constraints.Add("unmanaged");
		}
		else if (self.HasNotNullConstraint)
		{
			constraints.Add("notnull");
		}
		// Then class constraint (HasReferenceTypeConstraint) or struct (HasValueTypeConstraint)
		else if (self.HasReferenceTypeConstraint)
		{
			constraints.Add(self.ReferenceTypeConstraintNullableAnnotation == NullableAnnotation.Annotated ? "class?" : "class");
		}
		else if (self.HasValueTypeConstraint)
		{
			constraints.Add("struct");
		}

		// Then type constraints (classes first, then interfaces, then other generic type parameters)
		constraints.AddRange(self.ConstraintTypes.Where(_ => _.TypeKind == TypeKind.Class).Select(_ => _.GetFullyQualifiedName()));
		constraints.AddRange(self.ConstraintTypes.Where(_ => _.TypeKind == TypeKind.Interface).Select(_ => _.GetFullyQualifiedName()));
		constraints.AddRange(self.ConstraintTypes.Where(_ => _.TypeKind == TypeKind.TypeParameter).Select(_ => _.GetFullyQualifiedName()));

		// Then constructor constraint
		if (self.HasConstructorConstraint)
		{
			constraints.Add("new()");
		}

		return constraints.Count == 0 ? string.Empty :
			$"where {self.Name} : {string.Join(", ", constraints)}";
	}
}