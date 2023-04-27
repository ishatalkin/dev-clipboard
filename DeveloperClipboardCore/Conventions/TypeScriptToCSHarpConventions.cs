namespace DeveloperClipboardCore.Conventions;

public static class TypeScriptToCSharpConventions
{
    private static readonly ConvertionInfo[] TypeMapping = new[]
    {
        new ConvertionInfo("number", "int"),
        new ConvertionInfo("boolean", "bool"),
    };

    public static readonly ConvertionInfo[] Convertions = new[]
    {
        new ConvertionInfo("export interface", "public class"),
        // /** ... */ -> /// <summary> ... </summary>
        new ConvertionInfo(@"\/\*\*(.*)\*\/", "/// <summary>$1</summary>"),
        // fieldName: string; -> public string FieldName { get; set; }
        new ConvertionInfo(@"\b((?!\d)[\w$]+)\b: (.*);")
        {
            MatchEvaluator = (match =>
            {
                var propName = match.Groups[1].Value;
                var propTypeRaw = match.Groups[2].Value;

                var propTypes = propTypeRaw.Split("|").Select(x => x.Trim()).ToHashSet();

                var nullable = false;
                if (propTypes.Contains("null") || propTypes.Contains("undefined"))
                {
                    nullable = true;
                    propTypes.Remove("null");
                    propTypes.Remove("undefined");
                }

                var propType = string.Join(" | ", propTypes);

                //type: 'course'; -> public string Type { get; set; } = "course";
                string defaultValue = "default!";
                if (propType.StartsWith("'") && propType.EndsWith("'"))
                {
                    defaultValue = propType.Replace("'", "\"");
                    propType = "string";
                }

                foreach (var typeMapping in TypeMapping)
                {
                    if (propType == typeMapping.Find)
                    {
                        propType = typeMapping.Replace;
                        break;
                    }
                }

                if (nullable)
                {
                    propType += "?";
                }

                return ToProperty(propType, propName, defaultValue);
            })
        },
        // export type TermType = 'a' | 'b' | 'c'; -> public enum { a, b, c }
        new ConvertionInfo(@"export type \b((?!\d)[\w$]+)\b = (.*);")
        {
            MatchEvaluator = (match =>
            {
                var enumName = match.Groups[1].Value;
                var values = match.Groups[2].Value;
                values = values.Replace("|", ",").Replace("'", String.Empty).Replace("typeof ", String.Empty);
                return $"public enum {enumName} {{ {values} }}";
            })
        },
    };

    private static string ToProperty(string propType, string propName, string? defaultValue)
    {
        if (propName.Length == 1)
        {
            propName = propName.ToUpperInvariant();
        }
        else
        {
            // fieldName -> FieldName
            propName = propName.Substring(0, 1).ToUpperInvariant() + propName.Substring(1);
        }


        var property = $"public {propType} {propName} {{ get; set; }}";

        if (defaultValue != null)
        {
            property += $" = {defaultValue};";
        }

        return property;
    }
}