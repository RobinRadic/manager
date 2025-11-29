using System;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using Avalonia.Media;

namespace Manager.GUI.Services.Php;

public class PhpCompletionData : ICompletionData
{
    private readonly string _text;

    public PhpCompletionData(string text)
    {
        _text = text;
    }

    public IImage? Image => PhpIcons.Key;

    public string Text => _text;
    public object Content => _text;
    public object Description => PhpDocumentation.GetTooltip(_text);
    public double Priority => 0;

    public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
        textArea.Document.Replace(completionSegment, Text + " = ");
    }
}