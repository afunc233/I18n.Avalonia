using System;
using System.Globalization;
using System.Threading;
using I18n.Avalonia.Tests.VM;

namespace I18n.Avalonia.Tests.Helper;

internal static class TestHelper
{
    internal const string TestAxamlPlaceholder = "$TestAxaml$";

    internal const string AxamlFormat = $"""
                                         <UserControl xmlns='https://github.com/avaloniaui'
                                                      xmlns:i18n='clr-namespace:I18n.Avalonia;assembly=I18n.Avalonia'
                                                      xmlns:gb='clr-namespace:System.Globalization;assembly=System.Runtime'
                                                      xmlns:sys='clr-namespace:System;assembly=System.Runtime'
                                                      xmlns:sample='clr-namespace:I18n.Avalonia.Sample.I18ns;assembly=I18n.Avalonia.Sample'
                                                      xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                                             {TestAxamlPlaceholder}
                                         </UserControl>
                                         """;

    internal static CultureInfo zh = CultureInfo.GetCultureInfo("zh");

    internal static CultureInfo en = CultureInfo.GetCultureInfo("en");

    internal static CultureInfo fr = CultureInfo.GetCultureInfo("fr");

    private static ViewModel? vm;

    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    internal static ViewModel VM
    {
        get => vm ??= new ViewModel();
    }


    internal static void Excute(Action action)
    {
        try
        {
            _semaphore.Wait();
            action.Invoke();
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
