using System;
using System.IO;
using System.Xml;

namespace I18n.Avalonia.TranslatorProviders;

public class DefaultLocalXmlI18nProvider : DefaultXmlI18nProvider
{
    public DefaultLocalXmlI18nProvider(string filePath)
    {
        if (!Directory.Exists(filePath))
        {
            throw new DirectoryNotFoundException(filePath);
        }

        foreach (var file in Directory.EnumerateFiles(filePath))
        {
            using var xmlReader = XmlReader.Create(file);

            XmlDocument xmlDocument = new();
            xmlDocument.Load(xmlReader);

            AddXml(xmlDocument);
        }
    }
}
