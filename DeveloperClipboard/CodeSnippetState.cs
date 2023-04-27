namespace DeveloperClipboard;

public enum CodeSnippetState
{
    /// <summary> Новый кусок кода </summary>
    New,

    /// <summary> Код успешно сконвертирован </summary>
    Converted,

    /// <summary> Нечего конвертировать </summary>
    NothingToConvert
}