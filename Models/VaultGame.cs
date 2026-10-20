namespace VaultCode.Models;

public class VaultGame
{
    private readonly string _secretCode;

    public int CodeLength { get; }
    public ClearanceLevel ClearanceLevel { get; }
    public int MaxAttempts { get; } = 6;
    public List<CodeAttempt> Attempts { get; } = new();

    public int CurrentAttempt => Math.Min(Attempts.Count + 1, MaxAttempts);
    public bool IsSolved => Attempts.Any(a => a.Statuses.Count > 0 && a.Statuses.All(s => s == DigitStatus.Correct));
    public bool IsGameOver => IsSolved || Attempts.Count >= MaxAttempts;

    // Secret passcode is encapsulated; revealed only upon game over
    public string RevealedCode => IsGameOver ? _secretCode : new string('•', CodeLength);

    public VaultGame(ClearanceLevel level = ClearanceLevel.Operative, string? customCode = null)
    {
        ClearanceLevel = level;
        CodeLength = level == ClearanceLevel.Mastermind ? 5 : 4;

        if (!string.IsNullOrWhiteSpace(customCode) && customCode.Length == CodeLength)
        {
            _secretCode = customCode;
        }
        else
        {
            _secretCode = GenerateRandomPasscode(CodeLength);
        }
    }

    private static string GenerateRandomPasscode(int length)
    {
        var random = new Random();
        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = (char)('0' + random.Next(0, 10));
        }
        return new string(chars);
    }

    public CodeAttempt SubmitGuess(string guess)
    {
        if (IsGameOver)
        {
            throw new InvalidOperationException("Cannot submit guess; game is already over.");
        }

        if (string.IsNullOrWhiteSpace(guess) || guess.Length != CodeLength)
        {
            throw new ArgumentException($"Guess must be exactly {CodeLength} digits.", nameof(guess));
        }

        var statuses = EvaluateGuess(guess, _secretCode);
        var attempt = new CodeAttempt(guess, statuses);
        Attempts.Add(attempt);
        return attempt;
    }

    private static List<DigitStatus> EvaluateGuess(string guess, string secret)
    {
        int length = secret.Length;
        var statuses = new DigitStatus[length];
        var secretMatched = new bool[length];
        var guessMatched = new bool[length];

        // Pass 1: exact matches (Correct position and digit)
        for (int i = 0; i < length; i++)
        {
            if (guess[i] == secret[i])
            {
                statuses[i] = DigitStatus.Correct;
                secretMatched[i] = true;
                guessMatched[i] = true;
            }
        }

        // Pass 2: misplaced matches (Exists in secret but different position)
        for (int i = 0; i < length; i++)
        {
            if (guessMatched[i]) continue;

            for (int j = 0; j < length; j++)
            {
                if (!secretMatched[j] && guess[i] == secret[j])
                {
                    statuses[i] = DigitStatus.Misplaced;
                    secretMatched[j] = true;
                    guessMatched[i] = true;
                    break;
                }
            }

            if (!guessMatched[i])
            {
                statuses[i] = DigitStatus.Wrong;
            }
        }

        return statuses.ToList();
    }
}
