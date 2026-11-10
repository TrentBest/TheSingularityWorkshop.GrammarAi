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

// The host supplies ProtocolAi identities from its active vocabulary snapshot.
var csharpTokens = Enum.GetValues<CSharpExpressionToken>()
    .ToDictionary(token => token, token => (ulong)token + 300);
var csharpExpressions = CSharpExpressionGrammar.Create(3002, 1001, csharpTokens);
Console.WriteLine("LAYERED C# EXPRESSION GRAMMAR");
Console.WriteLine(csharpExpressions.Describe());
Console.WriteLine("The productions encode assignment, conditional and null-coalescing expressions, logical and bitwise operators, shifts, equality and relational operators, arithmetic, unary expressions, then recursive postfix member access, invocation (including empty argument lists), and element access.");
Console.WriteLine("The same postfix form composes chains such as receiver.Member(arguments)[index].");
Console.WriteLine();

// Type syntax is a separate grammar resource so expression and type maps can evolve independently.
var csharpTypeTokens = Enum.GetValues<CSharpTypeToken>()
    .ToDictionary(token => token, token => (ulong)token + 700);
var csharpTypes = CSharpTypeGrammar.Create(3003, 1001, csharpTypeTokens);
Console.WriteLine("LAYERED C# TYPE GRAMMAR");
Console.WriteLine(csharpTypes.Describe());
Console.WriteLine("The bounded type map covers qualified and generic names, nullable forms, array ranks, and tuple types.");
Console.WriteLine("It describes syntax shape only; type binding, aliases, nullable-reference semantics, and language-version policy remain host/compiler concerns.");
Console.WriteLine();

var schema = new GrammarStructureDefinition(
    9001,
    "ConditionalControlFlow",
    [
        new GrammarNodeDefinition(1, "IfStatement"),
        new GrammarNodeDefinition(2, "ElseIfClause"),
        new GrammarNodeDefinition(3, "ElseClause"),
        new GrammarNodeDefinition(4, "Expression"),
        new GrammarNodeDefinition(5, "Block")
    ],
    [
        new GrammarSlotDefinition(101, 1, "Condition", 4),
        new GrammarSlotDefinition(102, 1, "Body", 5),
        new GrammarSlotDefinition(103, 1, "ElseIf", 2, 0, null),
        new GrammarSlotDefinition(104, 1, "Else", 3, 0, 1),
        new GrammarSlotDefinition(201, 2, "Condition", 4),
        new GrammarSlotDefinition(202, 2, "Body", 5),
        new GrammarSlotDefinition(301, 3, "Body", 5)
    ],
    version: 1);

var structure = new GrammarStructureInstance(
    schema,
    [
        new GrammarNodeInstance(100, 1), // if
        new GrammarNodeInstance(101, 4), // if condition
        new GrammarNodeInstance(102, 5), // if body
        new GrammarNodeInstance(103, 2), // else-if
        new GrammarNodeInstance(104, 4), // else-if condition
        new GrammarNodeInstance(105, 5), // else-if body
        new GrammarNodeInstance(106, 3), // else
        new GrammarNodeInstance(107, 5)  // else body
    ],
    [
        new GrammarRelationship(100, 101, 101),
        new GrammarRelationship(100, 102, 102),
        new GrammarRelationship(100, 103, 103),
        new GrammarRelationship(100, 104, 106),
        new GrammarRelationship(103, 201, 104),
        new GrammarRelationship(103, 202, 105),
        new GrammarRelationship(106, 301, 107)
    ]);

var proposal = GrammarStructureProposalCodec.Serialize(structure);
var validated = GrammarStructureProposalCodec.Deserialize(proposal, schema);

Console.WriteLine("INTEGER-FIRST CONTROL-FLOW PROPOSAL");
Console.WriteLine(proposal);
Console.WriteLine($"Validated {validated.Nodes.Count} nodes and {validated.Relationships.Count} ordered relationships.");
Console.WriteLine("The ElseIf relationship precedes Else; the schema permits zero or more ElseIf clauses and at most one Else.");
Console.WriteLine();

var hostRegistry = new HostIdentifierRegistry();
var firstRequestId = hostRegistry.GetOrAdd("function:Calculate", "total");
var nextRequestId = hostRegistry.GetOrAdd("function:Calculate", "total");
var otherScopeId = hostRegistry.GetOrAdd("function:Other", "total");

Console.WriteLine("HOST-OWNED IDENTIFIER CONTINUITY");
Console.WriteLine($"Same scoped name reused: {firstRequestId == nextRequestId}");
Console.WriteLine($"Same spelling in another scope gets another identity: {firstRequestId != otherScopeId}");
Console.WriteLine("This small registry is an illustrative host responsibility, not a GrammarAi service.");

var externalReference = new GrammarProtocolReference(1001, 2001);
var catalogSnapshot = new HostCatalogSnapshot("catalog-snapshot-42", [externalReference]);
var externalSchema = new GrammarStructureDefinition(
    9002,
    "Invocation",
    [new GrammarNodeDefinition(10, "Invocation")],
    [new GrammarSlotDefinition(110, 10, "Target", GrammarSlotTarget.ExternalProtocolCategory(1001))]);
var externalStructure = new GrammarStructureInstance(
    externalSchema,
    [new GrammarNodeInstance(200, 10)],
    [new GrammarRelationship(200, 110, externalReference)],
    catalogSnapshot);
var externalProposal = GrammarStructureProposalCodec.Serialize(externalStructure);
var exchange = new HostExchangeEnvelope("request-42", catalogSnapshot.SnapshotId, externalProposal);
var validatedExternal = GrammarStructureProposalCodec.Deserialize(
    exchange.StructureProposal, externalSchema, catalogSnapshot);

Console.WriteLine();
Console.WriteLine("SNAPSHOT-BOUND EXTERNAL REFERENCE");
Console.WriteLine($"Validated {validatedExternal.Relationships.Count} external relationship against {exchange.CatalogSnapshotId}.");

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

/// <summary>
/// Illustrative immutable catalog snapshot selected by the host for one model request.
/// A production implementation should bind this resolver to its actual catalog and capability snapshot.
/// </summary>
sealed class HostCatalogSnapshot : IGrammarExternalReferenceResolver
{
    private readonly HashSet<GrammarProtocolReference> _exposedReferences;

    public HostCatalogSnapshot(string snapshotId, IEnumerable<GrammarProtocolReference> exposedReferences)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(snapshotId);
        ArgumentNullException.ThrowIfNull(exposedReferences);
        SnapshotId = snapshotId;
        _exposedReferences = exposedReferences.ToHashSet();
    }

    public string SnapshotId { get; }

    public GrammarExternalReferenceStatus Resolve(GrammarProtocolReference reference)
    {
        if (_exposedReferences.Contains(reference))
            return GrammarExternalReferenceStatus.ResolvedAndExposed;

        return _exposedReferences.Any(candidate => candidate.ProtocolId == reference.ProtocolId)
            ? GrammarExternalReferenceStatus.UnknownSymbol
            : GrammarExternalReferenceStatus.UnknownProtocol;
    }
}

/// <summary>Host-owned request provenance; deliberately not part of the GrammarAi proposal.</summary>
sealed record HostExchangeEnvelope(
    string RequestId,
    string CatalogSnapshotId,
    string StructureProposal);
