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

The public API currently includes:
- GrammarNodeDefinition
- GrammarSlotDefinition
- GrammarStructureDefinition

GrammarStructureDefinition is immutable and validates its node/slot references. This is a schema definition, not yet a proposal-instance model or compact wire codec.

## Still outside this increment

This addition does not yet provide:
- node instances or an object-graph builder;
- a canonical serialized proposal format;
- grammar/mapping version negotiation;
- ProtocolAi category metadata;
- host identifier interning, variable scope, or conversation memory;
- semantic validation, authorization, or execution.

Those concerns remain separate contracts. In particular, a structurally valid schema or future proposal does not authorize an operation.

## Verification

The GrammarStructureDefinitionTests exercise the If/ElseIf/Else schema, occurrence bounds, unknown references, duplicate identities, and immutable snapshots. GrammarStructureInstanceTests exercise a populated If/ElseIf/Else graph, ordered repeated branches, omitted optional else, required-slot enforcement, wrong target kinds, slot ownership, endpoint/identity validation, and the deliberately graph-shaped semantics. Both collections are bounded by GrammarLimits.MaxStructureInstances and GrammarLimits.MaxStructureRelationships. Repository CI is the execution evidence for these tests.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
