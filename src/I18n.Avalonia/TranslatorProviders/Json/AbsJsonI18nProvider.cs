using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace I18n.Avalonia.TranslatorProviders.Json;

public abstract class AbsJsonI18nProvider : FileTranslatorProvider
{
    protected virtual CultureInfo? GetCulture(string jsonString)
    {
        // 从 jsonString culture 中获取 cultureInfo {"culture": "en"}

        var match = Regex.Match(jsonString, "\"culture\"\\s*:\\s*\"([^\"]+)\"");
        var culture = match.Success ? match.Groups[1].Value : null;

        return culture is null ? null : CultureInfo.GetCultureInfo(culture);
    }

    protected virtual void ParseJson2Dic(string jsonString, Dictionary<string, string> dictionary)
    {
        // 从 jsonString 中解析出 key value 并添加到 dictionary 中
        var matchs = Regex.Matches(jsonString,
            ",?\\s*\"key\"\\s*:\\s*\"([^\"]+)\"\\s*,\\s*\"value\"\\s*:\\s*\"([^\"]+)\"\\s*");

        foreach (Match match in matchs)
            if (match.Success && match.Groups.Count >= 3)
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;
                dictionary.Add(key, value);
            }
    }

    protected void AddJson(string jsonString)
    {
        var cultureInfo = GetCulture(jsonString);

        if (cultureInfo is null)
        {
            if (DefaultCultureValue.Any())
                throw new XmlException("xml document culture is null, and default culture value is set.");

            ParseJson2Dic(jsonString, DefaultCultureValue);
        }
        else
        {
            var dic = new Dictionary<string, string>();
            ParseJson2Dic(jsonString, dic);
            CultureValues.Add(cultureInfo, dic);
        }
    }
}
