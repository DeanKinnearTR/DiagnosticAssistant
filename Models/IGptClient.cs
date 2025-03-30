namespace DiagnosticAssistant.Models;

public interface IGptClient
{
    Task<string?> SendMessagesAsync(List<GptMessage> messages);
}