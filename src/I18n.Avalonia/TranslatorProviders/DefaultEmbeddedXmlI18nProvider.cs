using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace I18n.Avalonia.TranslatorProviders;

/// <summary>
/// default embedded xml i18n provider
/// </summary>
public class DefaultEmbeddedXmlI18nProvider : DefaultXmlI18nProvider
{
    public DefaultEmbeddedXmlI18nProvider(string resourceKey, Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        var manifestResourceNames = assembly.GetManifestResourceNames();

        foreach (var resourceName in manifestResourceNames.Where(it => it.Split('.').Contains(resourceKey)))
        {
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                continue;
            }

            using var xmlReader = XmlReader.Create(stream);

            XmlDocument xmlDocument = new();
            xmlDocument.Load(xmlReader);

            AddXml(xmlDocument);
        }
    }
}
