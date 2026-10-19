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

    /// <summary>Gets the nonzero symbol identity. For protocol terminals, this is the referenced protocol symbol ID.</summary>
    public ulong Id { get; }

    /// <summary>Gets the owning protocol and symbol identity for a terminal; null for a nonterminal.</summary>
    public GrammarProtocolReference? ProtocolReference { get; }

    /// <summary>Gets whether this symbol represents a grammar nonterminal.</summary>
    public bool IsNonTerminal => ProtocolReference is null;

    /// <summary>Gets whether this symbol references a terminal owned by an external protocol.</summary>
    public bool IsProtocolSymbol => ProtocolReference.HasValue;

    /// <summary>Creates a nonterminal identified within this grammar.</summary>
    /// <param name="id">Nonzero nonterminal identity.</param>
    /// <returns>The nonterminal symbol.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/> is zero.</exception>
    public static GrammarSymbol NonTerminal(ulong id) => new(id, null);

    /// <summary>Creates a terminal reference to a symbol in an external protocol.</summary>
    /// <param name="reference">The nonzero protocol and symbol identities.</param>
    /// <returns>The protocol-terminal symbol.</returns>
    public static GrammarSymbol Terminal(GrammarProtocolReference reference) =>
        new(reference.SymbolId, reference);
}
