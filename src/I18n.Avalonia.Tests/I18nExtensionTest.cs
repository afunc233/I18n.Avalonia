using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using I18n.Avalonia.Sample.I18ns;
using I18n.Avalonia.Tests.Helper;
using I18n.Avalonia.Tests.VM;
using NUnit.Framework;

namespace I18n.Avalonia.Tests;

public class I18nExtensionTest
{
    [Test]
    public void KeyPropertyInit()
    {
        TestHelper.Excute(() =>
        {
            I18nProvider.SetCulture(TestHelper.zh);

            var i18n = new I18nExtension { Key = LangKeys.Chinese };

            Assert.That(string.Equals("中文", i18n.Key.CurrentValue));

            I18nProvider.SetCulture(TestHelper.en);

            Assert.That(string.Equals("Chinese", i18n.Key.CurrentValue));

            I18nProvider.SetCulture(TestHelper.fr);

            Assert.That(string.Equals("Chinois", i18n.Key.CurrentValue));
        });
    }


    [Test]
    public void Key()
    {
        TestHelper.Excute(() =>
        {
            const string testAxaml =
                @"<TextBlock Name='textBlock' Text='{i18n:I18n Key={x:Static sample:LangKeys.Chinese}}' />";

            var xaml = TestHelper.AxamlFormat.Replace(TestHelper.TestAxamlPlaceholder, testAxaml);


            var userControl = (UserControl)AvaloniaRuntimeXamlLoader.Load(xaml);
            var textBlock = userControl.FindControl<TextBlock>("textBlock");

            Assert.That(textBlock, Is.Not.Null);

            I18nProvider.SetCulture(TestHelper.zh);
            Assert.That(string.Equals("中文", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.en);

            Assert.That(string.Equals("Chinese", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.fr);

            Assert.That(string.Equals("Chinois", textBlock.Text));
        });
    }

    [Test]
    public void KeyAndArgs()
    {
        TestHelper.Excute(() =>
        {
            const string testAxaml = """
                                     <TextBlock Name='textBlock'>
                                         <TextBlock.Text>
                                             <i18n:I18n Key="{x:Static sample:LangKeys.Current_language_is}">
                                                 <i18n:I18n.Args>
                                                     <i18n:I18n Key="{x:Static sample:LangKeys.Language}" />
                                                     <Binding Path="((gb:CultureInfo)Culture).NativeName" />
                                                 </i18n:I18n.Args>
                                             </i18n:I18n>
                                         </TextBlock.Text>
                                     </TextBlock>
                                     """;

            var xaml = TestHelper.AxamlFormat.Replace(TestHelper.TestAxamlPlaceholder, testAxaml);

            var userControl = (UserControl)AvaloniaRuntimeXamlLoader.Load(xaml);

            userControl.DataContext = TestHelper.VM;

            var textBlock = userControl.FindControl<TextBlock>("textBlock");

            Assert.That(textBlock, Is.Not.Null);

            TestHelper.VM.Culture = TestHelper.zh;

            Assert.That(string.Equals("当前的语言是 中文", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.en;

            Assert.That(string.Equals("Current Language is English", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.fr;

            Assert.That(string.Equals("Le Langue actuel est français", textBlock.Text));
        });
    }

    [Test]
    public void KeyAndValueArgs()
    {
        TestHelper.Excute(() =>
        {
            const string testAxaml = """
                                     <TextBlock Name='textBlock'>
                                         <TextBlock.Text>
                                             <i18n:I18n Key="{x:Static sample:LangKeys.Addition_formula_2}">
                                                 <i18n:I18n.Args>
                                                     <sys:Int32>1</sys:Int32>
                                                     <sys:Int32>1</sys:Int32>
                                                     <sys:Int32>2</sys:Int32>
                                                 </i18n:I18n.Args>
                                             </i18n:I18n>
                                         </TextBlock.Text>
                                     </TextBlock>
                                     """;

            var xaml = TestHelper.AxamlFormat.Replace(TestHelper.TestAxamlPlaceholder, testAxaml);

            var userControl = (UserControl)AvaloniaRuntimeXamlLoader.Load(xaml);

            userControl.DataContext = TestHelper.VM;

            var textBlock = userControl.FindControl<TextBlock>("textBlock");

            Assert.That(textBlock, Is.Not.Null);

            TestHelper.VM.Culture = TestHelper.zh;

            Assert.That(string.Equals("1 + 1 = 2", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.en;

            Assert.That(string.Equals("1 + 1 = 2", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.fr;

            Assert.That(string.Equals("1 + 1 = 2", textBlock.Text));
        });
    }

    [Test]
    public void BindingKey()
    {
        TestHelper.Excute(() =>
        {
            const string testAxaml = """
                                     <TextBlock x:Name="textBlock" Text="{Binding Language.Value^}" />
                                     """;

            var xaml = TestHelper.AxamlFormat.Replace(TestHelper.TestAxamlPlaceholder, testAxaml);

            var userControl = (UserControl)AvaloniaRuntimeXamlLoader.Load(xaml);

            userControl.DataContext = TestHelper.VM;

            var textBlock = userControl.FindControl<TextBlock>("textBlock");

            Assert.That(textBlock, Is.Not.Null);
            TestHelper.VM.Culture = TestHelper.zh;

            Assert.That(string.Equals("中文", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.en;

            Assert.That(string.Equals("Chinese", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.fr;

            Assert.That(string.Equals("Chinois", textBlock.Text));
        });
    }

    [Test]
    public void EmbeddedXmlKey()
    {
        TestHelper.Excute(() =>
        {
            const string testAxaml =
                @"<TextBlock Name='textBlock' Text='{i18n:I18n Key={x:Static sample:EmbeddedXmlLangKeys.a}}' />";

            var xaml = TestHelper.AxamlFormat.Replace(TestHelper.TestAxamlPlaceholder, testAxaml);

            var userControl = (UserControl)AvaloniaRuntimeXamlLoader.Load(xaml);
            var textBlock = userControl.FindControl<TextBlock>("textBlock");

            Assert.That(textBlock, Is.Not.Null);

            I18nProvider.SetCulture(TestHelper.zh);
            Assert.That(string.Equals("甲", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.en);

            Assert.That(string.Equals("a", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.fr);

            Assert.That(string.Equals("a", textBlock.Text));
        });
    }

    [Test]
    public void EmbeddedJsonKey()
    {
        TestHelper.Excute(() =>
        {
            const string testAxaml =
                @"<TextBlock Name='textBlock' Text='{i18n:I18n Key={x:Static sample:EmbeddedJsonLangKeys.a}}' />";

            var xaml = TestHelper.AxamlFormat.Replace(TestHelper.TestAxamlPlaceholder, testAxaml);

            var userControl = (UserControl)AvaloniaRuntimeXamlLoader.Load(xaml);
            var textBlock = userControl.FindControl<TextBlock>("textBlock");

            Assert.That(textBlock, Is.Not.Null);

            I18nProvider.SetCulture(TestHelper.zh);
            Assert.That(string.Equals("甲", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.en);

            Assert.That(string.Equals("a", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.fr);

            Assert.That(string.Equals("a", textBlock.Text));
        });
    }

    [Test]
    public void LocalXmlKey()
    {
        TestHelper.Excute(() =>
        {
            const string testAxaml =
                @"<TextBlock Name='textBlock' Text='{i18n:I18n Key={x:Static sample:LocalXmlLangKeys.a}}' />";

            var xaml = TestHelper.AxamlFormat.Replace(TestHelper.TestAxamlPlaceholder, testAxaml);

            var userControl = (UserControl)AvaloniaRuntimeXamlLoader.Load(xaml);
            var textBlock = userControl.FindControl<TextBlock>("textBlock");

            Assert.That(textBlock, Is.Not.Null);

            I18nProvider.SetCulture(TestHelper.zh);
            Assert.That(string.Equals("甲", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.en);

            Assert.That(string.Equals("a", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.fr);

            Assert.That(string.Equals("a", textBlock.Text));
        });
    }

    [Test]
    public void I18nExtension_Constructor()
    {
        TestHelper.Excute(() =>
        {
            var i18nExtension = new I18nExtension(LangAbcKeys.Chinese);

            Assert.That(i18nExtension.Key, Is.EqualTo(LangAbcKeys.Chinese));
            Assert.That(i18nExtension.Args, Is.Empty);
        });
    }


    [Test]
    public void I18nExtension_Constructor_WithArgs()
    {
        TestHelper.Excute(() =>
        {
            var vm = new ViewModel();
            var i18nExtension = new I18nExtension(LangKeys.Current_language_is,
                new Binding(nameof(ViewModel.Language)) { Source = vm });

            Assert.That(i18nExtension.Key, Is.EqualTo(LangKeys.Current_language_is));
            Assert.That(i18nExtension.Args, Is.Not.Empty);

            var result = i18nExtension.ProvideValue(null);

            var textBlock = new TextBlock();

            if (result is IBinding binding)
            {
                textBlock.Bind(TextBlock.TextProperty, binding);

                I18nProvider.SetCulture(TestHelper.zh);

                Assert.That(textBlock.Text, Is.Null);
                I18nProvider.SetCulture(TestHelper.en);
                Assert.That(textBlock.Text, Is.Null);
            }
            else
            {
                Assert.Fail("result is not IBinding");
            }
        });
    }

    [Test]
    public void I18nExtension_Constructor_WithArgs2()
    {
        TestHelper.Excute(() =>
        {
            var i18nExtension = new I18nExtension(LangKeys.Current_language_is) { Args = [LangKeys.Language, "中文"] };

            Assert.That(i18nExtension.Key, Is.EqualTo(LangKeys.Current_language_is));
            Assert.That(i18nExtension.Args, Is.Not.Empty);

            var result = i18nExtension.ProvideValue(null);

            var textBlock = new TextBlock();

            if (result is IBinding binding)
            {
                textBlock.Bind(TextBlock.TextProperty, binding);

                I18nProvider.SetCulture(TestHelper.zh);

                Assert.That(textBlock.Text, Is.EqualTo("当前的语言是 中文"));
                I18nProvider.SetCulture(TestHelper.en);
                Assert.That(textBlock.Text, Is.EqualTo("Current Language is 中文"));
            }
            else
            {
                Assert.Fail("result is not IBinding");
            }
        });
    }
}
