namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// One instantiated node in a grammar-defined knowledge structure.
/// </summary>
/// <remarks>
/// Instance identity is independent of the node-kind identity in <see cref="GrammarNodeDefinition"/>.
/// </remarks>
public sealed record GrammarNodeInstance
{
    public GrammarNodeInstance(ulong id, ulong nodeDefinitionId)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (nodeDefinitionId == 0) throw new ArgumentOutOfRangeException(nameof(nodeDefinitionId));
        Id = id;
        NodeDefinitionId = nodeDefinitionId;
    }

    /// <summary>Gets the unique identity of this node instance within its containing structure instance.</summary>
    public ulong Id { get; }

    /// <summary>Gets the schema node-kind identity represented by this instance.</summary>
    public ulong NodeDefinitionId { get; }
}

/// <summary>
/// An ordered, typed relationship between two node instances.
/// </summary>
/// <remarks>
/// The relationship's position in <see cref="GrammarStructureInstance.Relationships"/> is significant.
/// In particular, it preserves the order of repeated slots such as ElseIf clauses.
/// </remarks>
public sealed record GrammarRelationship
{
    public GrammarRelationship(ulong parentInstanceId, ulong slotId, ulong childInstanceId)
    {
        if (parentInstanceId == 0) throw new ArgumentOutOfRangeException(nameof(parentInstanceId));
        if (slotId == 0) throw new ArgumentOutOfRangeException(nameof(slotId));
        if (childInstanceId == 0) throw new ArgumentOutOfRangeException(nameof(childInstanceId));
        ParentInstanceId = parentInstanceId;
        SlotId = slotId;
        ChildInstanceId = childInstanceId;
    }

    /// <summary>Gets the parent node-instance identity.</summary>
    public ulong ParentInstanceId { get; }

    /// <summary>Gets the contextual slot identity declared by the parent's node kind.</summary>
    public ulong SlotId { get; }

    /// <summary>Gets the child node-instance identity.</summary>
    public ulong ChildInstanceId { get; }
}

/// <summary>
/// An immutable, validated graph of instantiated nodes and ordered typed relationships.
/// </summary>
/// <remarks>
/// This is data, not executable code. It validates structural identity, slot ownership,
/// target kinds, and occurrence constraints. It deliberately permits shared nodes and cycles:
/// a knowledge map is a graph, and tree/DAG restrictions belong to a higher-level profile.
/// The instance has no implicit root; a host may define roots or entry points in its own context.
/// </remarks>
public sealed class GrammarStructureInstance
{
    private readonly IReadOnlyList<GrammarNodeInstance> _nodes;
    private readonly IReadOnlyList<GrammarRelationship> _relationships;
    private readonly IReadOnlyDictionary<ulong, GrammarNodeInstance> _nodesById;

    public GrammarStructureInstance(
        GrammarStructureDefinition definition,
        IEnumerable<GrammarNodeInstance> nodes,
        IEnumerable<GrammarRelationship> relationships)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(nodes);
        ArgumentNullException.ThrowIfNull(relationships);

        var nodeValues = nodes.Take(GrammarLimits.MaxStructureInstances + 1).ToArray();
        var relationshipValues = relationships.Take(GrammarLimits.MaxStructureRelationships + 1).ToArray();
        if (nodeValues.Length > GrammarLimits.MaxStructureInstances)
            throw new ArgumentException($"A structure instance cannot contain more than {GrammarLimits.MaxStructureInstances} nodes.", nameof(nodes));
        if (relationshipValues.Length > GrammarLimits.MaxStructureRelationships)
            throw new ArgumentException($"A structure instance cannot contain more than {GrammarLimits.MaxStructureRelationships} relationships.", nameof(relationships));
        if (nodeValues.Any(node => node is null))
            throw new ArgumentException("A structure instance cannot contain null nodes.", nameof(nodes));
        if (relationshipValues.Any(relationship => relationship is null))
            throw new ArgumentException("A structure instance cannot contain null relationships.", nameof(relationships));

