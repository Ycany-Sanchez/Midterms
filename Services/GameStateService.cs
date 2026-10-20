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
}

public class GameStateService
{
    public HackerAgent CurrentAgent { get; private set; } = new HackerAgent
    {
        FirstName = "Alex",
        LastName = "Mercer",
        Codename = "ZeroDay_77",
        Email = "zeroday@cybergrid.net",
        ClearanceLevel = ClearanceLevel.Operative,
        TotalBreaches = 4,
        TotalAttempts = 5,
        WinStreak = 4,
        AvatarIcon = "fa-user-ninja"
    };

    public VaultGame ActiveGame { get; private set; }

    public List<HeistRecord> RecentHeists { get; } = new()
    {
        new HeistRecord { Timestamp = DateTime.UtcNow.AddMinutes(-24), Clearance = ClearanceLevel.Operative, Won = true, AttemptsUsed = 3, SecretCode = "5091" },
        new HeistRecord { Timestamp = DateTime.UtcNow.AddMinutes(-58), Clearance = ClearanceLevel.Operative, Won = true, AttemptsUsed = 4, SecretCode = "8823" },
        new HeistRecord { Timestamp = DateTime.UtcNow.AddHours(-3), Clearance = ClearanceLevel.Mastermind, Won = true, AttemptsUsed = 5, SecretCode = "19402" },
        new HeistRecord { Timestamp = DateTime.UtcNow.AddHours(-6), Clearance = ClearanceLevel.Mastermind, Won = false, AttemptsUsed = 6, SecretCode = "37180" }
    };

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

    public CodeAttempt SubmitActiveGuess(string guess)
    {
        var attempt = ActiveGame.SubmitGuess(guess);

        if (ActiveGame.IsGameOver)
        {
            CurrentAgent.TotalAttempts++;
            if (ActiveGame.IsSolved)
            {
                CurrentAgent.TotalBreaches++;
                CurrentAgent.WinStreak++;
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
                SecretCode = ActiveGame.RevealedCode
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
