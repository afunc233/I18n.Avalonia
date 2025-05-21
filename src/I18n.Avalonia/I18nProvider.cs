using System;
using System.Collections.Generic;
using System.Globalization;

namespace I18n.Avalonia;

public class I18nProvider
{
    public static I18nProvider Instance = new Lazy<I18nProvider>(() => new I18nProvider()).Value;
    public event EventHandler<CultureInfo>? OnCultureChanged;

    public CultureInfo Culture { get; private set; }

    public readonly IList<ITranslatorProvider> TranslatorProviders = [];

    private I18nProvider()
    {
        Culture = CultureInfo.CurrentUICulture;
    }

    public void Add(ITranslatorProvider provider)
    {
        TranslatorProviders.Add(provider);
    }

    public void SetCulture(CultureInfo culture)
    {
        Culture = culture;
        foreach (var translatorProvider in TranslatorProviders)
        {
            translatorProvider.SetCulture(culture);
        }

        OnCultureChanged?.Invoke(null, Culture);
    }
}
