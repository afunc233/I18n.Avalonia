using I18n.Avalonia.Generator.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator;

/// <summary>
/// 
/// </summary>
[Generator(LanguageNames.CSharp)]
internal class ResxI18nGenerator : AttributeDetectBaseGenerator
{
    #region 固定量

    private static readonly string Attribute = $"{Const.ResxKeysOfAttributeFullName}";

    private static readonly string[] Exceptions =
    [
        "ResourceManager",
        "Culture"
    ];


    private static readonly string ResxKeysOfAttributeSource =
        $"""
         using System;

         namespace {Const.AttributeNamespace};
         #pragma warning disable CS9113
         [AttributeUsage(AttributeTargets.Class, Inherited = false)]
         public class {Const.ResxI18nOfAttribute}(Type resourceType) : Attribute;
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
            private static readonly I18n.Avalonia.ITranslatorProvider _translator = new $TranslatorProviderName$();
            
            #nullable enable
            class $TranslatorProviderName$ : I18n.Avalonia.ITranslatorProvider
            {
                private readonly Dictionary<string, Func<string?>> _translationProviders = new();
                
                internal $TranslatorProviderName$()
                {
                    I18n.Avalonia.I18nProvider.Instance.Add(this);
                }
                
                void ITranslatorProvider.AddOrUpdate(string key, Func<string?> value)
                {
                    _translationProviders[key] = value;
                }
                
                void ITranslatorProvider.SetCulture(CultureInfo culture)
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
            }
        $I18nUnit$
        }

        """;

    #endregion

    protected override string AttributeName => Attribute;

    protected override void OnInitialize(IncrementalGeneratorInitializationContext context)
    {
        base.OnInitialize(context);

        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{Const.ResxI18nOfAttribute}.g.cs", ResxKeysOfAttributeSource);
        });
    }

    protected override void GenerateCode(SourceProductionContext context,
        AttributeContextAndArguments contextAndArguments)
    {
        var generateCtx = contextAndArguments.Context;
        if (contextAndArguments.ArgumentSyntax?.FirstOrDefault()?.Expression is not TypeOfExpressionSyntax typeOfExpression)
        {
            return;
        }

        var targetSymbol = generateCtx.SemanticModel.GetSymbolInfo(typeOfExpression.Type).Symbol as INamedTypeSymbol;
        var targetFullName = targetSymbol!.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var nameSpace =
            generateCtx.TargetSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "");

        var memberNames = targetSymbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Select(x => x.Name)
            .Except(Exceptions).ToList();

        var translatorProviderName = $"{targetSymbol.Name}TranslatorProvider";

        var addOrUpdate = string.Join("\n",
            memberNames.Select(x => $"\t\t_translator.AddOrUpdate(\"{x}\",() => {targetFullName}.{x});"));

        // ReSharper disable once InconsistentNaming
        var i18nUnit = string.Join("\n",
            memberNames.Select(x =>
                $"""

                     /// <summary>
                     /// find string like {x}
                     /// </summary>
                     public static {Const.RootNamespace}.I18nUnit {x} => new {Const.RootNamespace}.I18nUnit(_translator, nameof({x}));
                 """));
        context.AddSource(
            $"{generateCtx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")}.g.cs",
            Format.Replace("$TranslatorProviderName$", translatorProviderName)
                .Replace("$ResxTypeName$", targetFullName)
                .Replace("$NameSpace$", nameSpace)
                .Replace("$ClassName$", generateCtx.TargetSymbol.Name)
                .Replace("$AddOrUpdate$", addOrUpdate)
                .Replace("$I18nUnit$", i18nUnit)
        );
    }
}
