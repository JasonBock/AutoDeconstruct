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
		static string[] GetFilteredProperties(IOperation value)
		{
			static string GetElementValue(IOperation elementOperation) =>
				elementOperation switch
				{
					INameOfOperation nameOfOperation => (string)nameOfOperation.ConstantValue.Value!,
					ILiteralOperation literalOperation => (string)literalOperation.ConstantValue.Value!,
					IBinaryOperation binaryOperation => (string)binaryOperation.ConstantValue.Value!,
					_ => throw new NotSupportedException($"Type of operation, {elementOperation.GetType().FullName}, is not supported.")
				};

			var properties = new List<string>();

			if (value is IArrayCreationOperation arrayCreationOperation)
			{
				foreach(var elementValue in arrayCreationOperation.Initializer!.ElementValues)
				{
					properties.Add(GetElementValue(elementValue));
				}
			}
			else if (value is IConversionOperation conversionOperation)
			{
				foreach (var elementValue in (conversionOperation.Operand! as ICollectionExpressionOperation)!.Elements)
				{
					properties.Add(GetElementValue(elementValue));
				}
			}
			else
			{
				throw new NotSupportedException($"Type of operation, {value.GetType().FullName}, is not supported.");
			}

			return [.. properties];
		}

		static void ValidateAttributeData(OperationAnalysisContext context, INamedTypeSymbol? targetType,
			Filtering filtering, string[] properties)
		{
			if (targetType is not null)
			{
				var (_, issue) = TypeSymbolModel.GetModel(
					context.Compilation, targetType, filtering, properties);

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

			var attributeType = attributeCreationOperation.Constructor!.ContainingType;

			if (SymbolEqualityComparer.Default.Equals(attributeType, typeAttribute))
			{
				var targetType = context.ContainingSymbol as INamedTypeSymbol;

				if (attributeCreationOperation.Arguments.Length == 0)
				{
					ValidateAttributeData(context, targetType, Filtering.None, []);
				}
				else
				{
					ValidateAttributeData(context, targetType,
						(Filtering)attributeCreationOperation.Arguments[0].Value!.ConstantValue.Value!,
						GetFilteredProperties(attributeCreationOperation.Arguments[1].Value!));
				}
			}
			else if (SymbolEqualityComparer.Default.Equals(attributeType, assemblyAttribute))
			{
				var targetType = (attributeCreationOperation.Arguments[0].Value as ITypeOfOperation)!.TypeOperand as INamedTypeSymbol;

				if (attributeCreationOperation.Arguments.Length == 1)
				{
					ValidateAttributeData(context, targetType, Filtering.None, []);
				}
				else
				{
					ValidateAttributeData(context, targetType,
						(Filtering)attributeCreationOperation.Arguments[1].Value!.ConstantValue.Value!,
						GetFilteredProperties(attributeCreationOperation.Arguments[2].Value!));
				}
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