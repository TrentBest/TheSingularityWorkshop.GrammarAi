<!-- The Singularity Workshop moniker -->
<p align="center">
  <a href="https://github.com/TrentBest">
    <img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop — animated workshop moniker" width="720" />
  </a>
</p>

# Consuming GrammarAi

GrammarAi provides two structural models:

1. **Production grammar:** grammar identity, start symbol, nonterminals, ordered productions, and qualified external terminal references.
2. **Typed knowledge map:** versioned node kinds, contextual slots, typed relationships, cardinality, graph instances, and a compact proposal codec.

The application/host owns vocabulary meaning, the active catalog snapshot, identifier lifecycle, semantic validation, policy, authorization, and execution. GrammarAi validates structure; it does not call a model or execute the result.

## 1. Install the package

The latest published package remains `0.1.0-alpha.2`:

```bash
dotnet add package TheSingularityWorkshop.GrammarAi --version 0.1.0-alpha.2
```

Or:

```xml
<PackageReference Include="TheSingularityWorkshop.GrammarAi" Version="0.1.0-alpha.2" />
```

That published package can be consumed by a .NET 10 application, but it predates the repository's .NET 10 source standardization and does not contain the newer development-branch API surface.

The current source candidate is `0.1.0-alpha.3`, targeting `net10.0`. It is not published on NuGet. To build from the repository:

```bash
dotnet restore TheSingularityWorkshop.GrammarAi.slnx
dotnet build TheSingularityWorkshop.GrammarAi.slnx --configuration Release
dotnet test TheSingularityWorkshop.GrammarAi.slnx --configuration Release
```

New GrammarAi development, tests, examples, and package candidates target **.NET 10 (`net10.0`)**. The .NET 10 SDK is pinned in `global.json`.

## 2. Production grammar: reference protocol identities

GrammarAi deliberately does not need a direct ProtocolAi package reference. An external terminal is represented by the qualified pair:

```text
[protocolId:symbolId]
```

For example, `[1001:2001]` means protocol/category 1001 owns member symbol 2001.

Suppose the host's ProtocolAi definition owns these identities:

```text
[1001] People
[2001] Bob
[2002] Jane
```

A grammar can define a Greeting structure:

```csharp
using TheSingularityWorkshop.GrammarAi;

var grammar = new GrammarBuilder(
        3001,
        "Greeting",
        4001)
    .Rule(
        5001,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2001)))
    .Rule(
        5002,
        4001,
        GrammarSymbol.Terminal(
            new GrammarProtocolReference(1001, 2002)))
    .Build();
```

GrammarAi owns grammar IDs and rules. The external vocabulary owner retains the meaning of symbols 2001 and 2002.

### Nonterminals and terminals

A nonterminal belongs to the grammar:

```csharp
var person = GrammarSymbol.NonTerminal(4001);
```

A protocol terminal references an externally owned identity:

```csharp
var bob = GrammarSymbol.Terminal(
    new GrammarProtocolReference(1001, 2001));
```

A referenced nonterminal must have a production. Multiple rules with the same left-hand side are alternatives; symbols within one rule form an ordered sequence.

### Check external references

Building a production grammar validates its internal structure. It cannot establish whether a host's external catalog contains or exposes every terminal. Supply the host's own lookup:

```csharp
var unresolved = grammar.FindUnresolvedProtocolReferences(reference =>
    protocolRegistry.ContainsSymbol(reference.ProtocolId, reference.SymbolId));

if (unresolved.Count > 0)
{
    // Reject or report missing protocol/symbol identities.
}
```

Here, `protocolRegistry.ContainsSymbol` is illustrative host code, not a GrammarAi API. The host must resolve identities against the catalog/capability snapshot selected for the request. This check is not protocol-version compatibility, semantic validation, or authorization.

Inspect the production grammar with `grammar.Describe()`.

## 3. Typed knowledge maps: node kinds and contextual slots

The newer typed structural model represents a bounded graph rather than only a sequence of production symbols.

- `GrammarNodeDefinition` declares a grammar-owned node kind.
- `GrammarSlotDefinition` declares a contextual relationship role owned by a node kind.
- Each slot targets exactly one declared GrammarAi node kind or one opaque external ProtocolAi category identity.
- Minimum/maximum occurrence counts express required, optional, repeated, and bounded relationships.
- `GrammarStructureDefinition` is immutable and versioned.
- `GrammarNodeInstance` and `GrammarRelationship` represent a candidate graph.
- `GrammarStructureInstance` validates the graph against its definition.
- `GrammarStructureProposalCodec` serializes/deserializes the compact proposal format.

