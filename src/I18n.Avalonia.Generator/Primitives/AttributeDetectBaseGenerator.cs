using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator.Primitives;

public interface IAttributeDetectGenerator : IIncrementalGenerator
{
    string AttributeName { get; }
}

public interface IAttributeDetectGenerator<T> : IAttributeDetectGenerator
{
    T Transform(GeneratorAttributeSyntaxContext context,
        CancellationToken token);
}

public interface IAttributeDetectGenerator<T, OtherT> : IAttributeDetectGenerator<T>
{
}

internal static class GeneratorAttributeSyntaxContextExtensions
{
    internal static AttributeContextAndArgumentSyntax ToAttributeContextAndArgumentSyntax(
        this GeneratorAttributeSyntaxContext context, string attributeName, CancellationToken token)
    {
        if (context.TargetNode is not ClassDeclarationSyntax node)
            return new AttributeContextAndArgumentSyntax(context, null);

        var attribute = node.GetSpecifiedAttribute(context.SemanticModel, attributeName, token);

        return new AttributeContextAndArgumentSyntax(context, attribute?.ArgumentList?.Arguments);
    }
}

internal static class SyntaxNodeExtensions
{
    internal static bool IsPublicStaticPartialClass(this SyntaxNode node)
    {
        return node is ClassDeclarationSyntax { Parent: not ClassDeclarationSyntax } classDeclarationSyntax
               && classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword)
               && classDeclarationSyntax.Modifiers.Any(SyntaxKind.PublicKeyword)
               && classDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword);
    }

    internal static bool IsPrivateStaticReadonlyField(this SyntaxNode node)
    {
        return node is FieldDeclarationSyntax fieldDeclarationSyntax
               && fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.PrivateKeyword)
               && fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.StaticKeyword)
               && fieldDeclarationSyntax.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
    }
}

internal abstract class AbsAttributeDetectGenerator : IAttributeDetectGenerator<AttributeContextAndArgumentSyntax>
{
    public abstract string AttributeName { get; }

    public AttributeContextAndArgumentSyntax Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        return context.ToAttributeContextAndArgumentSyntax(AttributeName, token);
    }

    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var info = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeName, IsPartialClass, Transform);

        context.RegisterSourceOutput(info, GenerateCode);
    }

    protected virtual bool IsPartialClass(SyntaxNode node, CancellationToken token)
    {
        return node.IsPublicStaticPartialClass();
    }

    protected abstract void GenerateCode(SourceProductionContext context,
        AttributeContextAndArgumentSyntax attributeContextAndType);
}

internal abstract class AbsAttributeDetectGenerator<T> : IAttributeDetectGenerator<AttributeContextAndArgumentSyntax, T>
{
    public abstract string AttributeName { get; }

    public AttributeContextAndArgumentSyntax Transform(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        return context.ToAttributeContextAndArgumentSyntax(AttributeName, token);
    }

    public virtual void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeName, IsPartialClass, Transform);

        context.RegisterSourceOutput(ConvertProvider(context, provider), GenerateCode);
    }

    protected virtual bool IsPartialClass(SyntaxNode node, CancellationToken token)
    {
        return node.IsPublicStaticPartialClass();
    }

    internal abstract IncrementalValuesProvider<(AttributeContextAndArgumentSyntax, T)> ConvertProvider(
        IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<AttributeContextAndArgumentSyntax> provider);

    protected abstract void GenerateCode(SourceProductionContext context,
        (AttributeContextAndArgumentSyntax, T) attributeContextAndType);
}
