using System;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;

namespace Manager.GUI.Services.Nginx;

public class NginxCompletionData : ICompletionData
{
    private readonly string _keyword;

    public NginxCompletionData(string text)
    {
        _keyword = text;
    }

    public IImage? Image
    {
        get
        {
            if (_keyword.StartsWith("$"))
            {
                return NginxIcons.Variable;
            }
            return NginxIcons.Directive;
        }
    }

    public string Text => _keyword;

    // The list content
    public object Content => _keyword;

    // The popup content (Uses the rich color-coded UI we created)
    public object Description => NginxDocumentation.GetTooltip(_keyword);

    public double Priority => 0;

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        textArea.Document.Replace(completionSegment, Text);
    }
}