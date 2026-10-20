namespace VaultCode.Models;

public class HackerAgent : User
{
    public string Codename { get; set; } = "Cipher_99";
    public ClearanceLevel ClearanceLevel { get; set; } = ClearanceLevel.Operative;
    public int TotalBreaches { get; set; } = 3;
    public int WinStreak { get; set; } = 3;
    public int TotalAttempts { get; set; } = 4;
    public string AvatarIcon { get; set; } = "fa-user-ninja";

    public double SuccessRate => TotalAttempts == 0
        ? 0.0
        : Math.Round((double)TotalBreaches / TotalAttempts * 100.0, 1);

    public HackerAgent()
    {
        Role = "Vault Infiltrator";
    }
}
