using System;
using System.Collections.Generic;
using System.Globalization;
using I18n.Avalonia.TranslatorProviders;

namespace I18n.Avalonia;

public class CultureChangedEventArgs(CultureInfo culture, IList<ITranslatorProvider> translatorProviders) : EventArgs
{
    public CultureInfo Culture { get; } = culture;

    public IList<ITranslatorProvider> TranslatorProviders { get; } = translatorProviders;
}
public static class I18nProvider
{
    private static readonly IList<ITranslatorProvider> TranslatorProviders = [];

    static I18nProvider()
    {
        Culture = CultureInfo.CurrentUICulture;
    }

    public static CultureInfo Culture { get; private set; }
    public static event EventHandler<CultureChangedEventArgs>? OnCultureChanged;

    public static void Add(ITranslatorProvider provider)
    {
        lock (TranslatorProviders)
        {
            provider.SetCulture(Culture);
            TranslatorProviders.Add(provider);
        }
    }

    public static void SetCulture(CultureInfo culture)
    {
        lock (TranslatorProviders)
        {
            Culture = culture;
            foreach (var translatorProvider in TranslatorProviders)
            {
                translatorProvider.SetCulture(culture);
                translatorProvider.Refresh();
            }

            OnCultureChanged?.Invoke(null, new CultureChangedEventArgs(culture, TranslatorProviders));
        }
    }
}
