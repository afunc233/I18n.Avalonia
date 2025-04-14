namespace I18n.Avalonia.Generator;

internal static class Const
{
    internal static string RootNamespace => "I18n.Avalonia";
    
    internal static string AttributeNamespace => $"{RootNamespace}.Attributes";
    
    internal static string ResxKeysOfAttribute => nameof(ResxKeysOfAttribute);

    internal static string ResxKeysOfAttributeFullName => $"{AttributeNamespace}.{nameof(ResxKeysOfAttribute)}";
    
}
