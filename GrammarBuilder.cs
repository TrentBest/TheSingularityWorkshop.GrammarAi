namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Builds grammar rules and publishes an immutable <see cref="GrammarDefinition"/> snapshot.
/// </summary>
public sealed class GrammarBuilder
{
    private readonly List<GrammarRule> _rules = [];

    /// <summary>Creates a grammar builder.</summary>
    /// <param name="id">Nonzero identity of this grammar.</param>
    /// <param name="name">Human-readable grammar name.</param>
    /// <param name="startSymbol">Nonzero identifier of the start nonterminal.</param>
    /// <exception cref="ArgumentOutOfRangeException">An identifier is zero.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is null, empty, or whitespace.</exception>
    public GrammarBuilder(ulong id, string name, ulong startSymbol)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A grammar name is required.", nameof(name));
        if (name.Length > GrammarLimits.MaxNameCharacters) throw new ArgumentException($"A grammar name cannot exceed {GrammarLimits.MaxNameCharacters} characters.", nameof(name));
        if (startSymbol == 0) throw new ArgumentOutOfRangeException(nameof(startSymbol));

        Id = id;
        Name = name;
        StartSymbol = startSymbol;
    }

    /// <summary>Gets the grammar's nonzero identity.</summary>
    public ulong Id { get; }

    /// <summary>Gets the grammar's human-readable name.</summary>
    public string Name { get; }

    /// <summary>Gets the nonzero identifier of the grammar's start nonterminal.</summary>
    public ulong StartSymbol { get; }

    /// <summary>Adds a production rule to this builder.</summary>
    /// <param name="ruleId">Nonzero identity unique within the built grammar.</param>
    /// <param name="leftHandSide">Nonzero identifier of the rule's left-hand nonterminal.</param>
    /// <param name="rightHandSide">Ordered symbols produced by the rule. An empty sequence represents epsilon.</param>
    /// <returns>This builder, allowing chained rule declarations.</returns>
    /// <exception cref="ArgumentOutOfRangeException">A rule or left-hand-side identifier is zero.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="rightHandSide"/> is null.</exception>
    public GrammarBuilder Rule(ulong ruleId, ulong leftHandSide, params GrammarSymbol[] rightHandSide)
    {
        if (_rules.Count >= GrammarLimits.MaxRules) throw new InvalidOperationException($"A grammar cannot contain more than {GrammarLimits.MaxRules} rules.");
        _rules.Add(new GrammarRule(ruleId, leftHandSide, rightHandSide));
        return this;
    }

    /// <summary>Validates the accumulated rules and creates an immutable grammar snapshot.</summary>
    /// <returns>A grammar definition independent of later builder changes.</returns>
    /// <exception cref="ArgumentException">Rule identifiers are duplicated or a referenced nonterminal has no production.</exception>
    public GrammarDefinition Build() => new(Id, Name, StartSymbol, _rules);
}
