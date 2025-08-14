using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator.Primitives;

internal class AttributeContextAndArgumentSyntax(GeneratorAttributeSyntaxContext context, SeparatedSyntaxList<AttributeArgumentSyntax>? attributeArguments)
{
    public GeneratorAttributeSyntaxContext Context { get; } = context;

    public SeparatedSyntaxList<AttributeArgumentSyntax>? ArgumentSyntax { get; } = attributeArguments;
}
