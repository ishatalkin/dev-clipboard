using System.Text.RegularExpressions;

namespace DeveloperClipboardCore.Conventions;

public class DdlToCSharpConventions
{
    public static readonly ConvertionInfo[] Conventions =
    {
        // date_from -> dateFrom
        new ConvertionInfo("_(.)").WithEvaluator(x => $"{x.Groups[1].Value.ToUpperInvariant()}"),
        // dateFrom -> DateFrom
        new ConvertionInfo(@"\b(.)").WithEvaluator(x => $"{x.Groups[1].Value.ToUpperInvariant()}"),
        // "Name" -> Name
        new("\"", ""),

        // id varchar(100) NOT NULL, -- Идентификатор -> /// <summary> Идентификатор </summary>\r\n id varchar(100) NOT NULL,
        new(@"^(.*)--(.*)[\r\n]+$", "/// <summary> $2 </summary>\r\n$1"),

        new("CREATE TABLE", "public class"),
        new("CONSTRAINT.*", ""),
        // Id Varchar{100) NOT NULL -> public Varchar{100) NOT NULL Id { get; set; } = default!;
        new(@"\b(.+?) (.+),[ \r\n]+$", "public $2 $1 { get; set; } = default!;\r\n"),

        new(@" NOT NULL", ""),
        new(@" NULL", "?"),
        
        new(@"\bvarchar\(.*\)", "string"),
        new(@"\bdate\b", "DateTime"),
        new(@"\bint4\b", "int"),
        new(@"\bbool\b", "bool"),
        new(@"\btimestamp\b", "DateTime"),
        new(@"\bnumeric\(.*\)", "decimal"),

        
        new(@"\(", "{"),
        new(@"\);", "}"),
    };
}