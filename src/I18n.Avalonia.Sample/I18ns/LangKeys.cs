using System;
using System.IO;
using System.Reflection;
using I18n.Avalonia.Attributes;
using I18n.Avalonia.Sample.Properties;
using I18n.Avalonia.TranslatorProviders;
using I18n.Avalonia.TranslatorProviders.Json;
using I18n.Avalonia.TranslatorProviders.Xml;

namespace I18n.Avalonia.Sample.I18ns;

/// <summary>
///     must public static partial
/// </summary>
[ResxI18nOf(typeof(Resources))]
public static partial class LangKeys;

/// <summary>
///     also support multiple Resources
/// </summary>
[ResxI18nOf(typeof(ResourcesAbc))]
public static partial class LangAbcKeys;

/// <summary>
///     also support local xml
/// </summary>
[FileBasedI18n]
public static partial class LocalXmlLangKeys
{
#pragma warning disable CA1823
    [I18nKeys] private static readonly string[] s_keys =
    [
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v",
        "w", "x", "y", "z"
    ];
#pragma warning restore CA1823

    [I18nProvider] private static readonly IFileTranslatorProvider s_translator =
        new DefaultLocalXmlI18nProvider(Path.Combine(AppContext.BaseDirectory, "Xml"));
}

/// <summary>
///     also support multiple Resources
/// </summary>
[FileBasedI18n]
public static partial class EmbeddedXmlLangKeys
{
#pragma warning disable CA1823
    [I18nKeys] private static readonly string[] s_keys =
    [
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v",
        "w", "x", "y", "z"
    ];
#pragma warning restore CA1823

    [I18nProvider] private static readonly IFileTranslatorProvider s_translator =
        new DefaultEmbeddedXmlI18nProvider("EmbeddedResourceXml", Assembly.GetExecutingAssembly());
}

/// <summary>
///     also support multiple Resources
/// </summary>
[FileBasedI18n]
public static partial class EmbeddedJsonLangKeys
{
#pragma warning disable CA1823
    [I18nKeys] private static readonly string[] s_keys =
    [
        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v",
        "w", "x", "y", "z"
    ];
#pragma warning restore CA1823

    [I18nProvider] private static readonly IFileTranslatorProvider s_translator =
        new DefaultEmbeddedJsonI18nProvider("EmbeddedResourceJson", Assembly.GetExecutingAssembly());
}
