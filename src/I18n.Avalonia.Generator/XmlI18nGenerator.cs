using System.Reflection;
using I18n.Avalonia.Generator.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator;

// [Generator(LanguageNames.CSharp)]
internal class XmlI18nGenerator : AttributeDetectBaseGenerator
{
    private static readonly string XmlKeysOfAttributeSource =
        $"""
         using System;

         namespace {Const.AttributeNamespace};
         #pragma warning disable CS9113
         [AttributeUsage(AttributeTargets.Class, Inherited = false)]
         public class {Const.XmlI18nOfAttribute}(string path, string[] keys) : Attribute;
         #pragma warning restore CS9113
         """;

    private static readonly string Format =
        """
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using I18n.Avalonia;

        namespace $NameSpace$;

        partial class $ClassName$
        {
            private static readonly I18n.Avalonia.ITranslatorProvider _translator = new I18n.Avalonia.Generator.XmlTranslatorProvider($path$,I18nProvider.Instance.Culture);
            
        $I18nUnit$
        }

        """;

    
    protected override string AttributeName => Const.XmlKeysOfAttributeFullName;

    protected override void OnInitialize(IncrementalGeneratorInitializationContext context)
    {
        base.OnInitialize(context);

        const string sourceName = "I18n.Avalonia.Generator.Sources.XmlTranslatorProvider.cs";
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sourceName);

        if (stream is null)
        {
            return;
        }

        using var reader = new StreamReader(stream);

        var source = reader.ReadToEnd();
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{Const.XmlI18nOfAttribute}.g.cs", XmlKeysOfAttributeSource);
            ctx.AddSource(sourceName, source);
        });
    }


    private static DiagnosticDescriptor DiagnosticDescriptor(string id,
        string title,
        string messageFormat,
        string category,
        DiagnosticSeverity defaultSeverity,
        bool isEnabledByDefault)
    {
        return new DiagnosticDescriptor(id, title, messageFormat, category, defaultSeverity, isEnabledByDefault);
    }

    protected override void GenerateCode(SourceProductionContext context,
        AttributeContextAndArguments attributeContextAndArgumentSyntax)
    {
        var generateCtx = attributeContextAndArgumentSyntax.Context;
        
        if (attributeContextAndArgumentSyntax.ArgumentSyntax is null)
        {
            return;
        }

        var literalExpression =
            attributeContextAndArgumentSyntax.ArgumentSyntax.Value.FirstOrDefault()?.Expression as
                LiteralExpressionSyntax;

        if (literalExpression is not { Token.Value: not null })
        {
            // 记录异常信息到生成器日志
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0001",
                        "SourceGenerator Error",
                        "An error occurred in XmlI18nGenerator: {0}",
                        "SourceGeneration",
                        DiagnosticSeverity.Error,
                        true),
                    attributeContextAndArgumentSyntax.Context.TargetNode.GetLocation(),
                    "XmlI18nOfAttribute path 不能为 null "));
            return;
        }
        var nameSpace =
            generateCtx.TargetSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "");
        var path = literalExpression.Token.ValueText;

        var collectionExpression =
            attributeContextAndArgumentSyntax.ArgumentSyntax.Value.Skip(1).FirstOrDefault()?.Expression as
                CollectionExpressionSyntax;

        if (collectionExpression is not { Elements.Count: > 0 })
        {
            // 记录异常信息到生成器日志
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0002",
                        "SourceGenerator Error",
                        "An error occurred in XmlI18nGenerator: {0}",
                        "SourceGeneration",
                        DiagnosticSeverity.Error,
                        true),
                    attributeContextAndArgumentSyntax.Context.TargetNode.GetLocation(),
                    "XmlI18nOfAttribute 未设置 keys "));
            return;
        }

        var keys = collectionExpression.Elements.Cast<ExpressionElementSyntax>().Select(it => it.Expression)
            .Cast<LiteralExpressionSyntax>().Select(it => it.Token.Value!.ToString());

        foreach (var key in keys)
        {
            if (!IdentifierValidator.IsValidIdentifier(key))
            {
                // 记录异常信息到生成器日志
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        DiagnosticDescriptor
                        ("XmlI18n0003",
                            "SourceGenerator Error",
                            "An error occurred in XmlI18nGenerator: {0}",
                            "SourceGeneration",
                            DiagnosticSeverity.Error,
                            true),
                        attributeContextAndArgumentSyntax.Context.TargetNode.GetLocation(),
                        $"XmlI18nOfAttribute keys {key} 不合法 "));
            }
        }

        //var addOrUpdate = string.Join("\n",    keys.Select(x => $"\t\t_translator.AddOrUpdate(\"{x}\",() => {targetFullName}.{x});"));

        // ReSharper disable once InconsistentNaming
        var i18nUnit = string.Join("\n",
            keys.Select(x =>
                $"""
                     /// <summary>
                     /// find string like {x}
                     /// </summary>
                     public static {Const.RootNamespace}.I18nUnit {x} => new {Const.RootNamespace}.I18nUnit(_translator, nameof({x}));
                 """));

        context.AddSource(
            $"{generateCtx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")}.g.cs",
            Format
                .Replace("$NameSpace$", nameSpace)
                .Replace("$ClassName$", generateCtx.TargetSymbol.Name)
                .Replace("$path$", $"\"{path}\"")
                .Replace("$I18nUnit$", i18nUnit));
    }
}

public static class IdentifierValidator
{
    /// <summary>
    /// 检查标识符是否合法。
    /// 可选：进一步检查 Unicode 标识符扩展规则（如允许 Unicode 字符）
    /// 参考：https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/char
    /// </summary>
    public static bool IsValidIdentifier(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return false;

        // 1. 检查是否是 C# 关键字
        if (SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None)
            return false;

        // 2. 尝试解析标识符，如果能成功解析则合法
        try
        {
            return SyntaxFacts.IsValidIdentifier(identifier);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
