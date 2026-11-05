namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Defines a typed relationship from one grammar node to another, including occurrence constraints.
/// </summary>
public sealed record GrammarSlotDefinition
{
    public GrammarSlotDefinition(
        ulong id,
        ulong ownerNodeId,
        string name,
        ulong targetNodeId,
        int minimumOccurrences = 1,
        int? maximumOccurrences = 1)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (ownerNodeId == 0) throw new ArgumentOutOfRangeException(nameof(ownerNodeId));
        if (targetNodeId == 0) throw new ArgumentOutOfRangeException(nameof(targetNodeId));
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
        TargetNodeId = targetNodeId;
        MinimumOccurrences = minimumOccurrences;
        MaximumOccurrences = maximumOccurrences;
    }

    /// <summary>Gets the nonzero identity of this slot within its structure definition.</summary>
    public ulong Id { get; }

    /// <summary>Gets the grammar node kind that owns this relationship.</summary>
    public ulong OwnerNodeId { get; }

    /// <summary>Gets the contextual role name, such as Condition or Body.</summary>
    public string Name { get; }

    /// <summary>Gets the grammar node kind permitted as the target.</summary>
    public ulong TargetNodeId { get; }

    /// <summary>Gets the minimum number of target nodes required in this slot.</summary>
    public int MinimumOccurrences { get; }

    /// <summary>Gets the maximum number of target nodes; null means unbounded.</summary>
    public int? MaximumOccurrences { get; }
}
