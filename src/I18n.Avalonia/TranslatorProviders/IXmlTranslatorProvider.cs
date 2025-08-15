using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace I18n.Avalonia.TranslatorProviders;

public interface IXmlTranslatorProvider : ITranslatorProvider
{
    Dictionary<CultureInfo, Dictionary<string, string>> CultureValues { get; }

    Dictionary<string, string> DefaultCultureValue { get; }

    CultureInfo Culture { get; }
}

public abstract class DefaultXmlI18nProvider : IXmlTranslatorProvider
{
    private CultureInfo _culture = I18nProvider.Culture;

    private readonly ICollection<I18nUnit> _i18nUnits = [];

    private readonly Dictionary<CultureInfo, Dictionary<string, string>> _cultureValues = new();

    private readonly Dictionary<string, string> _defaultCultureValue = new();

    ICollection<I18nUnit> ITranslatorProvider.I18nUnits => _i18nUnits;

    protected virtual CultureInfo? GetCulture(XmlDocument xmlDocument)
    {
        var rootNode = xmlDocument.FirstChild;
        var cultureNodes = rootNode.Attributes?.GetNamedItem("culture");
        return cultureNodes is null ? null : CultureInfo.GetCultureInfo(cultureNodes.Value);
    }

    protected virtual void ParseXml2Dic(XmlDocument xmlDocument, Dictionary<string, string> dictionary)
    {
        var rootNode = xmlDocument.FirstChild;
        rootNode.ParseXml2Dic(dictionary);
    }

    protected void AddXml(XmlDocument xmlDocument)
    {
        var xmlDocumentCulture = GetCulture(xmlDocument);

        if (xmlDocumentCulture is null)
        {
            if (_defaultCultureValue.Any())
            {
                throw new XmlException("xml document culture is null, and default culture value is set.");
            }

            ParseXml2Dic(xmlDocument, _defaultCultureValue);
        }
        else
        {
            var dic = new Dictionary<string, string>();
            ParseXml2Dic(xmlDocument, dic);
            _cultureValues.Add(xmlDocumentCulture, dic);
        }
    }

    void ITranslatorProvider.SetCulture(CultureInfo culture)
    {
        _culture = culture;
    }

    string? ITranslatorProvider.GetString(string key)
    {
        return this.GetStringOrDefault(key);
    }

    Dictionary<CultureInfo, Dictionary<string, string>> IXmlTranslatorProvider.CultureValues => _cultureValues;

    Dictionary<string, string> IXmlTranslatorProvider.DefaultCultureValue => _defaultCultureValue;

    CultureInfo IXmlTranslatorProvider.Culture => _culture;
}
