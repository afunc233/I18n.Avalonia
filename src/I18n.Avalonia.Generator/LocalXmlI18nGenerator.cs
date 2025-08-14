using I18n.Avalonia.Generator.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator;

[Generator(LanguageNames.CSharp)]
internal class LocalXmlI18nGenerator : AbsAttributeDetectGenerator
{
    private static readonly string XmlKeysOfAttributeSource =
        $"""
         using System;
         using System.Collections.Generic;
         using System.Diagnostics.CodeAnalysis;
         using System.Xml;
         
         namespace {Const.AttributeNamespace};
         #pragma warning disable CS9113
         [AttributeUsage(AttributeTargets.Class, Inherited = false)]
         public sealed class {Const.LocalXmlI18nOfAttribute}([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type parseType,string methodName,string forderPath) : Attribute;
         #pragma warning restore CS9113
         """;

    private static readonly string Format =
        """
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using System.IO;
        using System.Linq;
        using System.Xml;
        using I18n.Avalonia;

        namespace $NameSpace$;

        partial class $ClassName$
        {
            private static readonly $TranslatorProviderName$ _translator = new $TranslatorProviderName$();
            
            #nullable enable
            class $TranslatorProviderName$ : I18n.Avalonia.ITranslatorProvider
            {
                private readonly Dictionary<CultureInfo, Dictionary<string, string>> _cultureDic = [];

                private readonly Dictionary<string, string> _defaultTranslator = [];
                
                internal $TranslatorProviderName$()
                {
                    I18n.Avalonia.I18nProvider.Add(this);
                    string[] paths = [$Paths$];
                    var processPath = AppContext.BaseDirectory;
                    var xmlPath = Path.Combine([processPath, .. paths]);
                
                    var files = Directory.EnumerateFiles(xmlPath,"*.xml",SearchOption.TopDirectoryOnly);
                    LoadFiles(files);
                }
                
               
                private void LoadFiles(IEnumerable<string> files)
                {
                    foreach (var file in files)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(file);

                            CultureInfo culture = CultureInfo.GetCultureInfo(fileName);

                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(file);
                            var root = xmlDoc.DocumentElement;
                            if (root is not null)
                            {
                                var dic = new Dictionary<string, string>();
                                foreach (XmlNode node in root.ChildNodes)
                                {
                                    dic.Add(node.Name, node.InnerText);
                                }
                                _cultureDic.TryAdd(culture, dic);
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }

                    if (_cultureDic.Values.FirstOrDefault() is { } cultrueDic)
                    {
                        _defaultTranslator.Clear();
                        foreach (var keyValue in cultrueDic)
                        {
                            _defaultTranslator.Add(keyValue.Key, keyValue.Value);
                        }
                    }
                }
                
                private CultureInfo _culture = CultureInfo.CurrentUICulture;

                void ITranslatorProvider.SetCulture(CultureInfo culture)
                {
                    _culture = culture;
                }
                
                string? ITranslatorProvider.GetString(string key)
                {
                    var value = string.Empty;
                    if (_cultureDic.TryGetValue(_culture, out var translator) && translator.TryGetValue(key, out var translatorValue))
                    {
                        value = translatorValue;
                    }
                    if (value is null && _defaultTranslator.TryGetValue(key, out var defauleValue))
                    {
                        value = defauleValue;
                    }
                    return value;
                }
            }
            #nullable disable
            
            static $ClassName$()
            {
            }
        $I18nUnit$
        }

        """;

    public override string AttributeName => Const.LocalXmlKeysOfAttributeFullName;

