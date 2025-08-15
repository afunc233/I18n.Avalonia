namespace I18n.Avalonia.Generator.Primitives;

internal static class Const
{
    internal static string RootNamespace
    {
        get => "I18n.Avalonia";
    }

    internal static string AttributeNamespace
    {
        get => $"{RootNamespace}.Attributes";
    }

    internal static string ResxI18nOfAttribute
    {
        get => nameof(ResxI18nOfAttribute);
    }

    internal static string ResxKeysOfAttributeFullName
    {
        get => $"{AttributeNamespace}.{nameof(ResxI18nOfAttribute)}";
    }

    internal static string XmlI18nAttribute
    {
        get => nameof(XmlI18nAttribute);
    }

    internal static string XmlI18nKeysAttribute
    {
        get => nameof(XmlI18nKeysAttribute);
    }

    internal static string XmlI18nProviderAttribute
    {
        get => nameof(XmlI18nProviderAttribute);
    }

    internal static string XmlKeysOfAttributeFullName
    {
        get => $"{AttributeNamespace}.{nameof(XmlI18nAttribute)}";
    }

    internal static string EmbeddedResourceXmlI18nOfAttribute
    {
        get => nameof(EmbeddedResourceXmlI18nOfAttribute);
    }

    internal static string ITranslatorProvider
    {
        get => nameof(ITranslatorProvider);
    }
}
