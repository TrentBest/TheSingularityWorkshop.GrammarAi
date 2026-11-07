namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Identifies the kind of value accepted by a structural slot.</summary>
public enum GrammarSlotTargetKind : byte
{
    Node = 0,
    ExternalProtocolCategory = 1
}

/// <summary>
/// A discriminated slot target. External category IDs are opaque ProtocolAi-owned identities;
/// GrammarAi does not resolve or allocate them.
/// </summary>
public readonly record struct GrammarSlotTarget
{
    private GrammarSlotTarget(GrammarSlotTargetKind kind, ulong id)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        Kind = kind;
        Id = id;
    }

    public GrammarSlotTargetKind Kind { get; }
    public ulong Id { get; }

    public static GrammarSlotTarget Node(ulong nodeDefinitionId) =>
        new(GrammarSlotTargetKind.Node, nodeDefinitionId);

    public static GrammarSlotTarget ExternalProtocolCategory(ulong protocolCategoryId) =>
        new(GrammarSlotTargetKind.ExternalProtocolCategory, protocolCategoryId);
}

/// <summary>Host-supplied resolution outcome for a qualified external protocol symbol.</summary>
public enum GrammarExternalReferenceStatus : byte
{
    ResolvedAndExposed = 0,
    UnknownProtocol = 1,
    UnknownSymbol = 2,
    NotExposed = 3
}

/// <summary>
/// Resolves external references against the exact host-selected catalog/capability snapshot
/// used for the model request. Implementations must not silently widen that snapshot.
/// </summary>
public interface IGrammarExternalReferenceResolver
{
    GrammarExternalReferenceStatus Resolve(GrammarProtocolReference reference);
}
