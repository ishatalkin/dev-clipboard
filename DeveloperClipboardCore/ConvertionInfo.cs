using System.Text.RegularExpressions;

namespace DeveloperClipboardCore;

public record ConvertionInfo(string Find, string? Replace = null)
{
    public MatchEvaluator? MatchEvaluator { get; set; }

    /// <summary> Применять преобразования только для данной подстроки </summary>
    public string? For { get; set; }

    public ConvertionInfo ForSubstring(string str)
    {
        For = str;
        return this;
    }
    
    public ConvertionInfo WithEvaluator(MatchEvaluator evaluator)
    {
        MatchEvaluator = evaluator; 
        return this;
    }
}