public static class RomanCurrency
{
    // Game-friendly Roman currency display.
    // The actual game still stores only denarii in PlayerStats.
    public const int DenariiPerAureus = 25;
    public const int SestertiiPerDenarius = 4;
    public const int AssesPerSestertius = 4;

    public static string FormatDenarii(int denarii)
    {
        int remaining = denarii;

        int aurei = remaining / DenariiPerAureus;
        remaining %= DenariiPerAureus;

        int sestertii = remaining * SestertiiPerDenarius;
        int asses = sestertii * AssesPerSestertius;

        if (aurei > 0)
            return $"{denarii} denarii ({aurei} aurei + {remaining} denarii)";

        if (denarii > 0)
            return $"{denarii} denarii ({sestertii} sestertii / {asses} asses)";

        return "Free";
    }
}
