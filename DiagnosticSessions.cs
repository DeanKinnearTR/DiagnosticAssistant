using DiagnosticAssistant.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace DiagnosticAssistant;

public class DiagnosticSession
{
    private readonly List<GptMessage> _messages = [];
    private readonly IGptClient _gpt;

    public DiagnosticSession(string userProblem)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
                     ?? throw new Exception("Missing OPENAI_API_KEY");

        _gpt = new MockGptClient();

        _messages.Add(GptMessage.System(
            """
            You are a Windows PC diagnostic assistant. You are communicating with a C# application that can run PowerShell commands and return the output to you.

            Your responsibilities are:
            - Ask only for PowerShell commands that help diagnose the issue.
            - Respond only in the specified JSON format below.
            - Wait for command results before suggesting fixes or further steps.
            - When you are ready to make a final diagnosis, explain your reasoning and set "complete": true.

            Respond with this JSON structure:
            {
              "commands": ["PowerShell command 1", "PowerShell command 2", "..."],
              "reason": "Why these commands are being run.",
              "complete": false
            }
            """));

        _messages.Add(GptMessage.User("The user says: " + userProblem));
    }

    public async Task RunAsync()
    {
        var complete = false;

        while (!complete)
        {
            var step = _messages.Count(m => m.Role == "assistant") + 1;
            Console.WriteLine($"\n=== Step {step} ===\n");

            var gptReply = await _gpt.SendMessagesAsync(_messages);
            Console.WriteLine("\nGPT Response:\n" + gptReply);

            if (gptReply != null)
                _messages.Add(GptMessage.Assistant(gptReply));

            // Extract JSON block from GPT reply
            var json = Regex.Match(gptReply ?? "", @"\{.*\}", RegexOptions.Singleline).Value;
            if (string.IsNullOrWhiteSpace(json))
            {
                Console.WriteLine("⚠️ Could not extract valid JSON.");
                break;
            }

            CommandSet? commandSet;
            try
            {
                commandSet = JsonSerializer.Deserialize<CommandSet>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Failed to parse GPT response JSON: " + ex.Message);
                break;
            }

            if (commandSet == null)
            {
                Console.WriteLine("❌ GPT response missing commands. Exiting.");
                break;
            }

            Console.WriteLine("\nReason: " + commandSet.Reason);

            var results = new List<CommandResult>();

            foreach (var command in commandSet.Commands)
            {
                Console.WriteLine($"> Running: {command}");
                var output = CommandRunner.RunPowerShell(command);
                results.Add(new CommandResult { Command = command, Output = output });
            }

            var resultJson = JsonSerializer.Serialize(new { results }, new JsonSerializerOptions { WriteIndented = true });
            _messages.Add(GptMessage.User("Here are the results:\n" + resultJson));

            complete = commandSet.Complete;
        }

        Console.WriteLine("\n=== Diagnostic session complete. ===");
    }
}