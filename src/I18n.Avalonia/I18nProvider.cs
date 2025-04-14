using System;
using System.Collections.Generic;
using System.Globalization;

namespace I18n.Avalonia;

public static class I18nProvider
{
    public static event EventHandler<CultureInfo>? OnCultureChanged;

    public static CultureInfo Culture { get; private set; }

    static I18nProvider()
    {
        Culture = CultureInfo.CurrentUICulture;
    }

    public static readonly IList<ITranslatorProvider> TranslatorProviders = [];
    
    
    public static void Add(ITranslatorProvider provider)
    {
        TranslatorProviders.Add(provider);
    }
    
    public static void SetCulture(CultureInfo culture)
    {
        Culture = culture;
        foreach (var translatorProvider in TranslatorProviders)
        {
            translatorProvider.SetCulture(culture);
        }
        OnCultureChanged?.Invoke(null, Culture);
    }
}