namespace DiagnosticAssistant;

internal class Program
{
    private static async Task Main()
    {
        Console.WriteLine("=== PC Diagnostic Assistant ===");
        Console.Write("Describe the computer issue: ");
        var userInput = Console.ReadLine();
        if (string.IsNullOrEmpty(userInput)) return;

        var session = new DiagnosticSession(userInput);
        await session.RunAsync();
    }
}