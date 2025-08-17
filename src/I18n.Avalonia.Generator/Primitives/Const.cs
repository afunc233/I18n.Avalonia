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

    internal static string FileBasedI18nAttribute
    {
        get => nameof(FileBasedI18nAttribute);
    }

    internal static string I18nKeysAttribute
    {
        get => nameof(I18nKeysAttribute);
    }

    internal static string I18nProviderAttribute
    {
        get => nameof(I18nProviderAttribute);
    }

    internal static string FileKeysAttributeFullName
    {
        get => $"{AttributeNamespace}.{nameof(FileBasedI18nAttribute)}";
    }

    internal static string ITranslatorProvider
    {
        get => nameof(ITranslatorProvider);
    }

    internal static string IFileTranslatorProvider
    {
        get => nameof(IFileTranslatorProvider);
    }
}
