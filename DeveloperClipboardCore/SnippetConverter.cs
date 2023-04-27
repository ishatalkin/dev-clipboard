using Flurl;
using Flurl.Http;

namespace DeveloperClipboardCore;

public class ConvertRequest
{
    public string Input { get; set; }
    public string OperationId { get; set; }
    public Settings Settings { get; set; }
}

public class OperationIds
{
    public const string Json2Csharp = "jsontocsharp";
    public const string Json2Java = "jsontopojo";
    public const string Json2Python = "jsontopython";
}

public class Settings
{
    public bool UsePascalCase { get; set; }
    public bool UseFields { get; set; }
    public bool AlwaysUseNullables { get; set; }
    public bool UseJsonAttributes { get; set; }
    public bool NullValueHandlingIgnore { get; set; }
    public bool UseJsonPropertyName { get; set; }
    public bool ImmutableClasses { get; set; }
    public bool RecordTypes { get; set; }
    public bool NoSettersForCollections { get; set; }
}

public class SnippetConverter
{
    public Task<ConvertionResult> JsonToCsharp(string json)
    {
        return JsonTo(json, OperationIds.Json2Csharp);
    }

    public Task<ConvertionResult> JsonToJava(string json)
    {
        return JsonTo(json, OperationIds.Json2Java);
    }

    public Task<ConvertionResult> JsonToPython(string json)
    {
        return JsonTo(json, OperationIds.Json2Python);
    }

    private static async Task<ConvertionResult> JsonTo(string json, string operationId)
    {
        // Используем https://json2csharp.com/
        var converted = await "https://json2csharp.com/api/Default"
            .PostJsonAsync(new ConvertRequest
            {
                Input = json,
                OperationId = operationId,
                Settings = new()
            })
            .ReceiveJson<string>();

        return new(converted, converted.StartsWith("Exception: Invalid character after") ? ConvertionState.NothingToConvert : ConvertionState.Ok);
    }
}