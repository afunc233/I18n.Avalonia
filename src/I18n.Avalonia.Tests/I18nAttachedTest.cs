using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using I18n.Avalonia.Sample.I18ns;
using I18n.Avalonia.Tests.Helper;
using Xunit;

namespace I18n.Avalonia.Tests;

public class I18nAttachedTest
{
    [Fact]
    public void TestI18nAttachedWithArgs()
    {
        TestHelper.Excute(() =>
        {
            const string testAxaml = """
                                     <TextBlock Name='textBlock'
                                                i18n:I18nAttached.Key="{Binding Current_language_is}"
                                                i18n:I18nAttached.Args="{Binding Args}"
                                                i18n:I18nAttached.TextProperty="{x:Static TextBlock.TextProperty}">
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
    public void TestI18nAttachedWithAxamlArgs()
    {
        TestHelper.Excute(() =>
        {
            const string testAxaml = """
                                     <TextBlock Name='textBlock'
                                                i18n:I18nAttached.Key="{Binding Addition_formula_2}"
                                                i18n:I18nAttached.Args="{Binding (i18n:I18nAttached.Args),RelativeSource={RelativeSource Mode=Self}}"
                                                i18n:I18nAttached.TextProperty="{x:Static TextBlock.TextProperty}">
                                         <i18n:I18nAttached.Args>
                                             <sys:Int32>1</sys:Int32>
                                             <sys:Int32>1</sys:Int32>
                                             <sys:Int32>2</sys:Int32>
                                         </i18n:I18nAttached.Args>
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
    public void TestI18nAttachedWithCode()
    {
        TestHelper.Excute(() =>
        {
            var textBlock = new TextBlock();

            I18nAttached.SetKey(textBlock, LangKeys.Addition_formula_2);
            I18nAttached.SetTextProperty(textBlock, TextBlock.TextProperty);
            I18nAttached.SetArgs(textBlock, [1, 1, 2]);

            TestHelper.VM.Culture = TestHelper.zh;

            Assert.True(string.Equals("1 + 1 = 2", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.en;

            Assert.True(string.Equals("1 + 1 = 2", textBlock.Text));

            TestHelper.VM.Culture = TestHelper.fr;

            Assert.True(string.Equals("1 + 1 = 2", textBlock.Text));
        });
    }
}
