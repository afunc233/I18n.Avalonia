using System.Collections.Generic;
using System.Globalization;

namespace I18n.Avalonia;

public interface ITranslatorProvider
{
    ICollection<I18nUnit> I18nUnits { get; }
    
    void SetCulture(CultureInfo culture);

    string? GetString(string key);
}
