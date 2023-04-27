using System.Collections.ObjectModel;

namespace DeveloperClipboard;

public class SnippetCollection : ObservableCollection<CodeSnippet>
{
    public CodeSnippet Add(string description)
    {
        var codeSnippet = new CodeSnippet { Description = description };
        this.Add(codeSnippet);
        return codeSnippet;
    }
    
    public CodeSnippet AddConverted(string description, string code)
    {
        var codeSnippet = new CodeSnippet { Description = description, Code = code, State = CodeSnippetState.Converted};
        this.Add(codeSnippet);
        return codeSnippet;
    }

}