    public override void Initialize(IncrementalGeneratorInitializationContext context)
    {
        base.Initialize(context);

        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{Const.LocalXmlI18nOfAttribute}.g.cs", XmlKeysOfAttributeSource);
        });
    }


    private DiagnosticDescriptor DiagnosticDescriptorick(string id,
            string title,
            string messageFormat,
            string category,
            DiagnosticSeverity defaultSeverity,
            bool isEnabledByDefault)
    {
        return new DiagnosticDescriptor(id, title, messageFormat, category, defaultSeverity, isEnabledByDefault);
    }

    protected override void GenerateCode(SourceProductionContext context, AttributeContextAndArgumentSyntax attributeContextAndArgumentSyntax)
    {
        if (attributeContextAndArgumentSyntax.ArgumentSyntax is null)
        {
            return;
        }
//
//         if (attributeContextAndArgumentSyntax.ArgumentSyntax.Value.FirstOrDefault()?.Expression is not TypeOfExpressionSyntax typeOfExpression)
//         {
//             // 记录异常信息到生成器日志
//             context.ReportDiagnostic(
//                 Diagnostic.Create(
//                     DiagnosticDescriptorick
//                     ("XmlI18n0005",
//                         "SourceGenerator Error",
//                         "An error occurred in XmlI18nGenerator: {0}",
//                         "SourceGeneration",
//                         DiagnosticSeverity.Error,
//                         true),
//                     attributeContextAndArgumentSyntax.Context.TargetNode.GetLocation(),
//                     "XmlI18nOfAttribute 未设置 keys "));
//             return;
//         }
//         
//         if (attributeContextAndArgumentSyntax.ArgumentSyntax.Value.Skip(1).FirstOrDefault()?.Expression is not CollectionExpressionSyntax { Elements.Count: > 0 } keysExpression)
//         {
//             // 记录异常信息到生成器日志
//             context.ReportDiagnostic(
//                 Diagnostic.Create(
//                     DiagnosticDescriptorick
//                     ("XmlI18n0003",
//                     "SourceGenerator Error",
//                     "An error occurred in XmlI18nGenerator: {0}",
//                     "SourceGeneration",
//                     DiagnosticSeverity.Error,
//                     true),
//                     attributeContextAndArgumentSyntax.Context.TargetNode.GetLocation(),
//                     "XmlI18nOfAttribute 未设置 keys "));
//             return;
//         }
//
//         var keys = keysExpression.Elements.Cast<ExpressionElementSyntax>().Select(it => it.Expression).Cast<LiteralExpressionSyntax>().Select(it => it.Token.Value!.ToString()).ToList();
//
//         foreach (var key in keys)
//         {
//             if (!IdentifierValidator.IsValidIdentifier(key))
//             {
//                 // 记录异常信息到生成器日志
//                 context.ReportDiagnostic(
//                     Diagnostic.Create(
//                         DiagnosticDescriptorick
//                         ("XmlI18n0004",
//                         "SourceGenerator Error",
//                         "An error occurred in XmlI18nGenerator: {0}",
//                         "SourceGeneration",
//                         DiagnosticSeverity.Error,
//                         true),
//                         attributeContextAndArgumentSyntax.Context.TargetNode.GetLocation(),
//                         $"XmlI18nOfAttribute keys {key} 不合法 "));
//             }
//         }
//
//         
//         if (attributeContextAndArgumentSyntax.ArgumentSyntax.Value.LastOrDefault()?.Expression is not CollectionExpressionSyntax { Elements.Count: > 0 } pathsExpression)
//         {
//             // 记录异常信息到生成器日志
//             context.ReportDiagnostic(
//                 Diagnostic.Create(
//                     DiagnosticDescriptorick
//                     ("XmlI18n0001",
//                         "SourceGenerator Error",
//                         "An error occurred in XmlI18nGenerator: {0}",
//                         "SourceGeneration",
//                         DiagnosticSeverity.Error,
//                         true),
//                     attributeContextAndArgumentSyntax.Context.TargetNode.GetLocation(),
//                     "XmlI18nOfAttribute 未设置 paths "));
//             return;
//         }
//         var paths = pathsExpression.Elements.Cast<ExpressionElementSyntax>().Select(it => it.Expression).Cast<LiteralExpressionSyntax>().Select(it => it.Token.Value!.ToString()).ToList();
//
//         foreach (var path in paths)
//         {
//             if (!IdentifierValidator.IsValidIdentifier(path))
//             {
//                 // 记录异常信息到生成器日志
//                 context.ReportDiagnostic(
//                     Diagnostic.Create(
//                         DiagnosticDescriptorick
//                         ("XmlI18n0002",
//                             "SourceGenerator Error",
//                             "An error occurred in XmlI18nGenerator: {0}",
//                             "SourceGeneration",
//                             DiagnosticSeverity.Error,
//                             true),
//                         attributeContextAndArgumentSyntax.Context.TargetNode.GetLocation(),
//                         $"XmlI18nOfAttribute paths {path} 不合法 "));
//             }
//         }
//
//         var generateCtx = attributeContextAndArgumentSyntax.Context;
//
//         var nameSpace =
//             generateCtx.TargetSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
//                 .Replace("global::", "");
//         
//         var translatorProviderName = $"{generateCtx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).Replace("global::", "")}TranslatorProvider";
//
//         // ReSharper disable once InconsistentNaming
//         var i18nUnit = string.Join("\n",
//             keys.Select(x =>
//          $"""
//              /// <summary>
//              /// find string like {x}
//              /// </summary>
//              public static {Const.RootNamespace}.I18nUnit {x} => new {Const.RootNamespace}.I18nUnit(_translator, nameof({x}));
//          """));
//
//
//         var pathList = string.Join(",",paths.Select(it=> $"\"{it}\""));
//
//         context.AddSource(
//             $"{generateCtx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")}.g.cs",
//             Format.Replace("$TranslatorProviderName$", translatorProviderName)
//                 .Replace("$NameSpace$", nameSpace)
//                 .Replace("$ClassName$", generateCtx.TargetSymbol.Name)
//                 .Replace("$I18nUnit$", i18nUnit)
//                 .Replace("$Paths$", pathList)
//         );

        var s = Format;
    }
}

