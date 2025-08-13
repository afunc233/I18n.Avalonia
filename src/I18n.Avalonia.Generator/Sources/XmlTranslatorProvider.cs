using System;
using System.IO;
using System.Globalization;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace I18n.Avalonia.Generator;

public class XmlTranslatorProvider : I18n.Avalonia.ITranslatorProvider
{
    private readonly string _mainPath;
    private readonly string? _dirPath;
    private CultureInfo _culture = null!;

    private readonly XmlDocument MainXmlDocument = null!;

    private readonly Dictionary<CultureInfo, XmlDocument> _documents = new();

    public List<I18nUnit> I18nUnits { get; } = [];

    void ITranslatorProvider.AddOrUpdate(string key, Func<string?> value)
    {
        
    }

    void ITranslatorProvider.SetCulture(CultureInfo culture)
    {
        _culture = culture;
    }

    string? ITranslatorProvider.GetString(string key)
    {
        var xmlDocument = _documents.TryGetValue(_culture, out var document) ? document : MainXmlDocument;

        return GetStringFromDoc(xmlDocument, key) ?? GetStringFromDoc(MainXmlDocument, key);

        static string? GetStringFromDoc(XmlDocument? document, string key)
        {
            var rootNode = document?.ChildNodes[0];

            if (rootNode is null)
            {
                return null;
            }

            return (from XmlNode xmlNode in rootNode.ChildNodes
                where string.Equals(xmlNode.LocalName, key, StringComparison.OrdinalIgnoreCase)
                select xmlNode.InnerText).FirstOrDefault();
        }
    }

    public XmlTranslatorProvider(string path, CultureInfo mainCulture)
    {
        _mainPath = path;
        _culture = mainCulture;

        MainXmlDocument = new XmlDocument();
        try
        {
            MainXmlDocument.Load(path);
            _documents.Add(mainCulture, MainXmlDocument);
            _dirPath = Path.GetDirectoryName(path);
            if (_dirPath is null)
            {
                return;
            }
            
            var mainFileName = Path.GetFileName(path);

            var files = Directory.EnumerateFiles(_dirPath, "*.xml", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                if (string.Equals(mainFileName, fileName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var culture = new CultureInfo(fileName.Replace(mainFileName, ""));
                var xmlDocument = new XmlDocument();
                try
                {
                    xmlDocument.Load(file);
                    _documents.Add(culture, xmlDocument);
                }
                catch
                {
                    // ignored
                }
            }
        }
        catch
        {
            // ignored
        }

    }
}
