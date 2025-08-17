using System.Linq;
using System.Reflection;
using System.Xml;

namespace I18n.Avalonia.TranslatorProviders.Xml;

/// <summary>
///     default embedded xml i18n provider
/// </summary>
public class DefaultEmbeddedXmlI18nProvider(string resourceKey, Assembly? assembly = null) : AbsXmlI18nProvider
{
    private readonly Assembly _assembly = assembly ?? Assembly.GetCallingAssembly();

    public override void FillValues()
    {
        var manifestResourceNames = _assembly.GetManifestResourceNames();

        foreach (var resourceName in manifestResourceNames.Where(it => it.Split('.').Contains(resourceKey)))
        {
            using var stream = _assembly.GetManifestResourceStream(resourceName);
            if (stream is null) continue;

            using var xmlReader = XmlReader.Create(stream);

            XmlDocument xmlDocument = new();
            xmlDocument.Load(xmlReader);

            AddXml(xmlDocument);
        }
    }
}