A slot such as `Condition` or `Body` is a contextual grammatical role. It is not a global ProtocolAi vocabulary category. The external category identifies the namespace of a member; the slot identifies the role that member plays in this particular structure.

See [Typed Structural Schema](STRUCTURAL_SCHEMA.md) for the complete public contract, worked control-flow schema, resolver behavior, wire tuples, compatibility rules, and limits.

### Snapshot-bound external reference resolution

When a graph contains an external protocol/category member, the host supplies an `IGrammarExternalReferenceResolver` bound to the exact catalog/capability snapshot used for the model request.

Only `GrammarExternalReferenceStatus.ResolvedAndExposed` is accepted. The other defined outcomes are `UnknownProtocol`, `UnknownSymbol`, and `NotExposed`; each fails closed. GrammarAi has no global registry and cannot widen the selected request view.

The proposal includes separate version dimensions:

- `v`: wire encoding version;
- `g`: grammar definition identity;
- `gv`: grammar definition/schema version.

The current serializer emits canonical wire v2 and can read the supported legacy v1 nested-node form. Old v1-only readers cannot consume v2; the host must negotiate the wire version before sending. GrammarAi does not migrate schemas automatically.

### What structural validation establishes

It establishes that the graph matches the selected definition's structural contract: identities, target kinds, slot ownership, cardinality, relationship shape/order, and required relationships. It does **not** establish that a node or external symbol is meaningful, that a proposed operation is valid in the domain, that evidence is true, or that an action is safe or authorized.

## 4. Use with CommandAi

CommandAi is an independent command-proposal contract. Its enriched atomic-command representation can carry an optional opaque GrammarAi body, including a positive grammar wire version and bounded payload bytes. CommandAi validates framing and resource limits; it does not decode or validate the grammar body.

A trusted consumer must pin the expected GrammarAi definition ID/schema version, decode the body with that exact definition, and resolve external references against the request's exact snapshot. The older compact `Command=` proposal form does not gain this enriched body capability automatically.

See [CommandAi usage](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/blob/development/docs/USAGE.md). The observer schema token `TheSingularityWorkshop.GrammarAi.Observer@1` is a separate observer contract identity; it must not be confused with the typed structural schema's definition ID/version or wire version.

## 5. SingularityBot integration status

SingularityBot currently uses its own `BotDefinition` and `BotCommandVocabulary` with CommandAi proposal objects. Its bot host validates the proposals and retains live-state checks, authorization input, objective mutation, and the SC2 action path.

**GrammarAi is not currently a SingularityBot runtime dependency.** The defensible future use is an optional authoring/interchange graph only if Forge, an LLM authoring tool, and another independent consumer need a shared versioned graph that is not already covered by BotDefinition and its validators. It must not duplicate BotDefinition, replace the bot's domain validation, or become a second strategy engine.

See [GrammarAi and SingularityBot](INTEGRATION_SINGULARITYBOT.md) for the source-audited current path, optional composition, boundaries, and required consumer tests.

## 6. Where GrammarAi fits

```text
domain vocabulary / identity
          |
          v
       ProtocolAi
          |
          | qualified external references
          v
        GrammarAi
          |
          | validated structure
          v
    host/domain adapter
          |
          +--> semantic validation
          +--> policy and authorization
          +--> interpretation / execution
```

ProtocolAi defines **WHAT** an external identity refers to. GrammarAi defines **HOW** identities and grammar-owned nodes may be arranged. The host owns the meaning of the resulting structure and decides what may happen.

GrammarAi does not own model integration, provider formatting, grammar compilation, prompt transport, protocol compatibility policy, persistence, workflow lifecycle, GUI presentation, or execution.

## 7. Verify and explore

```bash
dotnet restore TheSingularityWorkshop.GrammarAi.slnx
dotnet build TheSingularityWorkshop.GrammarAi.slnx --configuration Release
dotnet test TheSingularityWorkshop.GrammarAi.slnx --configuration Release
dotnet run --project examples/GrammarAi.QuickStart/GrammarAi.QuickStart.csproj
```

Read next:
- [Knowledge Mapping Model](KNOWLEDGE_MAPPING_MODEL.md)
- [Typed Structural Schema](STRUCTURAL_SCHEMA.md)
- [Examples](EXAMPLES.md)
- [Gallery](GALLERY.md)
- [Theory](THEORY.md)
- [Domain Completeness](DOMAIN_COMPLETENESS.md)
- [Ecosystem Adoption Audit](ECOSYSTEM_ADOPTION_AUDIT.md)
- [GrammarAi and SingularityBot](INTEGRATION_SINGULARITYBOT.md)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
