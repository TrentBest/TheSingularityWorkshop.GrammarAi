using TheSingularityWorkshop.GrammarAi;

var grammar = new GrammarBuilder(3001, "Greeting", 4001)
    .Rule(5001, 4001,
        GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2001)))
    .Rule(5002, 4001,
        GrammarSymbol.Terminal(new GrammarProtocolReference(1001, 2002)))
    .Build();

Console.WriteLine("PRODUCTION GRAMMAR");
Console.WriteLine(grammar.Describe());
Console.WriteLine();

var schema = new GrammarStructureDefinition(
    9001,
    "ConditionalControlFlow",
    [
        new GrammarNodeDefinition(1, "IfStatement"),
        new GrammarNodeDefinition(2, "Expression"),
        new GrammarNodeDefinition(3, "Block")
    ],
    [
        new GrammarSlotDefinition(101, 1, "Condition", 2),
        new GrammarSlotDefinition(102, 1, "Body", 3)
    ],
    version: 1);

var structure = new GrammarStructureInstance(
    schema,
    [
        new GrammarNodeInstance(100, 1),
        new GrammarNodeInstance(101, 2),
        new GrammarNodeInstance(102, 3)
    ],
    [
        new GrammarRelationship(100, 101, 101),
        new GrammarRelationship(100, 102, 102)
    ]);

var proposal = GrammarStructureProposalCodec.Serialize(structure);
var validated = GrammarStructureProposalCodec.Deserialize(proposal, schema);

Console.WriteLine("INTEGER-FIRST PROPOSAL");
Console.WriteLine(proposal);
Console.WriteLine($"Validated {validated.Nodes.Count} nodes and {validated.Relationships.Count} relationships.");
Console.WriteLine();

var hostRegistry = new HostIdentifierRegistry();
var firstRequestId = hostRegistry.GetOrAdd("function:Calculate", "total");
var nextRequestId = hostRegistry.GetOrAdd("function:Calculate", "total");
var otherScopeId = hostRegistry.GetOrAdd("function:Other", "total");

Console.WriteLine("HOST-OWNED IDENTIFIER CONTINUITY");
Console.WriteLine($"Same scoped name reused: {firstRequestId == nextRequestId}");
Console.WriteLine($"Same spelling in another scope gets another identity: {firstRequestId != otherScopeId}");
Console.WriteLine("This small registry is an illustrative host responsibility, not a GrammarAi service.");

/// <summary>
/// Minimal example of host-owned identifier interning.
/// A production host must define its own scope, persistence, collision, and authorization policies.
/// </summary>
sealed class HostIdentifierRegistry
{
    private readonly Dictionary<(string Scope, string Name), ulong> _identities = new();
    private ulong _nextIdentity = 1;

    public ulong GetOrAdd(string scope, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(scope);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var key = (scope, name);
        if (_identities.TryGetValue(key, out var identity))
            return identity;

        identity = _nextIdentity++;
        _identities.Add(key, identity);
        return identity;
    }
}
