using System;
using I18n.Avalonia.Primitives;

namespace I18n.Avalonia;

public class I18nUnit
{
    private readonly Observable<string?> _value;

    public string? CurrentValue => _value.Value;

    public IObservable<string?> Value => _value;

    public I18nUnit(ITranslatorProvider translatorProvider, string key)
    {
        _value = new Observable<string?>(Next());

        I18nProvider.Instance.OnCultureChanged += (_, _) => { _value.OnNext(Next()); };
        return;

        string? Next()
        {
            return translatorProvider.GetString(key);
        }
    }
}
