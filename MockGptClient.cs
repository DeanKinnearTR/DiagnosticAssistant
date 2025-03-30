using DiagnosticAssistant.Models;

namespace DiagnosticAssistant;

public class MockGptClient : IGptClient
{
    private int _step;

    public Task<string?> SendMessagesAsync(List<GptMessage> messages)
    {
        var mockReply = _step switch
        {
            0 => """
                 {
                   "commands": [
                     "Get-Process | Sort-Object CPU -Descending | Select-Object -First 5",
                     "Get-CimInstance Win32_Processor | Select Name, LoadPercentage"
                   ],
                   "reason": "Checking CPU usage and load to diagnose slowness.",
                   "complete": false
                 }
                 """,
            1 => """
                 {
                   "commands": [
                     "Get-EventLog -LogName System -Newest 20"
                   ],
                   "reason": "Looking at recent system logs for hardware or OS errors.",
                   "complete": false
                 }
                 """,
            2 => """
                 {
                   "commands": [],
                   "reason": "Based on the logs and CPU usage, the system seems to be overloaded by background apps. Recommend checking startup items and services.",
                   "complete": true
                 }
                 """,
            _ => null
        };

        _step++;
        return Task.FromResult(mockReply);
    }
}
