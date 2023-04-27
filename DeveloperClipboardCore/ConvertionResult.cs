namespace DeveloperClipboardCore;

public enum ConvertionState
{
    Ok,
    NothingToConvert
}

public record ConvertionResult(string Code, ConvertionState State);