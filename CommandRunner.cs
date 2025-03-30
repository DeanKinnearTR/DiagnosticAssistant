using System.Diagnostics;

namespace DiagnosticAssistant;

public static class CommandRunner
{
    public static string RunPowerShell(string command)
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            return string.IsNullOrWhiteSpace(error) ? output : output + "\nERROR:\n" + error;
        }
        catch (Exception ex)
        {
            return $"Error running PowerShell command: {ex.Message}";
        }
    }
}