using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using I18n.Avalonia.Sample.I18ns;
using I18n.Avalonia.Tests.Helper;
using Xunit;

namespace I18n.Avalonia.Tests;

public class I18nExtensionTest
{
    [Fact]
    public void KeyPropertyInit()
    {
        TestHelper.Excute(() =>
        {
            I18nProvider.SetCulture(TestHelper.zh);

            var i18n = new I18nExtension() { Key = LangKeys.Chinese };

            Assert.True(string.Equals("中文", i18n.Key.CurrentValue));

            I18nProvider.SetCulture(TestHelper.en);

            Assert.True(string.Equals("Chinese", i18n.Key.CurrentValue));

            I18nProvider.SetCulture(TestHelper.fr);

            Assert.True(string.Equals("Chinois", i18n.Key.CurrentValue));
        });
    }


    [Fact]
    public void Key()
    {
        TestHelper.Excute(() =>
        {
            var testAxaml = @"<TextBlock Name='textBlock' Text='{i18n:I18n Key={x:Static sample:LangKeys.Chinese}}' />";

            var xaml = TestHelper.AxamlFormat.Replace(TestHelper.TestAxamlPlaceholder, testAxaml);


            var userControl = (UserControl)AvaloniaRuntimeXamlLoader.Load(xaml);
            var textBlock = userControl.FindControl<TextBlock>("textBlock");

            Assert.NotNull(textBlock);
            I18nProvider.SetCulture(TestHelper.zh);
            Assert.True(string.Equals("中文", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.en);

            Assert.True(string.Equals("Chinese", textBlock.Text));

            I18nProvider.SetCulture(TestHelper.fr);

            Assert.True(string.Equals("Chinois", textBlock.Text));
        });
    }

    [Fact]
    public void KeyAndArgs()
    {
        TestHelper.Excute(() =>
        {
            var testAxaml = """
                            <TextBlock Name='textBlock' DockPanel.Dock="Top">
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

            Assert.NotNull(textBlock);
            TestHelper.VM.Culture = TestHelper.zh;

            Assert.True(string.Equals("当前的语言是 中文", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.en;

            Assert.True(string.Equals("Current Language is English", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.fr;

            Assert.True(string.Equals("Le Langue actuel est français", textBlock.Text));
        });
    }

    [Fact]
    public void KeyAndValueArgs()
    {
        TestHelper.Excute(() =>
        {
            var testAxaml = """
                            <TextBlock Name='textBlock' DockPanel.Dock="Top">
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

            Assert.NotNull(textBlock);
            TestHelper.VM.Culture = TestHelper.zh;

            Assert.True(string.Equals("1 + 1 = 2", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.en;

            Assert.True(string.Equals("1 + 1 = 2", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.fr;

            Assert.True(string.Equals("1 + 1 = 2", textBlock.Text));
        });
    }

    [Fact]
    public void BindingKey()
    {
        TestHelper.Excute(() =>
        {
            var testAxaml = """
                            <TextBlock x:Name="textBlock" Text="{Binding Language.Value^}" DockPanel.Dock="Top" />
                            """;

            var xaml = TestHelper.AxamlFormat.Replace(TestHelper.TestAxamlPlaceholder, testAxaml);

            var userControl = (UserControl)AvaloniaRuntimeXamlLoader.Load(xaml);
            
            userControl.DataContext = TestHelper.VM;

            var textBlock = userControl.FindControl<TextBlock>("textBlock");

            Assert.NotNull(textBlock);
            TestHelper.VM.Culture = TestHelper.zh;

            Assert.True(string.Equals("中文", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.en;

            Assert.True(string.Equals("Chinese", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.fr;

            Assert.True(string.Equals("Chinois", textBlock.Text));
        });
    }
}
