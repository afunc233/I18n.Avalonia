using System;
using System.Collections.Generic;
using System.Globalization;
using I18n.Avalonia.TranslatorProviders;

namespace I18n.Avalonia;

public static class I18nProvider
{
    public static readonly IList<ITranslatorProvider> TranslatorProviders = [];

    static I18nProvider()
    {
        Culture = CultureInfo.CurrentUICulture;
    }

    public static CultureInfo Culture { get; private set; }
    public static event EventHandler<CultureInfo>? OnCultureChanged;

    public static void Add(ITranslatorProvider provider)
    {
        provider.SetCulture(Culture);
        TranslatorProviders.Add(provider);
    }

    public static void SetCulture(CultureInfo culture)
    {
        Culture = culture;
        foreach (var translatorProvider in TranslatorProviders)
        {
            translatorProvider.SetCulture(culture);
            translatorProvider.Refresh();
        }

        OnCultureChanged?.Invoke(null, Culture);
    }
}
