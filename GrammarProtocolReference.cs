namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Identifies a terminal symbol owned by an external integer-backed protocol.
/// </summary>
public readonly record struct GrammarProtocolReference
{
    /// <summary>Creates a reference to a protocol symbol.</summary>
    /// <param name="protocolId">Nonzero identity of the owning protocol.</param>
    /// <param name="symbolId">Nonzero identity of the symbol within that protocol.</param>
    /// <exception cref="ArgumentOutOfRangeException">Either identifier is zero.</exception>
    public GrammarProtocolReference(ulong protocolId, ulong symbolId)
    {
        if (protocolId == 0) throw new ArgumentOutOfRangeException(nameof(protocolId));
        if (symbolId == 0) throw new ArgumentOutOfRangeException(nameof(symbolId));

        ProtocolId = protocolId;
        SymbolId = symbolId;
    }

    /// <summary>Gets the nonzero identity of the owning protocol.</summary>
    public ulong ProtocolId { get; }

    /// <summary>Gets the nonzero identity of the referenced symbol within its protocol.</summary>
    public ulong SymbolId { get; }
}
