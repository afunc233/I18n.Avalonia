using System;
using I18n.Avalonia.Primitives;

namespace I18n.Avalonia;

public class I18nUnit(ITranslatorProvider translatorProvider, string key)
{
    private readonly Observable<string?> _value = new(key);

    public string? CurrentValue => _value.Value;

    public IObservable<string?> Value => _value;

    private string? Next()
    {
        return translatorProvider.GetString(key);
    }

    public void Refresh()
    {
        _value.OnNext(Next());
    }
}
