using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using I18n.Avalonia.Primitives;

namespace I18n.Avalonia;

/// <summary>
///     本附加参数 仅 处理 <see cref="I18nExtension" /> 类 不能处理的 Binding Key .
///     指在 axaml 里 使用 <see cref="I18nExtension" /> 的 Key 直接 绑定到 ViewModel
///     想不到办法 在 MarkupExtension 里 获取 Binding 的 <see cref="I18nUnit" /> 对象
///     Args 在 axaml 内定义 时 比较麻烦 见 本项目 的 测试项目 I18nAttachedTest.TestI18nAttachedWithAxamlArgs
/// </summary>
public class I18nAttached : AvaloniaObject
{
    public static readonly AttachedProperty<I18nUnit?> KeyProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, I18nUnit?>("Key", typeof(I18nAttached));

    public static readonly AttachedProperty<AvaloniaProperty<string?>> TextPropertyProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, AvaloniaProperty<string?>>("TextProperty",
            typeof(I18nAttached), TextBlock.TextProperty);

    public static readonly AttachedProperty<IEnumerable<object>?> ArgsProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, IEnumerable<object>?>("Args", typeof(I18nAttached),
            new List<object>());

    static I18nAttached()
    {
        KeyProperty.Changed.AddClassHandler<Control>(OnI18nUnitChanged);
        TextPropertyProperty.Changed.AddClassHandler<Control>(OnTextPropertyChanged);
        ArgsProperty.Changed.AddClassHandler<Control>(OnArgsChanged);
    }

    public static void SetKey(AvaloniaObject element, I18nUnit? commandValue)
    {
        element.SetValue(KeyProperty, commandValue);
    }

    public static I18nUnit? GetKey(AvaloniaObject element)
    {
        return element.GetValue(KeyProperty);
    }

    public static void SetTextProperty(AvaloniaObject element, AvaloniaProperty<string?> commandValue)
    {
        element.SetValue(TextPropertyProperty, commandValue);
    }

    public static AvaloniaProperty<string?> GetTextProperty(AvaloniaObject element)
    {
        return element.GetValue(TextPropertyProperty);
    }

    public static void SetArgs(AvaloniaObject element, IEnumerable<object>? commandValue)
    {
        element.SetValue(ArgsProperty, commandValue);
    }

    public static IEnumerable<object>? GetArgs(AvaloniaObject element)
    {
        return element.GetValue(ArgsProperty);
    }


    private static void OnI18nUnitChanged(Control control, AvaloniaPropertyChangedEventArgs changedEventArgs)
    {
        UpdateBinding(control);
    }

    private static void OnTextPropertyChanged(Control control, AvaloniaPropertyChangedEventArgs changedEventArgs)
    {
        UpdateBinding(control);
    }

    private static void OnArgsChanged(Control control, AvaloniaPropertyChangedEventArgs changedEventArgs)
    {
        UpdateBinding(control);
    }

    private static void UpdateBinding(AvaloniaObject avaloniaObject)
    {
        var textProperty = GetTextProperty(avaloniaObject);

        var i18nUnit = GetKey(avaloniaObject);

        if (i18nUnit is null) return;

        var args = GetArgs(avaloniaObject);

        avaloniaObject[!textProperty] = i18nUnit.ToBinding(args);
    }
}
