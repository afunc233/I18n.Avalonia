using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using I18n.Avalonia.Sample.I18ns;
using I18n.Avalonia.Tests.Helper;

namespace I18n.Avalonia.Tests.VM;

public partial class ViewModelItem : ViewModelBase
{
    public I18nUnit Current_language_is => LangKeys.Current_language_is;

    public string Value1 { get; set; } = Guid.NewGuid().ToString();

    public string Value2 { get; set; } = Guid.NewGuid().ToString();

    public IEnumerable<object> Args => [Value1, Value2];
}

public class ViewModel : ViewModelBase
{
    public List<ViewModelItem> Items { get; } = new List<ViewModelItem>();

    public ViewModel()
    {
        foreach (var index in Enumerable.Range(0, 10000))
        {
            Items.Add(new ViewModelItem { });
        }
    }

    public CultureInfo Culture
    {
        get => I18nProvider.Instance.Culture;
        set
        {
            if (I18nProvider.Instance.Culture.EnglishName.Equals(value.EnglishName)) return;
            I18nProvider.Instance.SetCulture(value);
            this.RaisePropertyChanged();
        }
    }

    public I18nUnit Language => LangKeys.Chinese;

    public I18nUnit Current_language_is => LangKeys.Current_language_is;

    public I18nUnit Addition_formula_2 => LangKeys.Addition_formula_2;

    public IEnumerable<object> Args =>
        new List<object>
        {
            LangKeys.Language, new Binding($"{nameof(Culture)}.{nameof(Culture.NativeName)}") { Source = this }
        };

    public IList<CultureInfo> AvailableCultures { get; } = new List<CultureInfo>
    {
        TestHelper.zh, TestHelper.en, TestHelper.fr
    };

    public IList<string> AvailableKeys { get; } = [];
}
