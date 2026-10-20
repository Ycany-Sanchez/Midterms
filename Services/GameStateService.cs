using VaultCode.Models;

namespace VaultCode.Services;

public class HeistRecord
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..6];
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public ClearanceLevel Clearance { get; set; }
    public bool Won { get; set; }
    public int AttemptsUsed { get; set; }
    public string SecretCode { get; set; } = string.Empty;
    public int Score { get; set; } = 0;
}

public class GameStateService
{
    public HackerAgent CurrentAgent { get; private set; } = new HackerAgent
    {
        FirstName = "Alex",
        LastName = "Mercer",
        Codename = "Cipher_99",
        Email = "agent@vaultcode.net",
        ClearanceLevel = ClearanceLevel.Operative,
        TotalBreaches = 0,
        TotalAttempts = 0,
        WinStreak = 0,
        AvatarIcon = "fa-user-ninja",
        BestScore = 0,
        FewestAttempts = 0
    };

    public IVaultGame ActiveGame { get; private set; }

    public List<HeistRecord> RecentHeists { get; } = new();

    public double AverageAttemptsPerWin => RecentHeists.Where(h => h.Won).Any()
        ? Math.Round(RecentHeists.Where(h => h.Won).Average(h => h.AttemptsUsed), 1)
        : 0.0;

    public IEnumerable<HeistRecord> LastFiveHeists => RecentHeists.Take(5);

    public event Action? OnChange;

    public GameStateService()
    {
        ActiveGame = new VaultGame(CurrentAgent.ClearanceLevel);
    }

    public void StartNewGame(ClearanceLevel? level = null)
    {
        var targetLevel = level ?? CurrentAgent.ClearanceLevel;
        ActiveGame = new VaultGame(targetLevel);
        NotifyStateChanged();
    }

    public CodeAttempt SubmitActiveGuess(string guess, int secondsRemaining = 0)
    {
        var attempt = ActiveGame.SubmitGuess(guess);

        if (ActiveGame.IsGameOver)
        {
            CurrentAgent.TotalAttempts++;
            int runScore = 0;

            if (ActiveGame.IsSolved)
            {
                CurrentAgent.TotalBreaches++;
                CurrentAgent.WinStreak++;

                // Score formula: (unused attempts bonus) + (time remaining bonus) + clearance difficulty multiplier
                int unusedAttempts = ActiveGame.MaxAttempts - ActiveGame.Attempts.Count;
                int difficultyBonus = ActiveGame.ClearanceLevel == ClearanceLevel.Mastermind ? 500 : 250;
                runScore = difficultyBonus + (unusedAttempts * 200) + (secondsRemaining * 30);

                if (runScore > CurrentAgent.BestScore)
                {
                    CurrentAgent.BestScore = runScore;
                }

                if (CurrentAgent.FewestAttempts == 0 || ActiveGame.Attempts.Count < CurrentAgent.FewestAttempts)
                {
                    CurrentAgent.FewestAttempts = ActiveGame.Attempts.Count;
                }
            }
            else
            {
                CurrentAgent.WinStreak = 0;
            }

            RecentHeists.Insert(0, new HeistRecord
            {
                Clearance = ActiveGame.ClearanceLevel,
                Won = ActiveGame.IsSolved,
                AttemptsUsed = ActiveGame.Attempts.Count,
                SecretCode = ActiveGame.RevealedCode,
                Score = runScore
            });
        }

        NotifyStateChanged();
        return attempt;
    }

    public void UpdateAgent(string codename, ClearanceLevel clearance, string avatarIcon, string? firstName = null, string? lastName = null, string? email = null)
    {
        if (!string.IsNullOrWhiteSpace(codename))
            CurrentAgent.Codename = codename.Trim();

        CurrentAgent.ClearanceLevel = clearance;

        if (!string.IsNullOrWhiteSpace(avatarIcon))
            CurrentAgent.AvatarIcon = avatarIcon;

        if (!string.IsNullOrWhiteSpace(firstName))
            CurrentAgent.FirstName = firstName.Trim();

        if (!string.IsNullOrWhiteSpace(lastName))
            CurrentAgent.LastName = lastName.Trim();

        if (!string.IsNullOrWhiteSpace(email))
            CurrentAgent.Email = email.Trim();

        StartNewGame(clearance);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
