using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator.Primitives;

internal static class SyntaxNodeExtension
{
    /// <summary>
    /// 从 SyntaxNode 获取 一个 AttributeSyntax
    /// </summary>
    /// <param name="node"></param>
    /// <param name="semanticModel"></param>
    /// <param name="fullAttributeName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal static AttributeSyntax? GetSpecifiedAttribute(this SyntaxNode node, SemanticModel semanticModel,
        string fullAttributeName,
        CancellationToken cancellationToken = default)
    {
        return GetSpecifiedAttributes(node, semanticModel, fullAttributeName, cancellationToken).FirstOrDefault();
    }


    private static IEnumerable<AttributeSyntax> GetSpecifiedAttributes(this SyntaxNode node,
        SemanticModel semanticModel, string fullAttributeName,
        CancellationToken cancellationToken = default)
    {
        foreach (var attributeSyntax in GetAllAttributes(node))
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;
            if (semanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol
                attributeSymbol)
                continue;
            var attributeName = attributeSymbol.ContainingType.ToDisplayString();
            if (attributeName == fullAttributeName)
                yield return attributeSyntax;
        }
    }

    private static IEnumerable<AttributeSyntax> GetAllAttributes(this SyntaxNode node)
    {
        var attributeLists = node switch
        {
            CompilationUnitSyntax compilationUnitSyntax => compilationUnitSyntax.AttributeLists,
            MemberDeclarationSyntax memberDeclarationSyntax => memberDeclarationSyntax.AttributeLists,
            LambdaExpressionSyntax lambdaExpressionSyntax => lambdaExpressionSyntax.AttributeLists,
            BaseParameterSyntax baseParameterSyntax => baseParameterSyntax.AttributeLists,
            StatementSyntax statementSyntax => statementSyntax.AttributeLists,
            _ => throw new NotSupportedException($"{node.GetType()} has no attribute")
        };
        return attributeLists.SelectMany(attributeListSyntax => attributeListSyntax.Attributes);
    }
}
