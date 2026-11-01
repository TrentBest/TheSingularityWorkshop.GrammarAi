using System.Text;

namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Immutable, self-describing grammar that connects protocol symbols through integer-backed production rules.
/// </summary>
/// <remarks>
/// Grammar construction validates the grammar's internal structure, but does not load external protocol
/// definitions. Protocol-terminal references are identity links only; the consumer must resolve them
/// against the corresponding protocol definition before treating a grammar as fully linked.
/// </remarks>
public sealed class GrammarDefinition
{
    private readonly IReadOnlyList<GrammarRule> _rules;
    private readonly HashSet<ulong> _nonTerminals;

    internal GrammarDefinition(ulong id, string name, ulong startSymbol, IEnumerable<GrammarRule> rules)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A grammar name is required.", nameof(name));
        if (name.Length > GrammarLimits.MaxNameCharacters) throw new ArgumentException($"A grammar name cannot exceed {GrammarLimits.MaxNameCharacters} characters.", nameof(name));
        if (startSymbol == 0) throw new ArgumentOutOfRangeException(nameof(startSymbol));

        ArgumentNullException.ThrowIfNull(rules);
        var values = rules.Take(GrammarLimits.MaxRules + 1).ToArray();
        if (values.Length > GrammarLimits.MaxRules)
            throw new ArgumentException($"A grammar cannot contain more than {GrammarLimits.MaxRules} rules.", nameof(rules));
        var totalSymbols = values.Sum(rule => (long)rule.RightHandSide.Count);
        if (totalSymbols > GrammarLimits.MaxTotalSymbols)
            throw new ArgumentException($"A grammar cannot contain more than {GrammarLimits.MaxTotalSymbols} production symbols in total.", nameof(rules));
        if (values.GroupBy(x => x.Id).Any(g => g.Count() > 1))
            throw new ArgumentException("A grammar cannot define the same rule ID more than once.", nameof(rules));

        _nonTerminals = values.Select(x => x.LeftHandSide).ToHashSet();
        if (!_nonTerminals.Contains(startSymbol))
            throw new ArgumentException("The start symbol must be the left-hand side of a grammar rule.", nameof(startSymbol));

        foreach (var symbol in values.SelectMany(x => x.RightHandSide).Where(x => x.IsNonTerminal))
        {
            if (!_nonTerminals.Contains(symbol.Id))
                throw new ArgumentException($"Grammar nonterminal '{symbol.Id}' has no production rule.", nameof(rules));
        }

        Id = id;
        Name = name;
        StartSymbol = startSymbol;
        _rules = Array.AsReadOnly(values);
    }

    /// <summary>Gets the grammar's nonzero identity.</summary>
    public ulong Id { get; }

    /// <summary>Gets the grammar's human-readable name.</summary>
    public string Name { get; }

    /// <summary>Gets the nonzero identity of the start nonterminal.</summary>
    public ulong StartSymbol { get; }

    /// <summary>Gets the immutable ordered production rules in declaration order.</summary>
    public IReadOnlyList<GrammarRule> Rules => _rules;

    /// <summary>
    /// Emits a deterministic representation suitable for inclusion in an AI-facing protocol description.
    /// </summary>
    /// <returns>A stable text representation of this grammar.</returns>
    public string Describe()
    {
        var builder = new StringBuilder();
        builder.Append('[').Append(Id).Append("] ").Append(Name)
            .Append(" start=[").Append(StartSymbol).Append(']');

        foreach (var rule in _rules)
        {
            builder.Append('\n');
            builder.Append("  rule [").Append(rule.Id).Append("] [")
                .Append(rule.LeftHandSide).Append("] -> ");

            if (rule.RightHandSide.Count == 0)
            {
                builder.Append('ε');
                continue;
            }

            for (var i = 0; i < rule.RightHandSide.Count; i++)
            {
                if (i > 0) builder.Append(' ');
                var symbol = rule.RightHandSide[i];
                if (symbol.IsProtocolSymbol)
                {
                    var reference = symbol.ProtocolReference!.Value;
                    builder.Append('[').Append(reference.ProtocolId).Append(':').Append(reference.SymbolId).Append(']');
                }
                else
                {
                    builder.Append('<').Append(symbol.Id).Append('>');
                }
            }
        }

        return builder.ToString();
    }

    /// <summary>Returns the deterministic representation produced by <see cref="Describe"/>.</summary>
    /// <returns>The grammar description.</returns>
    public override string ToString() => Describe();
}
