using AutoDeconstruct.Descriptors;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Immutable;

namespace AutoDeconstruct;

/// <summary>
/// An analyzer that checks conditions when
/// <see cref="AutoDeconstructAttribute"/> or <see cref="TargetAutoDeconstructAttribute"/> are used.
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
				AnalyzeAttributeUsageAnalyzer.AnalyzeOperationAction(
					operationContext);
			}, OperationKind.Attribute));
	}

	private static void AnalyzeOperationAction(OperationAnalysisContext context)
	{
		static void ValidateAttributeData(OperationAnalysisContext context,
			INamedTypeSymbol? targetType, SearchForExtensionMethods searchForExtensionMethods)
		{
			if (targetType is not null)
			{
				var (_, issue) = TypeSymbolModel.GetModel(
					context.Compilation, targetType, searchForExtensionMethods, CancellationToken.None);

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
				if (issue == TypeSymbolModelIssue.ExtensionsDeconstructExists)
				{
					context.ReportDiagnostic(Diagnostic.Create(
						ExtensionDeconstructExistsDescriptor.Create(),
						context.Operation.Syntax.GetLocation()));
				}
			}
		}

		var attributeOperation = (IAttributeOperation)context.Operation;

		if (attributeOperation.Operation is IObjectCreationOperation attributeCreationOperation)
		{
			var typeAttribute = context.Compilation.GetTypeByMetadataName(typeof(AutoDeconstructAttribute).FullName)!;
			var assemblyAttribute = context.Compilation.GetTypeByMetadataName(typeof(TargetAutoDeconstructAttribute).FullName)!;

			if (SymbolEqualityComparer.Default.Equals(attributeCreationOperation.Constructor!.ContainingType, typeAttribute))
			{
				var targetType = (context.ContainingSymbol as INamedTypeSymbol);
				var search = (SearchForExtensionMethods)(int)(attributeCreationOperation.Arguments[0].Value as IConversionOperation)!.ConstantValue.Value!;

				ValidateAttributeData(context, targetType, search);
			}
			else if (SymbolEqualityComparer.Default.Equals(attributeCreationOperation.Constructor!.ContainingType, assemblyAttribute))
			{
				var targetType = ((attributeCreationOperation.Arguments[0].Value as ITypeOfOperation)!.TypeOperand as INamedTypeSymbol);
				var search = (SearchForExtensionMethods)(int)(attributeCreationOperation.Arguments[1].Value as IConversionOperation)!.ConstantValue.Value!;

				ValidateAttributeData(context, targetType, search);
			}
		}
	}

	/// <summary>
	/// Gets an array of supported diagnostics from this analyzer.
	/// </summary>
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
		[
			NoAccessiblePropertiesDescriptor.Create(),
			InstanceDeconstructExistsDescriptor.Create(),
			ExtensionDeconstructExistsDescriptor.Create()
		];
}