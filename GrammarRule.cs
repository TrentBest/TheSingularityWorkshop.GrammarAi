namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Defines one production: a nonterminal expands to an ordered sequence of grammar symbols.
/// </summary>
public sealed record GrammarRule
{
    public GrammarRule(ulong id, ulong leftHandSide, IEnumerable<GrammarSymbol> rightHandSide)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (leftHandSide == 0) throw new ArgumentOutOfRangeException(nameof(leftHandSide));
        ArgumentNullException.ThrowIfNull(rightHandSide);

        Id = id;
        LeftHandSide = leftHandSide;
        RightHandSide = Array.AsReadOnly(rightHandSide.ToArray());
    }

    public ulong Id { get; }
    public ulong LeftHandSide { get; }
    public IReadOnlyList<GrammarSymbol> RightHandSide { get; }
}