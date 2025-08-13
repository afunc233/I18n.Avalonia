using System;
using I18n.Avalonia.Primitives;

namespace I18n.Avalonia;

public class I18nUnit
{
    private readonly Observable<string?> _value;

    public string? CurrentValue => _value.Value;

    public IObservable<string?> Value => _value;

    private readonly ITranslatorProvider _translatorProvider;
    private readonly string _key;
    public I18nUnit(ITranslatorProvider translatorProvider, string key)
    {
        _translatorProvider = translatorProvider;
        _key = key;
        
        _value = new Observable<string?>(Next());
    }
    
    private string? Next()
    {
        return _translatorProvider.GetString(_key);
    }
    
    public void Refresh()
    {
        _value.OnNext(Next());
    }
}

