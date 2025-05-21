using System;
using System.Globalization;
using System.Reactive.Linq;
using I18n.Avalonia.Tests.Helper;
using Xunit;

namespace I18n.Avalonia.Tests;

public class I18nProviderTest
{
    [Fact]
    public void TestOnCultureChanged()
    {
        TestHelper.Excute(() =>
        {
            var ci = new CultureInfo("ar");
            EventHandler<CultureInfo> handler = (sender, info) =>
            {
                Assert.Equal(ci, info);
            };
            I18nProvider.Instance.OnCultureChanged += handler;
            I18nProvider.Instance.SetCulture(ci);
            I18nProvider.Instance.OnCultureChanged -= handler;
        });
    }

    [Fact]
    public void TestCultureChangedObservable()
    {
        TestHelper.Excute(() =>
        {
            var ci = new CultureInfo("fr");

            var o = Observable.FromEventPattern<CultureInfo>(
                add => I18nProvider.Instance.OnCultureChanged += add,
                rm => I18nProvider.Instance.OnCultureChanged -= rm
            ).Select(it => it.EventArgs).Subscribe(culture =>
            {
                Assert.Equal(ci, culture);
            });

            I18nProvider.Instance.SetCulture(ci);
            o.Dispose();
        });
    }
}
