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

        var symbols = rightHandSide.Take(GrammarLimits.MaxSymbolsPerRule + 1).ToArray();
        if (symbols.Length > GrammarLimits.MaxSymbolsPerRule)
            throw new ArgumentException($"A production cannot contain more than {GrammarLimits.MaxSymbolsPerRule} symbols.", nameof(rightHandSide));
        if (symbols.Any(symbol => symbol.Id == 0))
            throw new ArgumentException("A production cannot contain an uninitialized grammar symbol.", nameof(rightHandSide));

        Id = id;
        LeftHandSide = leftHandSide;
        RightHandSide = Array.AsReadOnly(symbols);
    }

    /// <summary>Gets the rule's nonzero identity.</summary>
    public ulong Id { get; }

    /// <summary>Gets the nonzero identifier of the nonterminal on the left-hand side.</summary>
    public ulong LeftHandSide { get; }

    /// <summary>Gets the immutable, ordered sequence of produced symbols. An empty sequence represents epsilon.</summary>
    public IReadOnlyList<GrammarSymbol> RightHandSide { get; }
}
