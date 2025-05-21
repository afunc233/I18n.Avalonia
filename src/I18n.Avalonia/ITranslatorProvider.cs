using System;
using System.Globalization;

namespace I18n.Avalonia;

public interface ITranslatorProvider
{
    void AddOrUpdate(string key, Func<string?> value);

    void SetCulture(CultureInfo culture);

    string? GetString(string key);
}
