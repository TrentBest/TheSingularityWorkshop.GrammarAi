namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Builder used by tools to define grammar structure before publishing an immutable definition.
/// </summary>
public sealed class GrammarBuilder
{
    private readonly List<GrammarRule> _rules = [];

    public GrammarBuilder(ulong id, string name, ulong startSymbol)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A grammar name is required.", nameof(name));
        if (startSymbol == 0) throw new ArgumentOutOfRangeException(nameof(startSymbol));

        Id = id;
        Name = name;
        StartSymbol = startSymbol;
    }

    public ulong Id { get; }
    public string Name { get; }
    public ulong StartSymbol { get; }

    public GrammarBuilder Rule(ulong ruleId, ulong leftHandSide, params GrammarSymbol[] rightHandSide)
    {
        _rules.Add(new GrammarRule(ruleId, leftHandSide, rightHandSide));
        return this;
    }

    public GrammarDefinition Build() => new(Id, Name, StartSymbol, _rules);
}