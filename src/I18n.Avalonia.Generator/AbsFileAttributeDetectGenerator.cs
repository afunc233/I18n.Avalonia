using I18n.Avalonia.Generator.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator;

internal abstract class AbsFileAttributeDetectGenerator : AbsAttributeDetectGenerator
{
    private static readonly string Format =
        """
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using System.IO;
        using System.Linq;
        using System.Xml;
        using I18n.Avalonia;
        using I18n.Avalonia.TranslatorProviders;

        namespace $NameSpace$;

        partial class $ClassName$
        {
            static $ClassName$()
            {
        $I18nAddOrUpdate$

                I18n.Avalonia.I18nProvider.Add($TranslatorProviderName$);
                
                $TranslatorProviderName$.FillValues();
                
                $TranslatorProviderName$.Refresh();
            }

        $I18nUnit$
        }

        """;


    protected override void GenerateCode(SourceProductionContext context,
        AttributeContextAndArgumentSyntax attributeContextAndArgumentSyntax)
    {
        var generateCtx = attributeContextAndArgumentSyntax.Context;

        if (generateCtx.TargetNode is not ClassDeclarationSyntax classDeclaration)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0001",
                        DiagnosticSeverity.Error,
                        true),
                    generateCtx.TargetNode.GetLocation(),
                    "generateCtx.TargetNode is not a ClassDeclarationSyntax"));
            return;
        }

        var keyMember = classDeclaration.Members.FirstOrDefault(it =>
        {
            return it.AttributeLists.SelectMany(attr => attr.Attributes).Any(attr =>
                string.Equals(attr.Name.ToFullString(), Const.I18nKeysAttribute) ||
                string.Equals(attr.Name.ToFullString(), Const.I18nKeysAttribute.Replace("Attribute", "")));
        });

        if (keyMember is not FieldDeclarationSyntax keysDeclarationSyntax)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0002",
                        DiagnosticSeverity.Error,
                        true),
                    classDeclaration.GetLocation(),
                    "keys field not find"));
            return;
        }

        if (!keysDeclarationSyntax.IsPrivateStaticReadonlyField())
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0003",
                        DiagnosticSeverity.Error,
                        true),
                    keysDeclarationSyntax.GetLocation(),
                    "keys should be private static readonly"));
            return;
        }

        if (keysDeclarationSyntax.Declaration.Type is not ArrayTypeSyntax
            {
                ElementType: PredefinedTypeSyntax predefinedType
            }
            || !string.Equals(predefinedType.Keyword.Text, nameof(String), StringComparison.OrdinalIgnoreCase))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0004",
                        DiagnosticSeverity.Error,
                        true),
                    keysDeclarationSyntax.GetLocation(),
                    "keys should be private static readonly string[]"));
            return;
        }

        var keyInitializerValues = keysDeclarationSyntax.Declaration.Variables.FirstOrDefault()?.Initializer?.Value;

        if (keyInitializerValues is not CollectionExpressionSyntax collectionExpression)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0005",
                        DiagnosticSeverity.Error,
                        true),
                    keysDeclarationSyntax.GetLocation(),
                    "keys should be private static readonly string[]"));
            return;
        }

        var keyElements = collectionExpression.Elements.OfType<ExpressionElementSyntax>().ToList();
        var keys = new List<string>();

        foreach (var keyElement in keyElements)
        {
            var key = keyElement.ToString().Replace("\"", "");

            if (IdentifierValidator.IsValidIdentifier(key))
            {
                keys.Add(key);
                continue;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0006",
                        DiagnosticSeverity.Error,
                        true),
                    keyElement.GetLocation(),
                    $"{key} is not a valid identifier"));
            return;
        }

        var providerMember = classDeclaration.Members.FirstOrDefault(it =>
        {
            return it.AttributeLists.SelectMany(attr => attr.Attributes).Any(attr =>
                {
                    if (string.Equals(attr.Name.ToFullString(), Const.I18nProviderAttribute)) return true;

                    return string.Equals(attr.Name.ToFullString(),
                        Const.I18nProviderAttribute.Replace("Attribute", ""));
                }
            );
        });

        if (providerMember is not FieldDeclarationSyntax providerDeclarationSyntax)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0007",
                        DiagnosticSeverity.Error,
                        true),
                    classDeclaration.GetLocation(),
                    "provider field not find"));
            return;
        }

        if (!providerDeclarationSyntax.IsPrivateStaticReadonlyField())
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0008",
                        DiagnosticSeverity.Error,
                        true),
                    classDeclaration.GetLocation(),
                    "provider should be private static readonly"));
            return;
        }

        var providerType = providerDeclarationSyntax.Declaration.Type;

        if (providerType is not IdentifierNameSyntax identifierNameSyntax)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n0009",
                        DiagnosticSeverity.Error,
                        true),
                    classDeclaration.GetLocation(),
                    "translatorProviderName not find"));
            return;
        }

        if (!string.Equals(identifierNameSyntax.Identifier.Text, Const.IFileTranslatorProvider))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n00010",
                        DiagnosticSeverity.Error,
                        true),
                    identifierNameSyntax.Identifier.GetLocation(),
                    $"translatorProvider must be {Const.IFileTranslatorProvider}"));

            return;
        }

        var translatorProviderName = providerDeclarationSyntax.Declaration.Variables.FirstOrDefault()?.Identifier.Text;

        if (translatorProviderName is null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("XmlI18n00011",
                        DiagnosticSeverity.Error,
                        true),
                    classDeclaration.GetLocation(),
                    "translatorProviderName not find"));
            return;
        }

        var nameSpace =
            generateCtx.TargetSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "");

        // ReSharper disable once InconsistentNaming
        var i18nUnit = string.Join("\n",
            keys.Select(x =>
                $"""

                     private static {Const.RootNamespace}.I18nUnit _{x} = new {Const.RootNamespace}.I18nUnit({translatorProviderName}, nameof({x}));
                     
                     /// <summary>
                     /// find string like {x}
                     /// </summary>
                     public static {Const.RootNamespace}.I18nUnit {x} => _{x};
                 """));

        var i18nAddOrUpdate = string.Join("\n",
            keys.Select(x => $"\t\t{translatorProviderName}.I18nUnits.Add({x});"));

        context.AddSource(
            $"{generateCtx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")}.g.cs",
            Format.Replace("$TranslatorProviderName$", translatorProviderName)
                .Replace("$NameSpace$", nameSpace)
                .Replace("$ClassName$", generateCtx.TargetSymbol.Name)
                .Replace("$I18nAddOrUpdate$", i18nAddOrUpdate)
                .Replace("$I18nUnit$", i18nUnit)
        );
    }

    private static DiagnosticDescriptor DiagnosticDescriptor(string id, DiagnosticSeverity defaultSeverity,
        bool isEnabledByDefault)
    {
        return new DiagnosticDescriptor(id,
            "SourceGenerator Error",
            $"An error occurred in {nameof(ResxI18nGenerator)}: {{0}}",
            "SourceGeneration",
            defaultSeverity,
            isEnabledByDefault);
    }
}
