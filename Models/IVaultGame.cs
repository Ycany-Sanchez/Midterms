namespace VaultCode.Models;

public interface IVaultGame
{
    int CodeLength { get; }
    ClearanceLevel ClearanceLevel { get; }
    int MaxAttempts { get; }
    List<CodeAttempt> Attempts { get; }
    int CurrentAttempt { get; }
    bool IsSolved { get; }
    bool IsGameOver { get; }
    string RevealedCode { get; }

    CodeAttempt SubmitGuess(string guess);
}