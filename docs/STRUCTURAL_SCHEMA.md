<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# Typed Structural Schema

## Purpose

The integer-backed production grammar describes ordered symbol expansion. The typed structural schema adds a second view: named node kinds and named relationships between those node kinds.

This is the knowledge-map layer that lets an AI express a construct as a structured object graph rather than repeatedly spelling every relationship in natural language.

## Identity ownership

- GrammarNodeDefinition.Id and GrammarSlotDefinition.Id are identities in the GrammarAi structural namespace.
- GrammarProtocolReference remains the identity of an externally owned ProtocolAi vocabulary symbol.
- The numeric values may happen to be equal; equality does not make the identities interchangeable.
- Names are diagnostic labels. The IDs are the machine-facing addresses.

A slot Name is a contextual role such as Condition, Body, ElseIf, or Else. It is not a global lexical category for a ProtocolAi token.

## Node and slot contract

A node definition has an ID and a diagnostic name.

A slot definition declares:
- its own ID;
- the owning node kind;
- a contextual role name;
- exactly one target: a GrammarAi node kind or an externally owned ProtocolAi category;
- minimum occurrences;
- maximum occurrences, where null means unbounded.

| Minimum | Maximum | Meaning |
|---:|---:|---|
| 1 | 1 | exactly one required child |
| 0 | 1 | optional child |
| 0 | null | zero or more children |
| 1 | null | one or more children |
| N | M | bounded repeated children |

The structure definition validates unique node and slot IDs and ensures every owner references a node in the same definition. A slot target is a discriminated value: GrammarSlotTarget.Node(nodeDefinitionId) or GrammarSlotTarget.ExternalProtocolCategory(protocolCategoryId). Exactly one target case is represented. Node targets must reference a declared node kind; external category IDs remain opaque ProtocolAi-owned identities. It preserves declaration order and exposes GetSlots(ownerNodeId).

## Worked control-flow schema

The current tests define this first structural slice:

| Owner node | Slot | Target | Minimum | Maximum |
|---|---|---|---:|---:|
| IfStatement | Condition | Expression | 1 | 1 |
| IfStatement | Body | Block | 1 | 1 |
| IfStatement | ElseIf | ElseIfClause | 0 | unbounded |
| IfStatement | Else | ElseClause | 0 | 1 |
| ElseIfClause | Condition | Expression | 1 | 1 |
| ElseIfClause | Body | Block | 1 | 1 |
| ElseClause | Body | Block | 1 | 1 |

This expresses an ordered sequence of zero or more ElseIfClause relationships and at most one ElseClause. A consumer represents repeated child occurrences in order; the schema itself does not instantiate a program tree.

## Current implementation

The public API includes:
- GrammarNodeDefinition and GrammarSlotDefinition for grammar-owned kind and contextual-role identities;
- GrammarStructureDefinition for immutable node/slot schemas, including a positive definition Version;
- GrammarNodeInstance and GrammarRelationship for integer-addressed graph instances;
- GrammarStructureInstance for immutable graph validation;
- GrammarStructureProposalCodec for the compact, versioned JSON exchange format.

A proposal carries the definition ID and version. The host selects the active definition and passes it to Deserialize; a mismatch is rejected rather than silently interpreted against a different map. The wire version is separately versioned from the schema version. Names are omitted from the payload because the receiver resolves integer IDs through the selected definition.

The canonical compact shape is:

~~~json
{"v":2,"g":9001,"gv":2,"n":[[100,1],[101,4],[102,5]],"r":[[100,101,0,101],[100,102,0,102]]}
~~~

The n tuples are [instance ID, node-kind ID]. In v2, r tuples are tagged: [parent, slot, 0, child instance] for a nested node, or [parent, slot, 1, protocol/category ID, member symbol ID] for an external vocabulary value. v1 [parent, slot, child] tuples remain readable; the serializer emits canonical v2. External values require an IGrammarExternalReferenceResolver supplied by the host and bound to the exact catalog/capability snapshot used for the model request. Resolver outcomes distinguish resolved-and-exposed, unknown protocol, unknown symbol, and not exposed. The resolver is not a global GrammarAi registry. Tuple and relationship order are preserved. The codec rejects unknown wire versions/tags, mismatched definition ID/version, duplicate or unexpected properties, malformed tuples, oversized input, and structurally invalid graphs. Serialization is deterministic for the same validated instance.

