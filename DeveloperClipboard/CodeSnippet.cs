using System.ComponentModel;

namespace DeveloperClipboard;

public class CodeSnippet : INotifyPropertyChanged
{
    private string _description;

    // a property.
    public string Description
    {
        get => _description;
        set
        {
            if(_description!= value)
            {
                _description = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs( nameof(Description)));
            }
        }
    }
    
    private CodeSnippetState _state = CodeSnippetState.New;

    // a property.
    public CodeSnippetState State
    {
        get => _state;
        set
        {
            if(_state!= value)
            {
                _state = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs( nameof(State)));
            }
        }
    }
    
    private string? _code;

    // a property.
    public string? Code
    {
        get => _code;
        set
        {
            if(_code!= value)
            {
                _code = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs( nameof(Code)));
            }
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
}