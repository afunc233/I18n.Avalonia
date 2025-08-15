using System;
using I18n.Avalonia.Primitives;
using I18n.Avalonia.Sample.I18ns;
using I18n.Avalonia.Tests.Helper;
using NUnit.Framework;

namespace I18n.Avalonia.Tests;

public class ObservableTest
{
    [Test]
    public void TestObservable()
    {
        TestHelper.Excute(() =>
        {
            Observable<string> observable = new(nameof(LangKeys.Language));

            var isFirst = true;

            var disposable = observable.Subscribe(str =>
            {
                if (isFirst)
                    Assert.That(str, Is.EqualTo(nameof(LangKeys.Language)));
                else
                    Assert.That(str, Is.EqualTo(nameof(LangKeys.Chinese)));
            });

            isFirst = false;
            observable.OnNext(nameof(LangKeys.Chinese));

            Assert.That(observable.Value, Is.EqualTo(nameof(LangKeys.Chinese)));

            observable.Dispose();

            Assert.That(observable, Is.Not.Null);

            // 测试下面的代码会触发异常
            Assert.Throws<ObjectDisposedException>(() =>
            {
                var value = observable.Value;
            });
        });
    }
}
