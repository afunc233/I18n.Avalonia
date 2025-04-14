using System;
using System.Collections.Generic;
using System.ComponentModel;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Metadata;

namespace I18n.Avalonia;

public sealed class I18nExtension : MarkupExtension
{
    public I18nUnit Key { get; init; } = null!;

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [DefaultValue(null)]
    [Content]
    public IEnumerable<object> Args { get; init; } = new List<object>();

    public I18nExtension()
    {
    }

    public I18nExtension(I18nUnit key)
    {
        Key = key;
    }

    public I18nExtension(I18nUnit key, BindingBase args)
    {
        Key = key;
        Args = [args];
    }

    public override object ProvideValue(IServiceProvider? serviceProvider)
    {
        return Key.ToBinding(Args);
    }
}
