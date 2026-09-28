namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// A grammar element: either an integer-backed nonterminal or a terminal supplied by an external protocol.
/// </summary>
public readonly record struct GrammarSymbol
{
    private GrammarSymbol(ulong id, GrammarProtocolReference? protocolReference)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (protocolReference.HasValue && protocolReference.Value.ProtocolId == 0)
            throw new ArgumentException("The protocol reference must be valid.", nameof(protocolReference));
        Id = id;
        ProtocolReference = protocolReference;
    }

    public ulong Id { get; }
    public GrammarProtocolReference? ProtocolReference { get; }
    public bool IsNonTerminal => ProtocolReference is null;
    public bool IsProtocolSymbol => ProtocolReference.HasValue;

    public static GrammarSymbol NonTerminal(ulong id) => new(id, null);

    public static GrammarSymbol Terminal(GrammarProtocolReference reference) =>
        new(reference.SymbolId, reference);
}