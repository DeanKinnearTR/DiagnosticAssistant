using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DiagnosticAssistant.Models;

namespace DiagnosticAssistant;

public class ChatGptClient : IGptClient, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _model;
    private bool _disposed;

    public ChatGptClient(string apiKey, GptModel model = GptModel.Gpt35Turbo)
    {
        _model = model.ToModelString();

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public async Task<string?> SendMessagesAsync(List<GptMessage> messages)
    {
        var requestBody = new
        {
            model = _model, messages
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"GPT API Error: {response.StatusCode} - {responseString}");

        using var doc = JsonDocument.Parse(responseString);
        return doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
    }
}