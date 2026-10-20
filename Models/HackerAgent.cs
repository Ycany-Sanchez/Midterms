namespace VaultCode.Models;

public class HackerAgent : User
{
    public string Codename { get; set; } = "Cipher_99";
    public ClearanceLevel ClearanceLevel { get; set; } = ClearanceLevel.Operative;
    public int TotalBreaches { get; set; } = 0;
    public int WinStreak { get; set; } = 0;
    public int TotalAttempts { get; set; } = 0;
    public string AvatarIcon { get; set; } = "fa-user-ninja";
    public int BestScore { get; set; } = 0;
    public int FewestAttempts { get; set; } = 0;

    public double SuccessRate => TotalAttempts == 0
        ? 0.0
        : Math.Round((double)TotalBreaches / TotalAttempts * 100.0, 1);

    // Dynamic OOP Clearance Rank Progression
    public string ClearanceRank => TotalBreaches switch
    {
        >= 10 => "Vault Phantom",
        >= 6 => "Neural Hacker",
        >= 3 => "Ghost Operative",
        _ => "Rookie Probe"
    };

    public string RankIcon => TotalBreaches switch
    {
        >= 10 => "fa-crown",
        >= 6 => "fa-brain",
        >= 3 => "fa-ghost",
        _ => "fa-id-badge"
    };

    public string RankBadgeColor => TotalBreaches switch
    {
        >= 10 => "border-rose-500/70 text-rose-300 bg-rose-500/20 shadow-[0_0_12px_rgba(244,63,94,0.4)]",
        >= 6 => "border-amber-500/70 text-amber-300 bg-amber-500/20 shadow-[0_0_12px_rgba(245,158,11,0.4)]",
        >= 3 => "border-cyan-500/70 text-cyan-300 bg-cyan-500/20 shadow-[0_0_12px_rgba(6,182,212,0.4)]",
        _ => "border-slate-500/50 text-slate-400 bg-white/5"
    };

    public string NextRankRequirement => TotalBreaches switch
    {
        < 3 => $"{3 - TotalBreaches} breach(es) to Ghost Operative",
        < 6 => $"{6 - TotalBreaches} breach(es) to Neural Hacker",
        < 10 => $"{10 - TotalBreaches} breach(es) to Vault Phantom",
        _ => "Apex Status Achieved"
    };

    public HackerAgent()
    {
        Role = "Vault Infiltrator";
    }
}
