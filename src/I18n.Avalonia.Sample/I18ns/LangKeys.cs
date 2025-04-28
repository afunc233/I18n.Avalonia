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
