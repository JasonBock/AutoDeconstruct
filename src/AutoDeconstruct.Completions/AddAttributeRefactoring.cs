using AutoDeconstruct.Completions.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Composition;

namespace AutoDeconstruct.Completions;

/// <summary>
/// Suggests refactorings to create the
/// <c>AutoDeconstructAttribute</c> or <c>TargetAutoDeconstructAttribute</c> declaration for the type
/// at the current code location (if possible).
/// </summary>
[ExportCodeRefactoringProvider(LanguageNames.CSharp,
	Name = nameof(AddAttributeRefactoring))]
[Shared]
public sealed class AddAttributeRefactoring
	: CodeRefactoringProvider
{
	/// <inheritdoc />
	public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
	{
		var document = context.Document;

		if (document.Project.Solution.Workspace.Kind == WorkspaceKind.MiscellaneousFiles) { return; }

		var span = context.Span;
		var cancellationToken = context.CancellationToken;
		var model = await document.GetSemanticModelAsync(cancellationToken);

		if (model is null) { return; }

		var root = (CompilationUnitSyntax)await model.SyntaxTree.GetRootAsync(cancellationToken);
		var node = root.FindNode(span);

		// If the node is a TypeDeclarationSyntax, we can offer up
		// both attributes. Else, the only "realistic" thing to do
		// is offer [TargetAutoDeconstruct].
		// When we offer TargetAutoDeconstruct, we need to honor
		// what is configured.
		var actions = new List<CodeAction>(2);
		var deconstructTypeSymbol = node.FindParentSymbol(model, cancellationToken);

		if (deconstructTypeSymbol is not null)
		{
			// Create a [TargetAutoDeconstruct] code action regardless of whether the type exists
			// in the current compilation.
			var targetAction = await AddAttributeRefactoring.CreateTargetAutoDeconstructActionAsync(
				root, context, document, model, deconstructTypeSymbol, cancellationToken);

			if (targetAction is not null)
			{
				actions.Add(targetAction);
			}
		}

		if (node is TypeDeclarationSyntax typeDeclaration)
		{
			actions.Add(AddAttributeRefactoring.CreateAutoDeconstructAction(
				root, typeDeclaration, document));
		}

		context.RegisterRefactoring(
			CodeAction.Create("Add auto-deconstruction ...", [.. actions], false));
	}

	private static async Task<CodeAction?> CreateTargetAutoDeconstructActionAsync(CompilationUnitSyntax currentRoot,
		CodeRefactoringContext context, Document document, SemanticModel currentModel,
		INamedTypeSymbol deconstructTypeSymbol, CancellationToken cancellationToken)
	{
		if (deconstructTypeSymbol.IsUnboundGenericType)
		{
			deconstructTypeSymbol = deconstructTypeSymbol.OriginalDefinition;
		}

		// Figure out which document we should actually put the changes in.
		var options = context.TextDocument.Project.AnalyzerOptions
			.AnalyzerConfigOptionsProvider.GlobalOptions;

		if (options.TryGetValue("build_property.AutoDeconstructAttributeFile", out var mockFile))
		{
			var fullMockFilePath = document.Project.FilePath is not null ?
				Path.Combine(Path.GetDirectoryName(document.Project.FilePath), mockFile) :
				mockFile;
			var autoDeconstructDocument = document.Project.Documents.FirstOrDefault(_ => _.FilePath == fullMockFilePath);

			if (autoDeconstructDocument is not null)
			{
				document = autoDeconstructDocument;
				currentModel = (await document.GetSemanticModelAsync(cancellationToken))!;
				currentRoot =
					(CompilationUnitSyntax)(await (await autoDeconstructDocument.GetSemanticModelAsync(cancellationToken))!.SyntaxTree.GetRootAsync(cancellationToken));
			}
		}

		if (!(currentModel?.HasTargetAutoDeconstructAttributeDefinition(deconstructTypeSymbol) ?? false))
		{
			var newRoot = currentRoot.AddAttributeLists(
				SyntaxFactory.AttributeList(
					SyntaxFactory.SingletonSeparatedList(
						SyntaxFactory.Attribute(
							SyntaxFactory.IdentifierName("TargetAutoDeconstruct"))
						.WithArgumentList(
							SyntaxFactory.AttributeArgumentList(
								SyntaxFactory.SingletonSeparatedList(
									SyntaxFactory.AttributeArgument(
										SyntaxFactory.TypeOfExpression(
											SyntaxFactory.ParseName(deconstructTypeSymbol.Name))))))))
						.WithTarget(
							SyntaxFactory.AttributeTargetSpecifier(
								SyntaxFactory.Token(SyntaxKind.AssemblyKeyword))));

			if (!newRoot.HasUsing("AutoDeconstruct"))
			{
				newRoot = newRoot.AddUsings(
					SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("AutoDeconstruct")));
			}

			if (!deconstructTypeSymbol.ContainingNamespace.IsGlobalNamespace &&
				!string.IsNullOrWhiteSpace(deconstructTypeSymbol.ContainingNamespace.Name) &&
				!newRoot.HasUsing(deconstructTypeSymbol.ContainingNamespace.Name))
			{
				newRoot = newRoot.AddUsings(
					SyntaxFactory.UsingDirective(
						SyntaxFactory.ParseName(deconstructTypeSymbol.ContainingNamespace.Name)));
			}

			return CodeAction.Create("[TargetAutoDeconstruct]",
				token => Task.FromResult(document.WithSyntaxRoot(newRoot)));
		}

		return null;
	}

	private static CodeAction CreateAutoDeconstructAction(CompilationUnitSyntax currentRoot,
		TypeDeclarationSyntax typeDeclarationNode, Document document)
	{
		var newTypeDeclarationNode = typeDeclarationNode.AddAttributeLists(
			SyntaxFactory.AttributeList(
				SyntaxFactory.SingletonSeparatedList(
					SyntaxFactory.Attribute(
						SyntaxFactory.IdentifierName("AutoDeconstruct")))));
		var newRoot = currentRoot.ReplaceNode(typeDeclarationNode, newTypeDeclarationNode);

		if (!newRoot.HasUsing("AutoDeconstruct"))
		{
			newRoot = newRoot.AddUsings(
				SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("AutoDeconstruct")));
		}

		return CodeAction.Create("[AutoDeconstruct]",
			token => Task.FromResult(document.WithSyntaxRoot(newRoot)));
	}
}