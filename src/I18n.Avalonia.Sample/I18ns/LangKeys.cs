using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using I18n.Avalonia.Attributes;
using I18n.Avalonia.Sample.Properties;

namespace I18n.Avalonia.Sample.I18ns;

/// <summary>
/// must public static partial 
/// </summary>
[ResxI18nOf(typeof(Resources))]
public static partial class LangKeys;

/// <summary>
/// also support multiple Resources
/// </summary>
[ResxI18nOf(typeof(ResourcesAbc))]
public static partial class LangAbcKeys;

// /// <summary>
// /// also support local xml 
// /// </summary>
// [LocalXmlI18nOf(typeof(LocalXmlLangKeys), nameof(Parse), "xml")]
// public static partial class LocalXmlLangKeys
// {
//     public static string Parse(string key)
//     {
//         return key;
//     }
// }

/// <summary>
/// also support multiple Resources
/// </summary>
[EmbeddedResourceXmlI18nOf(typeof(EmbeddedResourceXmlLangKeys), nameof(Parser), "EmbeddedResourceXml")]
public static partial class EmbeddedResourceXmlLangKeys
{
    public static EmbeddedResourceXmlI18nParser Parser()
    {
        return new EmbeddedResourceXmlI18nParser();
    }

    public class EmbeddedResourceXmlI18nParser : IEmbeddedResourceXmlI18nParser
    {
        public string XmlContent { get; set; } = null!;
        public string XmlName { get; set; } = null!;
        
        public CultureInfo Culture { get; } =  null!;

        public List<(string, string)> Parse()
        {
            // var xmlDocument = new XmlDocument();
            // xmlDocument.LoadXml(XmlContent);
            // 
            // var list = new List<(string, string)>();
            // foreach (XmlNode node in nodes)
            // {
            //     list.Add((node.Attributes["name"].Value, node.InnerText));
            // }
            // return list;
            return [];
        }
    }
}
