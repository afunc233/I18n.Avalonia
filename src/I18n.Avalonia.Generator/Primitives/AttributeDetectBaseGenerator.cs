using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator.Primitives;

internal abstract class AttributeDetectBaseGenerator : IIncrementalGenerator
{
    protected abstract string AttributeName { get; }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        OnInitialize(context);
        var info = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeName, IsPartialClass, Transform).Where(it => it.ArgumentSyntax is not null);

        context.RegisterSourceOutput(info, GenerateCode);
    }

    protected virtual void OnInitialize(IncrementalGeneratorInitializationContext context)
    {
    }

    protected virtual bool IsPartialClass(SyntaxNode node, CancellationToken token)
    {
        return node is ClassDeclarationSyntax { Parent: not ClassDeclarationSyntax } classDeclarationSyntax
               && classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)
               && classDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword)
               && classDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    protected virtual AttributeContextAndArguments Transform(GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {
        if (context.TargetNode is not ClassDeclarationSyntax node)
        {
            return new AttributeContextAndArguments(context, null);
        }

        var attribute = node.GetSpecifiedAttribute(context.SemanticModel, AttributeName, token);

        return attribute is null ?
            new AttributeContextAndArguments(context, null) :
            new AttributeContextAndArguments(context, attribute.ArgumentList?.Arguments);
    }

    protected abstract void GenerateCode(SourceProductionContext context,
        AttributeContextAndArguments attributeContextAndArguments);
}
