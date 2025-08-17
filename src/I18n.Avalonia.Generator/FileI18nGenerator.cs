using I18n.Avalonia.Generator.Primitives;
using Microsoft.CodeAnalysis;

namespace I18n.Avalonia.Generator;

[Generator(LanguageNames.CSharp)]
internal class FileI18nGenerator : AbsFileAttributeDetectGenerator
{
    public override string AttributeName
    {
        get => Const.FileKeysAttributeFullName;
    }
}
