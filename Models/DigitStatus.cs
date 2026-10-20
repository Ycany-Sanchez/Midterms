namespace VaultCode.Models;

public enum DigitStatus
{
    Correct,     // Correct digit, correct position (Emerald)
    Misplaced,   // Digit exists in code, wrong position (Amber)
    Wrong        // Digit not in passcode (Slate/Muted)
}
