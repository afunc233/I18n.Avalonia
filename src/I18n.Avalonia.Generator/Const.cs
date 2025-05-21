namespace I18n.Avalonia.Generator;

internal static class Const
{
    internal static string RootNamespace => "I18n.Avalonia";

    internal static string AttributeNamespace => $"{RootNamespace}.Attributes";

    internal static string ResxI18nOfAttribute => nameof(ResxI18nOfAttribute);

    internal static string ResxKeysOfAttributeFullName => $"{AttributeNamespace}.{nameof(ResxI18nOfAttribute)}";
}
