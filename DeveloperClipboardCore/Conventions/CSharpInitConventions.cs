using System.Text.RegularExpressions;

namespace DeveloperClipboardCore.Conventions;

public class CSharpInitConventions
{
    public static readonly ConvertionInfo[] Conventions =
    {
        new(@"///.*", ""),
        new(@"\[.+\][\r\n]*", ""), // атрибуты
        new("public .*? ", ""),
        new(@"\{.*", "= ,"),
        new(@"[\r\n]+[\r\n\s]+", "\r\n"), // удалим множественные переносы строк
    };
}