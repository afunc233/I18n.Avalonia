using System.Collections.Generic;
using System.Globalization;

namespace I18n.Avalonia.TranslatorProviders;

public abstract class FileTranslatorProvider : IFileTranslatorProvider
{
    ICollection<I18nUnit> ITranslatorProvider.I18nUnits { get; } = [];

    void ITranslatorProvider.SetCulture(CultureInfo culture)
    {
        Culture = culture;
    }

    string? ITranslatorProvider.GetString(string key)
    {
        return this.GetStringOrDefault(key);
    }

    public Dictionary<CultureInfo, Dictionary<string, string>> CultureValues { get; } = new();

    public Dictionary<string, string> DefaultCultureValue { get; } = new();

    public CultureInfo Culture { get; private set; } = I18nProvider.Culture;

    public abstract void FillValues();
}
