using System;
using System.Globalization;
using System.Reactive.Linq;
using I18n.Avalonia.Sample.I18ns;
using I18n.Avalonia.Tests.Helper;
using NUnit.Framework;

namespace I18n.Avalonia.Tests;

public class I18nProviderTest
{
    [Test]
    [Order(int.MinValue)]
    public void InitValue()
    {
        TestHelper.Excute(() =>
        {
            I18nProvider.SetCulture(TestHelper.zh);
            Assert.That(!string.Equals(nameof(LangKeys.Language), LangKeys.Language.CurrentValue));
        });
    }

    [Test]
    public void TestOnCultureChanged()
    {
        TestHelper.Excute(() =>
        {
            var ci = new CultureInfo("ar");
            EventHandler<CultureInfo> handler = (sender, info) =>
            {
                Assert.That(info, Is.EqualTo(ci));
            };
            I18nProvider.OnCultureChanged += handler;
            I18nProvider.SetCulture(ci);
            I18nProvider.OnCultureChanged -= handler;
        });
    }

    [Test]
    public void TestCultureChangedObservable()
    {
        TestHelper.Excute(() =>
        {
            var ci = new CultureInfo("fr");

            var o = Observable.FromEventPattern<CultureInfo>(
                add => I18nProvider.OnCultureChanged += add,
                rm => I18nProvider.OnCultureChanged -= rm
            ).Select(it => it.EventArgs).Subscribe(culture =>
            {
                Assert.That(ci, Is.EqualTo(culture));
            });

            I18nProvider.SetCulture(ci);
            o.Dispose();
        });
    }
}
