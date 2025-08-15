using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using I18n.Avalonia.Sample.I18ns;
using I18n.Avalonia.Tests.Helper;

namespace I18n.Avalonia.Tests.VM;

public class ViewModelItem : ViewModelBase
{
    public I18nUnit Current_language_is
    {
        get => LangKeys.Current_language_is;
    }

    public string Value1 { get; set; } = Guid.NewGuid().ToString();

    public string Value2 { get; set; } = Guid.NewGuid().ToString();

    public IEnumerable<object> Args
    {
        get => [Value1, Value2];
    }
}

public class ViewModel : ViewModelBase
{
    public ViewModel()
    {
        foreach (var index in Enumerable.Range(0, 10000)) Items.Add(new ViewModelItem());
    }

    public List<ViewModelItem> Items { get; } = new();

    public CultureInfo Culture
    {
        get => I18nProvider.Culture;
        set
        {
            if (I18nProvider.Culture.EnglishName.Equals(value.EnglishName)) return;
            I18nProvider.SetCulture(value);
            RaisePropertyChanged();
        }
    }

    public I18nUnit Language
    {
        get => LangKeys.Chinese;
    }

    public I18nUnit Current_language_is
    {
        get => LangKeys.Current_language_is;
    }

    public I18nUnit Addition_formula_2
    {
        get => LangKeys.Addition_formula_2;
    }

    public IEnumerable<object> Args
    {
        get => new List<object>
        {
            LangKeys.Language, new Binding($"{nameof(Culture)}.{nameof(Culture.NativeName)}") { Source = this }
        };
    }

    public IList<CultureInfo> AvailableCultures { get; } = new List<CultureInfo>
    {
        TestHelper.zh, TestHelper.en, TestHelper.fr
    };

    public IList<string> AvailableKeys { get; } = [];
}
