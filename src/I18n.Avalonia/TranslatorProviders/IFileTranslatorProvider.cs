using System.Collections.Generic;
using System.Globalization;

namespace I18n.Avalonia.TranslatorProviders;

public interface IFileTranslatorProvider : ITranslatorProvider
{
    Dictionary<CultureInfo, Dictionary<string, string>> CultureValues { get; }

    Dictionary<string, string> DefaultCultureValue { get; }

    CultureInfo Culture { get; }

    void FillValues();
}
