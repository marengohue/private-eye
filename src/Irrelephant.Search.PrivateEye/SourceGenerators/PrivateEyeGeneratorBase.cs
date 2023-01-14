using System.Collections.Immutable;
using System.Text;
using Irrelephant.Search.PrivateEye.Core;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Irrelephant.Search.PrivateEye.SourceGenerators;

public abstract class PrivateEyeGeneratorBase : IIncrementalGenerator
{
    private string? ExtractName(AttributeSyntax attribute) =>
        attribute.Name switch
        {
            SimpleNameSyntax simple => simple.Identifier.Text,
            QualifiedNameSyntax qualified => qualified.Right.Identifier.Text,
            _ => null
        };

    private bool IsSearchDocument(SyntaxNode node)
    {
        if (node is not AttributeSyntax attribute)
            return false;

        return ExtractName(attribute) is nameof(PrivateEyeAttribute) or "PrivateEye";
    }

    private ITypeSymbol? GetSearchDocumentType(GeneratorSyntaxContext context)
    {
        var attribute = (AttributeSyntax)context.Node;

        var attributeTarget = attribute.Parent?.Parent;
        if (attributeTarget is RecordDeclarationSyntax or ClassDeclarationSyntax)
        {
            return context.SemanticModel.GetDeclaredSymbol(attributeTarget) as ITypeSymbol;
        }

        return null;
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var generationTarget = context.SyntaxProvider
            .CreateSyntaxProvider(
                (node, _) => IsSearchDocument(node),
                (syntaxContext, _) => GetSearchDocumentType(syntaxContext)
            )
            .Where(documentType => documentType is not null)
            .Collect();

        context.RegisterSourceOutput(generationTarget, GenerateSources);
    }

    private void GenerateSources(SourceProductionContext context, ImmutableArray<ITypeSymbol?> targets)
    {
        if (targets.IsDefaultOrEmpty)
            return;

        foreach (var type in targets)
        {
            if (type is null)
                return;

            var builder = new StringBuilder();
            GenerateComponent(builder, type);
            var typeNamespace = GetComponentNamespace(type) ?? "global";
            context.AddSource($"{typeNamespace}.{type.Name}-{GetComponentName()}.g.cs", builder.ToString());
        }
    }

    protected abstract void GenerateComponent(StringBuilder builder, ITypeSymbol indexDocumentType);

    protected abstract string GetComponentName();

    protected string? GetComponentNamespace(ITypeSymbol symbol) =>
        symbol.ContainingNamespace.IsGlobalNamespace
            ? null
            : symbol.ContainingNamespace.ToDisplayString();
}
