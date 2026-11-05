namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Immutable typed node-and-slot schema for an AI-facing knowledge map.
/// </summary>
/// <remarks>
/// Node and slot identities belong to GrammarAi and are not ProtocolAi vocabulary symbol IDs.
/// This schema describes permitted structure; it does not instantiate nodes, parse text, or authorize execution.
/// </remarks>
public sealed class GrammarStructureDefinition
{
    private readonly IReadOnlyList<GrammarNodeDefinition> _nodes;
    private readonly IReadOnlyList<GrammarSlotDefinition> _slots;

    public GrammarStructureDefinition(
        ulong id,
        string name,
        IEnumerable<GrammarNodeDefinition> nodes,
        IEnumerable<GrammarSlotDefinition> slots,
        uint version = 1)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (version == 0) throw new ArgumentOutOfRangeException(nameof(version));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A structure name is required.", nameof(name));
        if (name.Length > GrammarLimits.MaxNameCharacters)
            throw new ArgumentException($"A structure name cannot exceed {GrammarLimits.MaxNameCharacters} characters.", nameof(name));
        ArgumentNullException.ThrowIfNull(nodes);
        ArgumentNullException.ThrowIfNull(slots);

        var nodeValues = nodes.Take(GrammarLimits.MaxRules + 1).ToArray();
        var slotValues = slots.Take(GrammarLimits.MaxRules + 1).ToArray();
        if (nodeValues.Length > GrammarLimits.MaxRules)
            throw new ArgumentException($"A structure cannot contain more than {GrammarLimits.MaxRules} nodes.", nameof(nodes));
        if (slotValues.Length > GrammarLimits.MaxRules)
            throw new ArgumentException($"A structure cannot contain more than {GrammarLimits.MaxRules} slots.", nameof(slots));
        if (nodeValues.Any(x => x is null)) throw new ArgumentException("A structure cannot contain null nodes.", nameof(nodes));
        if (slotValues.Any(x => x is null)) throw new ArgumentException("A structure cannot contain null slots.", nameof(slots));
        if (nodeValues.Select(x => x.Id).Distinct().Count() != nodeValues.Length)
            throw new ArgumentException("Node identities must be unique within a structure.", nameof(nodes));
        if (slotValues.Select(x => x.Id).Distinct().Count() != slotValues.Length)
            throw new ArgumentException("Slot identities must be unique within a structure.", nameof(slots));

        var nodeIds = nodeValues.Select(x => x.Id).ToHashSet();
        foreach (var slot in slotValues)
        {
            if (!nodeIds.Contains(slot.OwnerNodeId))
                throw new ArgumentException($"Slot '{slot.Id}' references unknown owner node '{slot.OwnerNodeId}'.", nameof(slots));
            if (!nodeIds.Contains(slot.TargetNodeId))
                throw new ArgumentException($"Slot '{slot.Id}' references unknown target node '{slot.TargetNodeId}'.", nameof(slots));
        }

        Id = id;
        Version = version;
        Name = name;
        _nodes = Array.AsReadOnly(nodeValues);
        _slots = Array.AsReadOnly(slotValues);
    }

    /// <summary>Gets this structure definition's nonzero identity.</summary>
    public ulong Id { get; }

    /// <summary>Gets the positive schema version used for compatibility checks.</summary>
    public uint Version { get; }

    /// <summary>Gets its human-readable name.</summary>
    public string Name { get; }

    /// <summary>Gets the immutable node kinds in declaration order.</summary>
    public IReadOnlyList<GrammarNodeDefinition> Nodes => _nodes;

    /// <summary>Gets the immutable typed relationships in declaration order.</summary>
    public IReadOnlyList<GrammarSlotDefinition> Slots => _slots;

    /// <summary>Returns the slots declared by a node, preserving declaration order.</summary>
    public IReadOnlyList<GrammarSlotDefinition> GetSlots(ulong ownerNodeId) =>
        Array.AsReadOnly(_slots.Where(slot => slot.OwnerNodeId == ownerNodeId).ToArray());
}
