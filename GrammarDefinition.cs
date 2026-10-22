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
        if (startSymbol == 0) throw new ArgumentOutOfRangeException(nameof(startSymbol));

        var values = rules?.ToArray() ?? throw new ArgumentNullException(nameof(rules));
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
    /// Finds distinct protocol-terminal references the host's resolver cannot resolve.
    /// </summary>
    /// <param name="isResolved">
    /// Host-supplied lookup that returns true when a protocol and symbol identity are known.
    /// </param>
    /// <returns>Unresolved references in first-use order, without duplicates.</returns>
    /// <remarks>
    /// GrammarAI does not load protocol definitions or depend on ProtocolAI. The host supplies
    /// the lookup so it can use its own registry or protocol implementation. This checks identity
    /// existence only; it does not establish semantic compatibility between grammar and protocol versions.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="isResolved"/> is null.</exception>
    public IReadOnlyList<GrammarProtocolReference> FindUnresolvedProtocolReferences(
        Func<GrammarProtocolReference, bool> isResolved)
    {
        ArgumentNullException.ThrowIfNull(isResolved);

        var unresolved = new List<GrammarProtocolReference>();
        var seen = new HashSet<GrammarProtocolReference>();

        foreach (var symbol in _rules.SelectMany(rule => rule.RightHandSide))
        {
            if (symbol.ProtocolReference is not { } reference || !seen.Add(reference))
                continue;

            if (!isResolved(reference))
                unresolved.Add(reference);
        }

        return unresolved.AsReadOnly();
    }

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
