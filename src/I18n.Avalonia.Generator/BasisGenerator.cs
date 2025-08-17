using I18n.Avalonia.Generator.Primitives;
using Microsoft.CodeAnalysis;

namespace I18n.Avalonia.Generator;

[Generator(LanguageNames.CSharp)]
internal class BasisGenerator : IIncrementalGenerator
{
    private static readonly string I18nKeysAttributeSource =
        $"""
         using System;
         using System.Collections.Generic;
         using System.Diagnostics.CodeAnalysis;
         using System.Xml;

         namespace {Const.AttributeNamespace};
         #pragma warning disable CS9113

         [AttributeUsage(AttributeTargets.Field, Inherited = false)]
         public sealed class {Const.I18nKeysAttribute} : Attribute;

         #pragma warning restore CS9113
         """;

    private static readonly string I18nProviderAttributeSource =
        $"""
         using System;
         using System.Collections.Generic;
         using System.Diagnostics.CodeAnalysis;
         using System.Xml;

         namespace {Const.AttributeNamespace};
         #pragma warning disable CS9113

         [AttributeUsage(AttributeTargets.Field, Inherited = false)]
         public sealed class {Const.I18nProviderAttribute} : Attribute;

         #pragma warning restore CS9113
         """;

    private static readonly string ResxKeysOfAttributeSource =
        $"""
         using System;

         namespace {Const.AttributeNamespace};
         #pragma warning disable CS9113
         [AttributeUsage(AttributeTargets.Class, Inherited = false)]
         public class {Const.ResxI18nOfAttribute}(Type resourceType) : Attribute;
         #pragma warning restore CS9113
         """;

    private static readonly string FileBasedI18nAttributeSource =
        $"""
         using System;
         using System.Collections.Generic;
         using System.Diagnostics.CodeAnalysis;
         using System.Xml;

         namespace {Const.AttributeNamespace};
         #pragma warning disable CS9113

         [AttributeUsage(AttributeTargets.Class, Inherited = false)]
         public sealed class {Const.FileBasedI18nAttribute} : Attribute;

         #pragma warning restore CS9113
         """;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            // keys
            ctx.AddSource($"{Const.I18nKeysAttribute}.g.cs", I18nKeysAttributeSource);

            // provider
            ctx.AddSource($"{Const.I18nProviderAttribute}.g.cs", I18nProviderAttributeSource);

            // add resx
            ctx.AddSource($"{Const.ResxI18nOfAttribute}.g.cs", ResxKeysOfAttributeSource);

            // add xml
            ctx.AddSource($"{Const.FileBasedI18nAttribute}.g.cs", FileBasedI18nAttributeSource);
        });
    }
}