        var nodeDefinitions = definition.Nodes.ToDictionary(node => node.Id);
        var slotDefinitions = definition.Slots.ToDictionary(slot => slot.Id);
        var nodesById = new Dictionary<ulong, GrammarNodeInstance>();

        foreach (var node in nodeValues)
        {
            if (!nodeDefinitions.ContainsKey(node.NodeDefinitionId))
                throw new ArgumentException($"Node instance '{node.Id}' references unknown node kind '{node.NodeDefinitionId}'.", nameof(nodes));
            if (!nodesById.TryAdd(node.Id, node))
                throw new ArgumentException($"Node instance identity '{node.Id}' is duplicated.", nameof(nodes));
        }

        var occurrenceCounts = new Dictionary<(ulong ParentId, ulong SlotId), int>();
        foreach (var relationship in relationshipValues)
        {
            if (!nodesById.TryGetValue(relationship.ParentInstanceId, out var parent))
                throw new ArgumentException($"Relationship references unknown parent instance '{relationship.ParentInstanceId}'.", nameof(relationships));
            if (!nodesById.TryGetValue(relationship.ChildInstanceId, out var child))
                throw new ArgumentException($"Relationship references unknown child instance '{relationship.ChildInstanceId}'.", nameof(relationships));
            if (!slotDefinitions.TryGetValue(relationship.SlotId, out var slot))
                throw new ArgumentException($"Relationship references unknown slot '{relationship.SlotId}'.", nameof(relationships));
            if (parent.NodeDefinitionId != slot.OwnerNodeId)
                throw new ArgumentException($"Slot '{slot.Id}' does not belong to parent node kind '{parent.NodeDefinitionId}'.", nameof(relationships));
            if (child.NodeDefinitionId != slot.TargetNodeId)
                throw new ArgumentException($"Slot '{slot.Id}' requires target node kind '{slot.TargetNodeId}', not '{child.NodeDefinitionId}'.", nameof(relationships));

            (ulong ParentId, ulong SlotId) key = (relationship.ParentInstanceId, relationship.SlotId);
            occurrenceCounts.TryGetValue(key, out var count);
            count++;
            if (slot.MaximumOccurrences.HasValue && count > slot.MaximumOccurrences.Value)
                throw new ArgumentException($"Slot '{slot.Id}' on node instance '{parent.Id}' exceeds its maximum occurrence count.", nameof(relationships));
            occurrenceCounts[key] = count;
        }

        foreach (var node in nodeValues)
        {
            foreach (var slot in definition.GetSlots(node.NodeDefinitionId))
            {
                occurrenceCounts.TryGetValue((node.Id, slot.Id), out var count);
                if (count < slot.MinimumOccurrences)
                    throw new ArgumentException($"Slot '{slot.Id}' on node instance '{node.Id}' requires at least {slot.MinimumOccurrences} relationship(s), but has {count}.", nameof(relationships));
            }
        }

        Definition = definition;
        _nodes = Array.AsReadOnly(nodeValues);
        _relationships = Array.AsReadOnly(relationshipValues);
        _nodesById = new System.Collections.ObjectModel.ReadOnlyDictionary<ulong, GrammarNodeInstance>(nodesById);
    }

    /// <summary>Gets the schema used to validate this instance.</summary>
    public GrammarStructureDefinition Definition { get; }

    /// <summary>Gets node instances in supplied declaration order.</summary>
    public IReadOnlyList<GrammarNodeInstance> Nodes => _nodes;

    /// <summary>Gets relationships in supplied order; this order is significant for repeated slots.</summary>
    public IReadOnlyList<GrammarRelationship> Relationships => _relationships;

    /// <summary>Finds a node instance by its instance identity.</summary>
    public bool TryGetNode(ulong instanceId, out GrammarNodeInstance? node) =>
        _nodesById.TryGetValue(instanceId, out node);

    /// <summary>Returns relationships for a parent and slot, preserving their supplied order.</summary>
    public IReadOnlyList<GrammarRelationship> GetRelationships(ulong parentInstanceId, ulong slotId) =>
        Array.AsReadOnly(_relationships
            .Where(relationship => relationship.ParentInstanceId == parentInstanceId && relationship.SlotId == slotId)
            .ToArray());
}
