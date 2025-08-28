using AutoDeconstruct.Analysis.Descriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace AutoDeconstruct.Analysis;

/// <summary>
/// An analyzer that checks conditions when
/// <c>AutoDeconstructAttribute</c> or <c>TargetAutoDeconstructAttribute</c> are used.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AnalyzeAttributeUsageAnalyzer
	: DiagnosticAnalyzer
{
	/// <summary>
	/// Initializes the analyzer.
	/// </summary>
	/// <param name="context">An <see cref="AnalysisContext"/> instance.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is <see langword="null"/>.</exception>
	public override void Initialize(AnalysisContext context)
	{
		if (context is null)
		{
			throw new ArgumentNullException(nameof(context));
		}

		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterCompilationStartAction(compilationContext =>
			compilationContext.RegisterOperationAction(operationContext =>
			{
			   AnalyzeOperationAction(
					operationContext);
			}, OperationKind.Attribute));
	}

	private static void AnalyzeOperationAction(OperationAnalysisContext context)
	{
		static void ValidateAttributeData(OperationAnalysisContext context, INamedTypeSymbol? targetType)
		{
			if (targetType is not null)
			{
				var (_, issue) = TypeSymbolModel.GetModel(
					context.Compilation, targetType, CancellationToken.None);

				if (issue == TypeSymbolModelIssue.NoAccessibleProperties)
				{
					context.ReportDiagnostic(Diagnostic.Create(
						NoAccessiblePropertiesDescriptor.Create(),
						context.Operation.Syntax.GetLocation()));
				}
				else if (issue == TypeSymbolModelIssue.InstanceDeconstructExists)
				{
					context.ReportDiagnostic(Diagnostic.Create(
						InstanceDeconstructExistsDescriptor.Create(),
						context.Operation.Syntax.GetLocation()));
				}
			}
		}

		var attributeOperation = (IAttributeOperation)context.Operation;

		if (attributeOperation.Operation is IObjectCreationOperation attributeCreationOperation)
		{
			var typeAttribute = context.Compilation.GetTypeByMetadataName(Shared.AutoDeconstructAttributeName)!;
			var assemblyAttribute = context.Compilation.GetTypeByMetadataName(Shared.TargetAutoDeconstructAttributeName)!;

			if (SymbolEqualityComparer.Default.Equals(attributeCreationOperation.Constructor!.ContainingType, typeAttribute))
			{
				var targetType = context.ContainingSymbol as INamedTypeSymbol;

				ValidateAttributeData(context, targetType);
			}
			else if (SymbolEqualityComparer.Default.Equals(attributeCreationOperation.Constructor!.ContainingType, assemblyAttribute))
			{
				var targetType = (attributeCreationOperation.Arguments[0].Value as ITypeOfOperation)!.TypeOperand as INamedTypeSymbol;

				ValidateAttributeData(context, targetType);
			}
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[
			NoAccessiblePropertiesDescriptor.Create(),
			InstanceDeconstructExistsDescriptor.Create()
		];
}