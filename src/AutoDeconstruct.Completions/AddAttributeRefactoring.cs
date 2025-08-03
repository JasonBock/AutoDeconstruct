using AutoDeconstruct.Completions.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Composition;
using System.Xml.Linq;

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

		var solution = document.Project.Solution;
		var span = context.Span;
		var cancellationToken = context.CancellationToken;
		var model = await document.GetSemanticModelAsync(cancellationToken);

		if (model is null) { return; }

		var root = (CompilationUnitSyntax)await model.SyntaxTree.GetRootAsync(cancellationToken);
		var node = root.FindNode(span);
		var deconstructTypeSymbol = node.FindParentSymbol(model, cancellationToken);

		if (deconstructTypeSymbol is not null)
		{
			var actions = new List<CodeAction>(2)
			{
			   // Create a [TargetAutoDeconstruct] code action regardless of whether the type exists
			   // in the current compilation.
			   AddAttributeRefactoring.CreateTargetAutoDeconstructAction()
			};

			// Only offer [AutoDeconstruct] if the type is in the
			// current compilation
			if (deconstructTypeSymbol.DeclaringSyntaxReferences.Length > 0)
			{
				var reference = deconstructTypeSymbol.DeclaringSyntaxReferences.Single(_ => _ == node.GetReference());

				var typeDeclarationNode = (await reference.GetSyntaxAsync()) as TypeDeclarationSyntax;

				if (typeDeclarationNode is not null)
				{
					var typeDeclarationDocument = solution.GetDocument(typeDeclarationNode.SyntaxTree);

					if (typeDeclarationDocument is not null)
					{
						actions.Add(AddAttributeRefactoring.CreateAutoDeconstructAction(
							root, typeDeclarationNode, typeDeclarationDocument));
					}
				}
			}

			context.RegisterRefactoring(
				CodeAction.Create("Add auto-deconstruction ...", [.. actions], false));
		}
	}

	private static CodeAction CreateTargetAutoDeconstructAction()
	{

	}

	private static CodeAction CreateAutoDeconstructAction(CompilationUnitSyntax currentRoot,
		TypeDeclarationSyntax typeDeclarationNode, Document document)
	{
		var newTypeDeclarationNode = typeDeclarationNode.AddAttributeLists([]);
		var newRoot = currentRoot.ReplaceNode(typeDeclarationNode, newTypeDeclarationNode);

		return CodeAction.Create("[AutoDeconstruct]",
			token => Task.FromResult(document.WithSyntaxRoot(newRoot)));
	}
}