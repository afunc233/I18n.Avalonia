namespace I18n.Avalonia.Generator.Primitives;

internal static class Const
{
    internal static string RootNamespace => "I18n.Avalonia";

    internal static string AttributeNamespace => $"{RootNamespace}.Attributes";

    internal static string ResxI18nOfAttribute => nameof(ResxI18nOfAttribute);

    internal static string ResxKeysOfAttributeFullName => $"{AttributeNamespace}.{nameof(ResxI18nOfAttribute)}";
    
    internal static string XmlI18nOfAttribute => nameof(XmlI18nOfAttribute);
    
    internal static string XmlKeysOfAttributeFullName => $"{AttributeNamespace}.{nameof(XmlI18nOfAttribute)}";
}
