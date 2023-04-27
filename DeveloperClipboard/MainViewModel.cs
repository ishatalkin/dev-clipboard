namespace DeveloperClipboard;

public class MainViewModel
{
    public SnippetCollection Snippets {get; } = new SnippetCollection();
    
    public CodeSnippet? Selected { get; set; }
    
    public string? CurrentText { get; set; }
}