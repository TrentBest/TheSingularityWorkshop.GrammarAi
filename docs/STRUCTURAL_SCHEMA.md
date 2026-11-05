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
- the permitted target node kind;
- minimum occurrences;
- maximum occurrences, where null means unbounded.

| Minimum | Maximum | Meaning |
|---:|---:|---|
| 1 | 1 | exactly one required child |
| 0 | 1 | optional child |
| 0 | null | zero or more children |
| 1 | null | one or more children |
| N | M | bounded repeated children |

The structure definition validates unique node and slot IDs and ensures that every slot owner and target references a node in the same definition. It preserves declaration order and exposes GetSlots(ownerNodeId).

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
{"v":1,"g":9001,"gv":1,"n":[[100,1],[101,4],[102,5]],"r":[[100,101,101],[100,102,102]]}
~~~

The n tuples are [instance ID, node-kind ID]. The r tuples are [parent instance ID, slot ID, child instance ID]. Tuple and relationship order are preserved. The codec rejects unknown wire versions, mismatched definition ID/version, duplicate or unexpected properties, malformed tuples, oversized input, and structurally invalid graphs. Serialization is deterministic for the same validated instance.

## Still outside this increment

This addition does not yet provide:
- a host identifier allocator, scope registry, or cross-request interning service;
- semantic interpretation or a host-specific root/entry-point policy;
- automatic migration between different schema versions;
- ProtocolAi category metadata;
- host identifier interning, variable scope, or conversation memory;
- semantic validation, authorization, or execution.

Those concerns remain separate contracts. In particular, a structurally valid schema or future proposal does not authorize an operation.

## Verification

The GrammarStructureDefinitionTests exercise the If/ElseIf/Else schema, occurrence bounds, unknown references, duplicate identities, and immutable snapshots. GrammarStructureInstanceTests exercise a populated If/ElseIf/Else graph, ordered repeated branches, omitted optional else, required-slot enforcement, wrong target kinds, slot ownership, endpoint/identity validation, graph semantics, deterministic codec round trips, stale schema rejection, wire-version rejection, duplicate-property rejection, and structural validation after decoding. Instance collections are bounded by GrammarLimits.MaxStructureInstances and GrammarLimits.MaxStructureRelationships; serialized proposals are bounded by GrammarLimits.MaxProposalCharacters. Repository CI is the execution evidence for these tests.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
