# I18n.Avalonia

简体中文 | [English](./README.en.md)

一个给 [Avalonia](https://github.com/AvaloniaUI/Avalonia) 做的 I18n 的类库.
包括一个 SourceGenerator 从 resx 文件生成的 Designer 类中生成一个封装器。

## 内容

### I18n.Avalonia

一个给 [Avalonia](https://github.com/AvaloniaUI/Avalonia) 做的 I18n 的类库

#### 特点

- 支持动态切换语言
    - 随时切换当前语言，无须重新启动程序
- 支持插值
    - 支持一个 format 和多种类参数混合，
        - Binding 参数
        - I18n 参数
        - 固定参数

这是一个混用 I18n 和 Binding 作为参数的例子，Key 获取到的文本可以接收两个参数

``` xaml
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
```

由于在 [I18nExtension.cs](./src/I18n.Avalonia/I18nExtension.cs) 里 很难从 Binding 值
计算出 [I18nUnit](./src/I18n.Avalonia/I18nUnit.cs) 作为 key
所以又新加了一个附加属性 [I18nAttached.cs](./src/I18n.Avalonia/I18nAttached.cs) 的用法，本意只是为了 Key 可以 绑定自
ViewModel

``` xaml
    <TextBlock Name='textBlock'
        i18n:I18nAttached.Key="{Binding Current_language_is}"
        i18n:I18nAttached.Args="{Binding Args}"
        i18n:I18nAttached.TextProperty="{x:Static TextBlock.TextProperty}" />
```

附加属性 [I18nAttached.cs](./src/I18n.Avalonia/I18nAttached.cs) 的用法 参数如果是希望 在 Axaml 内设置的话 需要额外多写一句
才能有正常效果，也许 Avalonia 应该优化一下
不过这里本就是为了解决 Binding ViewModel 的 [I18nUnit](./src/I18n.Avalonia/I18nUnit.cs) 值，所以正常情况下 参数 也应该来自
Binding 这里只是记录一下支持的特殊用法

``` xaml
i18n:I18nAttached.Args="{Binding (i18n:I18nAttached.Args),RelativeSource={RelativeSource Mode=Self}}"
```

完整内容

``` xaml
    <TextBlock Name='textBlock'
               i18n:I18nAttached.Key="{Binding Addition_formula_2}"
               i18n:I18nAttached.Args="{Binding (i18n:I18nAttached.Args),RelativeSource={RelativeSource Mode=Self}}"
               i18n:I18nAttached.TextProperty="{x:Static TextBlock.TextProperty}">
        <i18n:I18nAttached.Args>
             <sys:Int32>1</sys:Int32>
             <sys:Int32>1</sys:Int32>
             <sys:String>2</sys:String>
        </i18n:I18nAttached.Args>
    </TextBlock>
```

### I18n.Avalonia.Generator

使用 **public static partial** 类 和 **ResxKeysOfAttribute** 标识 生成类的 命名空间 和 对应的 类名


**要看生成代码的话 需要您先在 IDE 里 生成本项目**

- [ResxKeysGenerator.cs](./src/I18n.Avalonia.Generator/ResxKeysGenerator.cs):
  本项目中生效的 SourceGenerator 首先会生成 ResxKeysOfAttribute 供项目使用，用法可参考 [LangKeys.cs](./src/I18n.Avalonia.Sample/I18ns/LangKeys.cs)

### I18n.Avalonia.Sample

一个 引用 I18n.Avalonia.Generator 的项目. 需要注意项目文件 [I18n.Avalonia.Sample.csproj](./src/I18n.Avalonia.Sample/I18n.Avalonia.Sample.csproj) 里的 `ProjectReference`
, 确保是作为SourceGenerator 引入的 关键点 就在设置 `OutputItemType="Analyzer" ReferenceOutputAssembly="false"`

### I18n.Avalonia.Tests

单元测试,目前比较简单

- [I18nExtensionTest.cs](./src/I18n.Avalonia.Tests/I18nExtensionTest.cs)
  是给 [I18nExtension.cs](./src/I18n.Avalonia/I18nExtension.cs) 做的简单单元测试，也展示了可行的用法

- [I18nAttachedTest.cs](./src/I18n.Avalonia.Tests/I18nAttachedTest.cs)
  是给 [I18nAttached.cs](./src/I18n.Avalonia/I18nAttached.cs) 做的简单单元测试，也展示了可行的用法

## SourceGenerator 深入

### 怎么调试？

- 使用 [launchSettings.json](./src/I18n.Avalonia.Generator/Properties/launchSettings.json) 文件.
- 调试测试.

### 如何确定我应该使用哪些语法节点？

考虑安装 `Roslyn syntax tree viewer` 插件 [Rossynt](https://plugins.jetbrains.com/plugin/16902-rossynt/).

### 如何进一步了解怎么编写 SourceGenerator ？

观看教程
视频: [Let’s Build an Incremental Source Generator With Roslyn, by Stefan Pölz](https://youtu.be/azJm_Y2nbAI)
完整可用的信息在 [Source Generators Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md).
