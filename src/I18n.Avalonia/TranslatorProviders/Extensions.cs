using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace I18n.Avalonia.TranslatorProviders;

public static class Extensions
{
    public static string GetStringOrDefault(this IFileTranslatorProvider provider, string key)
    {
        var culture = provider.Culture;

        // 找到相同或者有关的culture
        var cultureInfo = provider.CultureValues.Keys.FirstOrDefault(it =>
            string.Equals(it.Name, culture.Name, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(it.TwoLetterISOLanguageName, culture.TwoLetterISOLanguageName,
                StringComparison.OrdinalIgnoreCase));

        cultureInfo ??= culture;

        if (!provider.CultureValues.TryGetValue(cultureInfo, out var value)) return DefaultValue(key);
        return value.TryGetValue(key, out var value1) ? value1 : DefaultValue(key);

        string DefaultValue(string k)
        {
            return provider.DefaultCultureValue.TryGetValue(k, out var defaultValue) ? defaultValue : k;
        }
    }

    public static void Refresh(this ITranslatorProvider provider)
    {
        foreach (var i18nUnit in provider.I18nUnits) i18nUnit.Refresh();
    }

    internal static void ParseXml2Dic(this XmlNode rootNode, Dictionary<string, string> cultureValue)
    {
        foreach (XmlNode childNode in rootNode.ChildNodes)
        {
            if (childNode is not XmlElement element) continue;

            cultureValue.Add(element.Name, element.InnerText);
        }
    }
}
