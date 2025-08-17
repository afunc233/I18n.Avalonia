using System.IO;
using System.Xml;

namespace I18n.Avalonia.TranslatorProviders.Xml;

public class DefaultLocalXmlI18nProvider(string filePath) : AbsXmlI18nProvider
{
    public override void FillValues()
    {
        if (!Directory.Exists(filePath)) throw new DirectoryNotFoundException(filePath);

        foreach (var file in Directory.EnumerateFiles(filePath))
        {
            using var xmlReader = XmlReader.Create(file);

            XmlDocument xmlDocument = new();
            xmlDocument.Load(xmlReader);

            AddXml(xmlDocument);
        }
    }
}
