namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Defines one production: a nonterminal expands to an ordered sequence of grammar symbols.
/// </summary>
public sealed record GrammarRule
{
    /// <summary>Creates an immutable production rule.</summary>
    /// <param name="id">Nonzero identity of this rule.</param>
    /// <param name="leftHandSide">Nonzero identifier of the nonterminal being expanded.</param>
    /// <param name="rightHandSide">Ordered symbols produced by the rule; empty means epsilon.</param>
    /// <exception cref="ArgumentOutOfRangeException">The rule or left-hand-side identifier is zero.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="rightHandSide"/> is null.</exception>
    public GrammarRule(ulong id, ulong leftHandSide, IEnumerable<GrammarSymbol> rightHandSide)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (leftHandSide == 0) throw new ArgumentOutOfRangeException(nameof(leftHandSide));
        ArgumentNullException.ThrowIfNull(rightHandSide);

        Id = id;
        LeftHandSide = leftHandSide;
        RightHandSide = Array.AsReadOnly(rightHandSide.ToArray());
    }

    /// <summary>Gets the rule's nonzero identity.</summary>
    public ulong Id { get; }

    /// <summary>Gets the nonzero identifier of the nonterminal on the left-hand side.</summary>
    public ulong LeftHandSide { get; }

    /// <summary>Gets the immutable, ordered sequence of produced symbols. An empty sequence represents epsilon.</summary>
    public IReadOnlyList<GrammarSymbol> RightHandSide { get; }
}
