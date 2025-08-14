using Microsoft.CodeAnalysis.CSharp;

namespace I18n.Avalonia.Generator.Primitives;

public static class IdentifierValidator
{
    /// <summary>
    /// 检查标识符是否合法。
    /// 可选：进一步检查 Unicode 标识符扩展规则（如允许 Unicode 字符）
    /// 参考：https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/char
    /// </summary>
    public static bool IsValidIdentifier(string identifier)
    {
        if (string.IsNullOrEmpty(identifier))
            return false;

        // 1. 检查是否是 C# 关键字
        if (SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None)
            return false;

        // 2. 尝试解析标识符，如果能成功解析则合法
        try
        {
            return SyntaxFacts.IsValidIdentifier(identifier);
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
