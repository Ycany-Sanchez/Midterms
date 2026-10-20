namespace VaultCode.Models;

public class CodeAttempt
{
    public string GuessString { get; set; } = string.Empty;
    public List<DigitStatus> Statuses { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public CodeAttempt() { }

    public CodeAttempt(string guessString, List<DigitStatus> statuses)
    {
        GuessString = guessString;
        Statuses = statuses;
        Timestamp = DateTime.UtcNow;
    }
}
