using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;

namespace I18n.Avalonia.TranslatorProviders.Xml;

public abstract class AbsXmlI18nProvider : FileTranslatorProvider
{
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
            if (DefaultCultureValue.Any())
                throw new XmlException("xml document culture is null, and default culture value is set.");

            ParseXml2Dic(xmlDocument, DefaultCultureValue);
        }
        else
        {
            var dic = new Dictionary<string, string>();
            ParseXml2Dic(xmlDocument, dic);
            CultureValues.Add(xmlDocumentCulture, dic);
        }
    }
}
