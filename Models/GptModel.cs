namespace DiagnosticAssistant.Models;
// ReSharper disable InconsistentNaming

/// <summary>
/// Supported OpenAI GPT models.
/// </summary>
public enum GptModel
{
    /// <summary>gpt-3.5-turbo (fast and cheap)</summary>
    Gpt35Turbo,

    /// <summary>gpt-4 (more capable, more expensive)</summary>
    Gpt4,

    /// <summary>gpt-4o (fastest and best quality as of 2024)</summary>
    Gpt4o
}


public static class GptModelExtensions
{
    public static string ToModelString(this GptModel model)
    {
        return model switch
        {
            GptModel.Gpt35Turbo => "gpt-3.5-turbo",
            GptModel.Gpt4 => "gpt-4",
            GptModel.Gpt4o => "gpt-4o",
            _ => "gpt-4"
        };
    }

    public static GptModel FromModelString(string modelName)
    {
        return modelName.ToLowerInvariant() switch
        {
            "gpt-3.5-turbo" => GptModel.Gpt35Turbo,
            "gpt-4" => GptModel.Gpt4,
            "gpt-4o" => GptModel.Gpt4o,
            _ => GptModel.Gpt4
        };
    }
}