using System;
using System.Collections.Generic;
using System.Globalization;

namespace I18n.Avalonia;

public interface ITranslatorProvider
{
    List<I18nUnit> I18nUnits { get; }
    
    void AddOrUpdate(string key, Func<string?> value);

    void SetCulture(CultureInfo culture);

    string? GetString(string key);
}
