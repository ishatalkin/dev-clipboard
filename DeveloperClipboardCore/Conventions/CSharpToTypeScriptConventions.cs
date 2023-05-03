using System.Text.RegularExpressions;

namespace DeveloperClipboardCore.Conventions;

public class CSharpToTypeScriptConventions
{
    private static readonly ConvertionInfo[] TypeConventions =
    {
        new("\\bint\\b", "number"),
        new("\\bbool\\b", "boolean"),
        new("\\bDateTime\\b", "Date"),
    };

    public static readonly ConvertionInfo[] TypeToInterfaceConventions = new ConvertionInfo[]
        {
            new("/// <summary>", "/**"),
            new("</summary>", "*/"),
            new(@"\[.+\][\r\n]*", ""), // атрибуты
            new("public class", "export interface"),
            new("public record", "export interface"),
            new("\\bpublic\\b", ""),
            new("{ get; .* }.*", ";"),
        }
        .Concat(TypeConventions)
        .Concat(new[]
        {
            new("\\bint\\b", "number"),
            new("\\bbool\\b", "boolean"),
            new("\\bDateTime\\b", "Date"),

            // boolean Prop -> Prop: boolean;
            new("\\b(.+) (.+);", "$2: $1;"),
            new(@"\?;", " | undefined;"),

            // Prop: -> prop:
            new ConvertionInfo("\\b([A-Z])(.*):")
                .WithEvaluator(x => $"{x.Groups[1].Value.ToLowerInvariant()}{x.Groups[2].Value}:")
        }).ToArray();

    public static readonly ConvertionInfo[] ParamsConventions =
        TypeConventions
            .Concat(new ConvertionInfo[]
            {
                // boolean param, -> param: boolean,
                new(@"\b([^,]+?)\b \b([^,]+?)\b", "$2: $1"),
            }).ToArray();
}