using System.Globalization;
using I18n.Avalonia.Generator.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator;

/// <summary>
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class ResxI18nGenerator : AbsAttributeDetectGenerator
{
    public override string AttributeName
    {
        get => Attribute;
    }

    protected override void GenerateCode(SourceProductionContext context,
        AttributeContextAndArgumentSyntax contextAndArguments)
    {
        var generateCtx = contextAndArguments.Context;
        if (contextAndArguments.ArgumentSyntax?.FirstOrDefault()?.Expression is not TypeOfExpressionSyntax
            typeOfExpression)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    DiagnosticDescriptor
                    ("ResxI18n0001",
                        DiagnosticSeverity.Error,
                        true),
                    contextAndArguments.ArgumentSyntax?.FirstOrDefault()?.Expression.GetLocation(),
                    $"{AttributeName} type should not null "));
            return;
        }

        var targetSymbol = generateCtx.SemanticModel.GetSymbolInfo(typeOfExpression.Type).Symbol as INamedTypeSymbol;
        var targetFullName = targetSymbol!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", "");

        var nameSpace =
            generateCtx.TargetSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "");

        var propertySymbols = targetSymbol.GetMembers().OfType<IPropertySymbol>().ToList();

        if (Exceptions.Any(it => propertySymbols.All(p => string.Equals(p.Name, it))))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(DiagnosticDescriptor("ResxI18n0002", DiagnosticSeverity.Error, true),
                    contextAndArguments.ArgumentSyntax?.FirstOrDefault()?.Expression.GetLocation(),
                    $"{targetFullName} is incorrect type. should has properties [{string.Join(", ", Exceptions)}]"));
            return;
        }

        var culturePropertySymbol = propertySymbols.First(it => string.Equals(it.Name, Exceptions.Last()));

        if (!culturePropertySymbol.IsStatic)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(DiagnosticDescriptor("ResxI18n0003", DiagnosticSeverity.Error, true),
                    culturePropertySymbol.Locations.LastOrDefault(),
                    $"{culturePropertySymbol.Name} should be static "));
            return;
        }

        if (!string.Equals(culturePropertySymbol.Type.Name, nameof(CultureInfo)))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(DiagnosticDescriptor("ResxI18n0004", DiagnosticSeverity.Error, true),
                    culturePropertySymbol.Locations.LastOrDefault(),
                    $"type of {culturePropertySymbol.Name} should be {nameof(CultureInfo)}"));
            return;
        }

        var memberNames = propertySymbols.Select(it => it.Name).Except(Exceptions).ToList();

        var translatorProviderName = $"{targetSymbol.Name}TranslatorProvider";

        var addOrUpdate = string.Join("\n",
            memberNames.Select(x => $"\t\t_translator.AddOrUpdate(\"{x}\",() => {targetFullName}.{x});"));

        var i18nAddOrUpdate = string.Join("\n",
            memberNames.Select(x => $"\t\t_translator.I18nUnits.Add({x});"));

        // ReSharper disable once InconsistentNaming
        var i18nUnit = string.Join("\n",
            memberNames.Select(x =>
                $"""

                     private static readonly I18n.Avalonia.I18nUnit _{x} = new I18n.Avalonia.I18nUnit(_translator, nameof({x}));
                     /// <summary>
                     /// find string like {x}
                     /// </summary>
                     public static {Const.RootNamespace}.I18nUnit {x} => _{x};
                 """));
        context.AddSource(
            $"{generateCtx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")}.g.cs",
            Format.Replace("$TranslatorProviderName$", translatorProviderName)
                .Replace("$ResxTypeName$", targetFullName)
                .Replace("$NameSpace$", nameSpace)
                .Replace("$ClassName$", generateCtx.TargetSymbol.Name)
                .Replace("$AddOrUpdate$", addOrUpdate)
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

    #region 固定量

    private static readonly string Attribute = $"{Const.ResxKeysOfAttributeFullName}";

    private static readonly string[] Exceptions =
    [
        "ResourceManager",
        "Culture"
    ];

    private static readonly string Format =
        """
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using I18n.Avalonia;
        using I18n.Avalonia.TranslatorProviders;

        namespace $NameSpace$;

        partial class $ClassName$
        {
            private static readonly $TranslatorProviderName$ _translator = new $TranslatorProviderName$();
            
            #nullable enable
            class $TranslatorProviderName$ : I18n.Avalonia.ITranslatorProvider
            {
                private readonly Dictionary<string, Func<string?>> _translationProviders = new();
                
                public ICollection<I18nUnit> I18nUnits { get; } = [];
                
                internal $TranslatorProviderName$()
                {
                    I18n.Avalonia.I18nProvider.Add(this);
                }
                
                public void AddOrUpdate(string key, Func<string?> value)
                {
                    _translationProviders[key] = value;
                }
                
                public void SetCulture(CultureInfo culture)
                {
                    $ResxTypeName$.Culture = culture;
                }
                
                string? ITranslatorProvider.GetString(string key)
                {
                    if (_translationProviders.TryGetValue(key, out var valueFunc))
                    {
                        return valueFunc.Invoke();
                    }
                    return string.Empty;
                }
            }
            #nullable disable
            
            static $ClassName$()
            {
        $AddOrUpdate$

        $I18nAddOrUpdate$

                _translator.Refresh();
            }
        $I18nUnit$
        }

        """;

    #endregion
}
