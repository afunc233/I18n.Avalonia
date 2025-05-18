using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator;

internal abstract class AttributeDetectBaseGenerator : IIncrementalGenerator
{
    protected class AttributeContextAndType(GeneratorAttributeSyntaxContext context, TypeSyntax? typeSyntax)
    {
        public GeneratorAttributeSyntaxContext Context { get; } = context;
        public TypeSyntax? TypeSyntax { get; } = typeSyntax;
    }

    protected abstract string AttributeName { get; }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        OnInitialize(context);
        var info = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeName, IsPartialClass, Transform).Where(it => it.TypeSyntax is not null);

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

    protected virtual AttributeContextAndType Transform(GeneratorAttributeSyntaxContext context,
        CancellationToken token)
    {
        var node = context.TargetNode as ClassDeclarationSyntax;

        if (node is null)
        {
            return new AttributeContextAndType(context, null);
        }

        var attribute = node.GetSpecifiedAttribute(context.SemanticModel, AttributeName, token);
        if (attribute is null)
        {
            return new AttributeContextAndType(context, null);
        }

        var argumentSyntax = attribute.ArgumentList?.Arguments.FirstOrDefault();
        return argumentSyntax is not { Expression: TypeOfExpressionSyntax typeOfExp }
            ? new AttributeContextAndType(context, null)
            : new AttributeContextAndType(context, typeOfExp.Type);
    }

    protected abstract void GenerateCode(SourceProductionContext context,
        AttributeContextAndType attributeContextAndType);
}
