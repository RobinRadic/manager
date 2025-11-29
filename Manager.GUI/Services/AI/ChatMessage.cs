using System;

namespace Manager.GUI.Services.AI;

public class ChatMessage
{
    public string Role { get; } // "User" or "Gemini"
    public string Text { get; }
    public DateTime Timestamp { get; }
    public bool IsLoading { get; } 
    public ChatMessage(string role, string text, bool isLoading = false)
    {
        Role = role;
        Text = text;
        Timestamp = DateTime.Now;
        IsLoading = isLoading;
    }
}