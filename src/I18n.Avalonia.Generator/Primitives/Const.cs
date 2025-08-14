namespace I18n.Avalonia.Generator.Primitives;

internal static class Const
{
    internal static string RootNamespace => "I18n.Avalonia";

    internal static string AttributeNamespace => $"{RootNamespace}.Attributes";

    internal static string ResxI18nOfAttribute => nameof(ResxI18nOfAttribute);

    internal static string ResxKeysOfAttributeFullName => $"{AttributeNamespace}.{nameof(ResxI18nOfAttribute)}";
    
    internal static string LocalXmlI18nOfAttribute => nameof(LocalXmlI18nOfAttribute);
    
    internal static string LocalXmlKeysOfAttributeFullName => $"{AttributeNamespace}.{nameof(LocalXmlI18nOfAttribute)}";
    
    internal static string EmbeddedResourceXmlI18nOfAttribute => nameof(EmbeddedResourceXmlI18nOfAttribute);
    
    internal static string EmbeddedResourceXmlKeysOfAttributeFullName => $"{AttributeNamespace}.{nameof(EmbeddedResourceXmlI18nOfAttribute)}";
    
    internal static string IEmbeddedResourceXmlI18nParser => nameof(IEmbeddedResourceXmlI18nParser);

}
