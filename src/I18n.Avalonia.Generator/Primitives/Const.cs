namespace I18n.Avalonia.Generator.Primitives;

internal static class Const
{
    internal static string RootNamespace => "I18n.Avalonia";

    internal static string AttributeNamespace => $"{RootNamespace}.Attributes";

    internal static string ResxI18nOfAttribute => nameof(ResxI18nOfAttribute);

    internal static string ResxKeysOfAttributeFullName => $"{AttributeNamespace}.{nameof(ResxI18nOfAttribute)}";
    
    internal static string XmlI18nAttribute => nameof(XmlI18nAttribute);
    
    internal static string XmlI18nKeysAttribute => nameof(XmlI18nKeysAttribute);
    
    internal static string XmlI18nProviderAttribute => nameof(XmlI18nProviderAttribute);
    
    internal static string XmlKeysOfAttributeFullName => $"{AttributeNamespace}.{nameof(XmlI18nAttribute)}";
    
    internal static string EmbeddedResourceXmlI18nOfAttribute => nameof(EmbeddedResourceXmlI18nOfAttribute);

    internal static string ITranslatorProvider => nameof(ITranslatorProvider);
}
