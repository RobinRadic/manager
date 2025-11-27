using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LlmTornado;
using LlmTornado.Chat.Models;
using LlmTornado.Code;
using Manager.GUI.Services.Settings;

namespace Manager.GUI.Services.AI;

public class AiService
{
    private readonly SettingsService _settingsService;

    public AiService(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }
    private async Task<TornadoApi?> GetApiAsync()
    {
        var settings = _settingsService.CurrentSettings;
        if (string.IsNullOrWhiteSpace(settings.AiApiKey)) return null;

        LLmProviders provider = settings.AiProvider switch
        {
            "OpenAI" => LLmProviders.OpenAi,
            "Google" => LLmProviders.Google,
            "Anthropic" => LLmProviders.Anthropic,
            _ => LLmProviders.Google
        };

        return new TornadoApi(new List<ProviderAuthentication> 
        { 
            new ProviderAuthentication(provider, settings.AiApiKey) 
        });
    }
    private string GetModelName()
    {
        var s = _settingsService.CurrentSettings;
        if (!string.IsNullOrWhiteSpace(s.AiModel)) return s.AiModel;
        
        return s.AiProvider switch
        {
            "OpenAI" => "gpt-4o",
            "Google" => "gemini-2.5-flash",
            "Anthropic" => "claude-3-5-sonnet-latest",
            _ => "gemini-1.5-flash"
        };
    }
    
    public async Task<string> ChatAsync(string userMessage, string? systemContext = null)
    {
        var api = await GetApiAsync();
        if (api == null) return "Please configure an API Key in Settings.";

        try
        {
            var conversation = api.Chat.CreateConversation(new ChatModel(GetModelName())); 
            conversation.AppendSystemMessage("You are an expert Nginx Administrator. Be concise.");
            
            if (!string.IsNullOrWhiteSpace(systemContext))
            {
                // conversation.AppendSystemMessage($"CURRENT CONFIG CONTEXT:\n{systemContext}");
                conversation.AppendUserInput($"CURRENT CONFIG CONTEXT:\n{systemContext}\n");
            }

            conversation.AppendUserInput(userMessage);
            var response = await conversation.GetResponseRich();
            return response.Text;
            return await conversation.GetResponse() ?? "No response.";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<string> ExplainCodeAsync(string codeSnippet)
    {
        return await ChatAsync($"Explain this Nginx configuration snippet concisely:\n\n{codeSnippet}");
    }

    public async Task<string> FixCodeAsync(string codeSnippet)
    {
        return await ChatAsync($"Find and fix any errors in this Nginx snippet. Output ONLY the fixed code if possible, or a brief explanation if ambiguous:\n\n{codeSnippet}");
    }
}