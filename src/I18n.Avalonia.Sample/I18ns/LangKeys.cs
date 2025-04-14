using I18n.Avalonia.Attributes;
using I18n.Avalonia.Sample.Properties;

namespace I18n.Avalonia.Sample.I18ns;

/// <summary>
/// 必须是 public static partial 
/// </summary>
[ResxKeysOf(typeof(Resources))]
public static partial class LangKeys;

[ResxKeysOf(typeof(ResourcesAbc))]
public static partial class LangAbcKeys;
