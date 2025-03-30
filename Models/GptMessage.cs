using System.Text.Json.Serialization;

namespace DiagnosticAssistant.Models;

public class GptMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Content { get; set; } = string.Empty;

    public GptMessage() { }

    public GptMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }

    public static GptMessage System(string content) => new GptMessage("system", content);
    public static GptMessage User(string content) => new GptMessage("user", content);
    public static GptMessage Assistant(string content) => new GptMessage("assistant", content);

    public override string ToString() => $"[{Role}] {Content}";
}