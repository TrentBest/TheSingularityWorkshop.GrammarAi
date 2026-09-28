using System.Text;

namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Immutable, self-describing grammar that connects protocol symbols through integer-backed production rules.
/// </summary>
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

    public ulong Id { get; }
    public string Name { get; }
    public ulong StartSymbol { get; }
    public IReadOnlyList<GrammarRule> Rules => _rules;

    /// <summary>
    /// Emits a deterministic representation suitable for inclusion in an AI-facing protocol description.
    /// </summary>
    public string Describe()
    {
        var builder = new StringBuilder();
        builder.Append('[').Append(Id).Append("] ").Append(Name)
            .Append(" start=[").Append(StartSymbol).Append(']');

        foreach (var rule in _rules)
        {
            builder.AppendLine();
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

    public override string ToString() => Describe();
}