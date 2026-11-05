namespace TheSingularityWorkshop.GrammarAi;

/// <summary>
/// Defines a grammar-owned structural node kind. Its identity is separate from ProtocolAi vocabulary identities.
/// </summary>
public sealed record GrammarNodeDefinition
{
    public GrammarNodeDefinition(ulong id, string name)
    {
        if (id == 0) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("A node name is required.", nameof(name));
        if (name.Length > GrammarLimits.MaxNameCharacters)
            throw new ArgumentException($"A node name cannot exceed {GrammarLimits.MaxNameCharacters} characters.", nameof(name));

        Id = id;
        Name = name;
    }

    /// <summary>Gets the nonzero identity owned by this grammar's structural namespace.</summary>
    public ulong Id { get; }

    /// <summary>Gets the diagnostic name of this node kind.</summary>
    public string Name { get; }
}
