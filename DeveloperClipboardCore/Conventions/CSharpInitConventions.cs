using System.Text.RegularExpressions;

namespace DeveloperClipboardCore.Conventions;

public class CSharpInitConventions
{
    public static readonly ConvertionInfo[] Conventions =
    {
        new(@"///.*", ""),
        new(@"\[.+\][\r\n]*", ""), // атрибуты
        new(@"<.+>", ""), // аргументы дженериков
        new("public .*? ([A-Za-z_])", "$1"), //$1 - первый символ проперти
        new(@"\{.*", "= ,"),
        new(@"^[\s]+^", ""), // удалим множественные переносы строк
        new(@"[ \t][ \t]+", ""), // удалим множественные пробелы / табуляции
    };
}