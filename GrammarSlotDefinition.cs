namespace TheSingularityWorkshop.GrammarAi;

/// <summary>Defines a typed relationship from one grammar node to a node or external category.</summary>
public sealed record GrammarSlotDefinition
{
    /// <summary>Source-compatible constructor for a nested GrammarAi node target.</summary>
    public GrammarSlotDefinition(ulong id, ulong ownerNodeId, string name, ulong targetNodeId,
        int minimumOccurrences = 1, int? maximumOccurrences = 1)
        : this(id, ownerNodeId, name, GrammarSlotTarget.Node(targetNodeId), minimumOccurrences, maximumOccurrences) { }

    public GrammarSlotDefinition(ulong id, ulong ownerNodeId, string name, GrammarSlotTarget target,
        int minimumOccurrences = 1, int? maximumOccurrences = 1)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (ownerNodeId == 0) throw new ArgumentOutOfRangeException(nameof(ownerNodeId));
        if (target.Id == 0) throw new ArgumentOutOfRangeException(nameof(target));
        if (target.Kind is not (GrammarSlotTargetKind.Node or GrammarSlotTargetKind.ExternalProtocolCategory))
            throw new ArgumentOutOfRangeException(nameof(target));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A slot name is required.", nameof(name));
        if (name.Length > GrammarLimits.MaxNameCharacters)
            throw new ArgumentException($"A slot name cannot exceed {GrammarLimits.MaxNameCharacters} characters.", nameof(name));
        if (minimumOccurrences < 0) throw new ArgumentOutOfRangeException(nameof(minimumOccurrences));
        if (maximumOccurrences is < 0) throw new ArgumentOutOfRangeException(nameof(maximumOccurrences));
        if (maximumOccurrences.HasValue && maximumOccurrences.Value < minimumOccurrences)
            throw new ArgumentException("Maximum occurrences cannot be less than minimum occurrences.", nameof(maximumOccurrences));

        Id = id;
        OwnerNodeId = ownerNodeId;
        Name = name;
        Target = target;
        MinimumOccurrences = minimumOccurrences;
        MaximumOccurrences = maximumOccurrences;
    }

    public ulong Id { get; }
    public ulong OwnerNodeId { get; }
    public string Name { get; }
    public GrammarSlotTarget Target { get; }
    /// <summary>Gets the node target for legacy callers; throws for an external-category slot.</summary>
    public ulong TargetNodeId => Target.Kind == GrammarSlotTargetKind.Node
        ? Target.Id
        : throw new InvalidOperationException("This slot targets an external ProtocolAi category.");
    public ulong TargetProtocolCategoryId => Target.Kind == GrammarSlotTargetKind.ExternalProtocolCategory
        ? Target.Id
        : throw new InvalidOperationException("This slot targets a GrammarAi node kind.");
    public int MinimumOccurrences { get; }
    public int? MaximumOccurrences { get; }
}
