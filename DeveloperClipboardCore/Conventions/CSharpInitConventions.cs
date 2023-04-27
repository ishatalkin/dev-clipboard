using System.Text.RegularExpressions;

namespace DeveloperClipboardCore.Conventions;

public class CSharpInitConventions
{
    public static readonly ConvertionInfo[] Conventions =
    {
        new("///.*", ""),
        new("public .*? ", ""),
        new(@"\{.*", "= ,"),
    };
}