public static class RomanCurrency
{
    // Gameplay still stores only denarii.
    // These conversions are display-only so the shop can teach Roman currency.
    public const int DenariiPerAureus = 25;
    public const int SestertiiPerDenarius = 4;
    public const int AssesPerSestertius = 4;

    public static string FormatDenarii(int denarii)
    {
        if (denarii <= 0)
            return "Free";

        int aurei = denarii / DenariiPerAureus;
        int remainingDenarii = denarii % DenariiPerAureus;
        int sestertii = denarii * SestertiiPerDenarius;
        int asses = sestertii * AssesPerSestertius;

        if (aurei > 0)
            return $"{denarii} denarii ({aurei} aurei + {remainingDenarii} denarii)";

        return $"{denarii} denarii ({sestertii} sestertii / {asses} asses)";
    }
}
