using System.Collections.Immutable;
using System.Reflection;
using System.Xml;
using I18n.Avalonia.Generator.Primitives;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace I18n.Avalonia.Generator;

[Generator(LanguageNames.CSharp)]
internal class EmbeddedResourceXmlI18nGenerator : AbsAttributeDetectGenerator<ImmutableArray<AdditionalText>>
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
         public sealed class {Const.EmbeddedResourceXmlI18nOfAttribute}([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] Type parseType,string methodName,string nameContains) : Attribute;
         #pragma warning restore CS9113
         """;

    private static readonly string EmbeddedResourceXmlI18nParserSource =
        """
        using System;
        using System.Collections.Generic;
        using System.Globalization;
        using System.Xml;

        public interface IEmbeddedResourceXmlI18nParser
        {
            string XmlContent { get;set; } 
            
            string XmlName { get;set; }
            
            CultureInfo Culture { get; }
            
            List<(string,string)> Parse();
        }
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
            private static readonly $TranslatorProviderName$ _translator = new $TranslatorProviderName$();
            
            #nullable enable
            class $TranslatorProviderName$ : I18n.Avalonia.ITranslatorProvider
            {
                private readonly Dictionary<string, Func<string?>> _translationProviders = new();
                
                public List<I18nUnit> I18nUnits { get; } = [];
                
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
                    
                    foreach(I18nUnit i18nUnit in I18nUnits)
                    {
                        i18nUnit.Refresh();
                    }
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

                _translator.SetCulture(I18n.Avalonia.I18nProvider.Culture);
            }
        $I18nUnit$
        }
        """;

    public override string AttributeName => Const.EmbeddedResourceXmlKeysOfAttributeFullName;

    public override void Initialize(IncrementalGeneratorInitializationContext context)
    {
        base.Initialize(context);

        
        var compilationProvider = context.CompilationProvider;
        
        context.RegisterSourceOutput(compilationProvider, (spc, source) =>
        {
            // source.
        });
        
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{Const.EmbeddedResourceXmlI18nOfAttribute}.g.cs", XmlKeysOfAttributeSource);
            ctx.AddSource($"{Const.IEmbeddedResourceXmlI18nParser}.g.cs", EmbeddedResourceXmlI18nParserSource);
        });
    }

    internal override IncrementalValuesProvider<(AttributeContextAndArgumentSyntax, ImmutableArray<AdditionalText>)>
        ConvertProvider(IncrementalGeneratorInitializationContext context,
            IncrementalValuesProvider<AttributeContextAndArgumentSyntax> provider)
    {
        return provider.Combine(context.AdditionalTextsProvider.Collect());
    }

    protected override void GenerateCode(SourceProductionContext context,
        (AttributeContextAndArgumentSyntax, ImmutableArray<AdditionalText>) args)
    {
        var attributeContextAndType = args.Item1;

        var additionalTextArray = args.Item2;

        if (attributeContextAndType.ArgumentSyntax is null)
        {
            return;
        }

        if (attributeContextAndType.ArgumentSyntax.Value.FirstOrDefault()?.Expression is not TypeOfExpressionSyntax
            typeOfExpression)
        {
            // 记录异常信息到生成器日志
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor
                    ("XmlI18n0005",
                        "SourceGenerator Error",
                        "An error occurred in XmlI18nGenerator: {0}",
                        "SourceGeneration",
                        DiagnosticSeverity.Error,
                        true),
                    attributeContextAndType.ArgumentSyntax.Value.FirstOrDefault()?.Expression.GetLocation() ??
                    attributeContextAndType.Context.TargetNode.GetLocation(),
                    "parseType 请使用 typeof "));
            return;
        }

        //InvocationExpressionSyntax InvocationExpression nameof(ParseKeyValue)
        if (attributeContextAndType.ArgumentSyntax.Value.Skip(1).FirstOrDefault()?.Expression is not
            InvocationExpressionSyntax invocationExpression)
        {
            // 记录异常信息到生成器日志
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor
                    ("XmlI18n0003",
                        "SourceGenerator Error",
                        "An error occurred in XmlI18nGenerator: {0}",
                        "SourceGeneration",
                        DiagnosticSeverity.Error,
                        true),
                    attributeContextAndType.ArgumentSyntax.Value.Skip(1).FirstOrDefault()?.Expression?.GetLocation() ??
                    attributeContextAndType.Context.TargetNode.GetLocation(),
                    "methodName 请使用 nameof "));
            return;
        }

        if (invocationExpression.ArgumentList.Arguments.FirstOrDefault() is not { } methodNameArgumentSyntax)
        {
            // 记录异常信息到生成器日志
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor
                    ("XmlI18n0003",
                        "SourceGenerator Error",
                        "An error occurred in XmlI18nGenerator: {0}",
                        "SourceGeneration",
                        DiagnosticSeverity.Error,
                        true),
                    attributeContextAndType.ArgumentSyntax.Value.Skip(1).FirstOrDefault()?.Expression?.GetLocation() ??
                    attributeContextAndType.Context.TargetNode.GetLocation(),
                    "methodName 请使用 nameof "));
            return;
        }

        var generateCtx = attributeContextAndType.Context;
        var type = typeOfExpression.Type;

        if (generateCtx.SemanticModel.GetSymbolInfo(type).Symbol is not INamedTypeSymbol typeSymbol)
        {
            // 记录异常信息到生成器日志
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor
                    ("XmlI18n0004",
                        "SourceGenerator Error",
                        "An error occurred in XmlI18nGenerator: {0}",
                        "SourceGeneration",
                        DiagnosticSeverity.Error,
                        true),
                    attributeContextAndType.ArgumentSyntax.Value.FirstOrDefault()?.Expression.GetLocation() ??
                    attributeContextAndType.Context.TargetNode.GetLocation(),
                    "parseType 请使用 typeof "));
            return;
        }

        var methodName = methodNameArgumentSyntax.ToString();

        var convertMethod = typeSymbol.GetMembers(methodName).OfType<IMethodSymbol>().SingleOrDefault();

        if (convertMethod is null)
        {
            // 记录异常信息到生成器日志
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor
                    ("XmlI18n0005",
                        "SourceGenerator Error",
                        "An error occurred in XmlI18nGenerator: {0}",
                        "SourceGeneration",
                        DiagnosticSeverity.Error,
                        true),
                    attributeContextAndType.ArgumentSyntax.Value.FirstOrDefault()?.Expression.GetLocation() ??
                    attributeContextAndType.Context.TargetNode.GetLocation(),
                    "获取method 失败"));
            return;
        }

        // 判断 convertMethod 的返回值 实现了 IEmbeddedResourceXmlI18nParser

        if (convertMethod.ReturnType is not INamedTypeSymbol returnType)
        {
            return;
        }


        if (!string.Equals(returnType.Name, Const.IEmbeddedResourceXmlI18nParser))
        {
            var any = returnType.AllInterfaces.Any(it => string.Equals(it.Name, Const.IEmbeddedResourceXmlI18nParser));

            if (!any)
            {
                return;
            }
        }

        if (attributeContextAndType.ArgumentSyntax.Value.Skip(2).FirstOrDefault()?.Expression is not
            LiteralExpressionSyntax nameContainsLiteralExpression)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    new DiagnosticDescriptor
                    ("XmlI18n0006",
                        "SourceGenerator Error",
                        "An error occurred in XmlI18nGenerator: {0}",
                        "SourceGeneration",
                        DiagnosticSeverity.Error,
                        true),
                    attributeContextAndType.ArgumentSyntax.Value.Skip(2).FirstOrDefault()?.Expression?.GetLocation() ??
                    attributeContextAndType.Context.TargetNode.GetLocation(),
                    "nameContains 请使用字符串"));
            return;
        }

        // 从 nameContainsLiteralExpression 中获取字符串 并去掉引号
        var nameContains = nameContainsLiteralExpression.ToString().Replace("\"", "");

        var additionalTexts = additionalTextArray.Where(it => it.Path.Contains(nameContains));
        var nameSpace =
            generateCtx.TargetSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
                .Replace("global::", "");

        // generateCtx.
        
        
        
        foreach (var additionalText in additionalTexts)
        {
            var xmlContent = additionalText.GetText()?.ToString();

            if (string.IsNullOrEmpty(xmlContent))
            {
                continue;
            }
            

            try
            {
                XmlDocument xmlDocument = new();
                xmlDocument.LoadXml(xmlContent);

                // 调用 convertMethod 生成一个 IEmbeddedResourceXmlI18nParser
                var declaringTypeName = string.Format(
                    "{0}.{1}",
                    nameSpace,
                    convertMethod.ContainingType.Name
                );
                var methodArgumentTypeNames =
                    convertMethod.Parameters.Select(p => p.Type.ContainingNamespace.Name + "." + p.Type.Name
                    );
                var methodInfo = Type.GetType(declaringTypeName)?.GetMethod(
                    methodName,
                    methodArgumentTypeNames.Select(typeName => Type.GetType(typeName)).ToArray()
                );
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        var translatorProviderName = string.Empty;
        var targetFullName = string.Empty;
        var addOrUpdate = string.Empty;
        var i18nAddOrUpdate = string.Empty;
        var i18nUnit = string.Empty;

        var f = Format;
        // context.AddSource(
        //     $"{generateCtx.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "")}.g.cs",
        //     Format.Replace("$TranslatorProviderName$", translatorProviderName)
        //         .Replace("$ResxTypeName$", targetFullName)
        //         .Replace("$NameSpace$", nameSpace)
        //         .Replace("$ClassName$", generateCtx.TargetSymbol.Name)
        //         .Replace("$AddOrUpdate$", addOrUpdate)
        //         .Replace("$I18nAddOrUpdate$", i18nAddOrUpdate)
        //         .Replace("$I18nUnit$", i18nUnit)
        // );
    }

    private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        return Assembly.GetExecutingAssembly();
    }
}
