namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Resource limits for grammar definitions and production rules.</summary>
public static class GrammarLimits
{
    /// <summary>Maximum number of production rules in one grammar.</summary>
    public const int MaxRules = 4_096;
    /// <summary>Maximum number of symbols in one production's right-hand side.</summary>
    public const int MaxSymbolsPerRule = 1_024;
    /// <summary>Maximum aggregate number of right-hand-side symbols in one grammar.</summary>
    public const int MaxTotalSymbols = 65_536;
    /// <summary>Maximum grammar name length, in UTF-16 characters.</summary>
    public const int MaxNameCharacters = 256;
}
