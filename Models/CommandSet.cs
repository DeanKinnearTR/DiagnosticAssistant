using System.Text.Json.Serialization;

namespace DiagnosticAssistant.Models;

public class CommandSet
{
    [JsonPropertyName("commands")]
    public List<string> Commands { get; set; } = [];

    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    [JsonPropertyName("complete")]
    public bool Complete { get; set; }
}