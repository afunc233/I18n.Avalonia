using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;
using I18n.Avalonia.Primitives;

namespace I18n.Avalonia;

public sealed class I18nExtension : MarkupExtension
{
    public I18nExtension()
    {
    }

    public I18nExtension(I18nUnit key) : this()
    {
        Key = key;
    }

    public I18nExtension(I18nUnit key, BindingBase args) : this(key)
    {
        Args = [args];
    }

    public I18nUnit Key { get; init; } = null!;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [DefaultValue(null)]
    [Content]
    public IEnumerable<object> Args { get; init; } = new List<object>();

    public override object ProvideValue(IServiceProvider? serviceProvider)
    {
        return Key.ToBinding(Args);
    }
}
