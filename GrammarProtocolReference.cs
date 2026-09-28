namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Identifies a terminal symbol owned by an external integer-backed protocol.
/// </summary>
public readonly record struct GrammarProtocolReference
{
    public GrammarProtocolReference(ulong protocolId, ulong symbolId)
    {
        if (protocolId == 0) throw new ArgumentOutOfRangeException(nameof(protocolId));
        if (symbolId == 0) throw new ArgumentOutOfRangeException(nameof(symbolId));

        ProtocolId = protocolId;
        SymbolId = symbolId;
    }

    public ulong ProtocolId { get; }
    public ulong SymbolId { get; }
}