## External-reference resolution contract

The public host boundary is intentionally small:

~~~csharp
public enum GrammarExternalReferenceStatus : byte
{
    ResolvedAndExposed = 0,
    UnknownProtocol = 1,
    UnknownSymbol = 2,
    NotExposed = 3
}

public interface IGrammarExternalReferenceResolver
{
    GrammarExternalReferenceStatus Resolve(GrammarProtocolReference reference);
}
~~~

GrammarProtocolReference qualifies a member symbol with its owning protocol/category identity. The resolver is required whenever an instance contains an external relationship. UnknownProtocol means the referenced category/protocol identity is not present in the selected catalog; UnknownSymbol means the member is not present in that category; NotExposed means the identity exists but is not exposed in the selected request view. Only ResolvedAndExposed is accepted. An unknown enum value also fails closed because validation accepts only that exact success value.

The resolver implementation belongs to the host and must be bound to the exact catalog/capability snapshot used to construct the model request. Snapshot identity and request provenance stay in the host-owned exchange envelope; GrammarAi receives only the resolver for that snapshot. GrammarAi has no global registry, session cache, or permission to widen the selected view.

A relationship carries exactly one value case: either a child instance ID or a qualified external reference. Its ChildInstanceId accessor throws for an external relationship; no synthetic child ID is produced. The target case must match the slot's declared target. Minimum/maximum cardinality applies to all relationships in that slot, and the original relationship sequence is preserved, including interleaved relationships belonging to different slots.

## Wire compatibility

| Capability | v1 reader | v2 reader |
|---|---|---|
| Nested node relationship | [parent, slot, child] | [parent, slot, 0, child] |
| External category/member value | Not representable | [parent, slot, 1, protocolOrCategory, member] |
| Serializer output | Historical v1 | Canonical v2 |
| Read compatibility | Existing v1 payloads | v1 nested payloads and v2 tagged payloads |
| Compatibility with old readers | Native | Old v1 readers cannot consume v2; negotiate wire version in the host before sending |

The top-level g and gv values identify the grammar definition and its structural version; v identifies the wire encoding. Changing a slot's target contract changes the structural definition and requires a new definition version. Changing tuple encoding changes the wire version. The codec rejects a stale definition ID/version; it does not migrate schemas. The host must negotiate a wire version with a consumer before transmitting v2.

## Still outside this increment

This addition does not yet provide:
- a host identifier allocator, scope registry, or cross-request interning service;
- semantic interpretation or a host-specific root/entry-point policy;
- automatic migration between different schema versions;
- ProtocolAi category metadata or a ProtocolAi dependency; GrammarAi accepts opaque category/member IDs and delegates membership/exposure checks to the host resolver;
- semantic validation, authorization, or execution.

Those concerns remain separate contracts. In particular, a structurally valid schema or future proposal does not authorize an operation.

## Verification

The GrammarStructureDefinitionTests exercise the If/ElseIf/Else schema, occurrence bounds, unknown references, duplicate identities, and immutable snapshots. GrammarStructureInstanceTests exercise a populated If/ElseIf/Else graph, ordered repeated branches, omitted optional else, required-slot enforcement, wrong target kinds, slot ownership, endpoint/identity validation, graph semantics, v1 nested decoding, v2 external-value round trips, colliding member IDs under distinct categories, resolver-required and resolver-failure cases, wrong category/target cases, mixed relationship ordering and cardinality, malformed tuple arities, stale schema rejection, unknown wire versions/tags, duplicate-property rejection, oversized payload rejection, and structural validation after decoding. Instance collections are bounded by GrammarLimits.MaxStructureInstances and GrammarLimits.MaxStructureRelationships; serialized proposals are bounded by GrammarLimits.MaxProposalCharacters. Repository CI is the execution evidence for these tests.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
