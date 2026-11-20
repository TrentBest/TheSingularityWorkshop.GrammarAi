<!-- The Singularity Workshop moniker -->
<p align="center"><a href="https://github.com/TrentBest"><img src="https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg" alt="The Singularity Workshop animated moniker" width="720" /></a></p>

# GrammarAi problem domain

## The human problem

A vocabulary tells a system what a symbol means, but it does not say which combinations are valid. A model or authoring tool can return individually familiar identities in an invalid order, omit a required relationship, exceed a cardinality constraint, or connect a value to the wrong kind of target.

GrammarAi provides reusable, integer-first structural contracts for those problems. It currently has two complementary representation models: production grammars and typed node/slot knowledge maps. They are related but are not interchangeable.

## The two structural models

### 1. Production grammar

The production model describes:
- an immutable grammar identity and start symbol;
- grammar-owned nonterminal identities;
- uniquely identified, ordered production rules;
- ordered right-hand-side symbols;
- terminals that reference externally owned protocol/category and member identities;
- deterministic structural validation and self-description.

A production rule describes alternatives and ordered symbol sequences. GrammarAi validates the declared grammar; it does not parse free-form text or prove that a generated response conforms to the grammar.

### 2. Typed knowledge map

The typed structural model describes:
- grammar-owned node-kind identities;
- contextual slot identities and their owning node kinds;
- exactly one target kind per slot: a declared GrammarAi node kind or an external ProtocolAi category;
- minimum/maximum relationship cardinality;
- ordered graph instances with nested-node or qualified external-value relationships;
- immutable, versioned definitions and structural instances;
- a compact, versioned proposal codec.

A slot such as `Condition`, `Body`, `Repository`, or `Trigger` is a contextual role in a particular structure. It is not a global lexical category. The external category identifies the vocabulary namespace; the member symbol identifies a value in that namespace; the GrammarAi slot identifies the role that value occupies in this graph.

The host supplies an external-reference resolver bound to the exact catalog/capability snapshot used for the request. Only a `ResolvedAndExposed` reference is accepted. GrammarAi has no global registry and does not decide which values a host should expose.

See [Typed Structural Schema](STRUCTURAL_SCHEMA.md) for the full graph and wire contract, including v1/v2 compatibility boundaries.

## Responsibilities and ownership

| Layer | Owns | Does not own |
|---|---|---|
| ProtocolAi | Vocabulary/category identity and member-symbol identity | Grammar structure, domain authorization, execution |
| GrammarAi | Production rules, node kinds, contextual slots, typed relationships, composition, bounded structural validation | Global vocabulary registry, host snapshot selection, semantic truth, policy, authorization, execution |
| Consumer domain | Meaning, domain invariants, semantic validation, quality, and runtime behavior | Reusable generic grammar representation |
| Host | Identifier scope/allocation, active context, catalog snapshot, resolver, interpretation, trust, policy, authorization, and execution | The reusable GrammarAi definition itself |
| CommandAi | Command proposal and atomic command representation, optional opaque grammar-body transport | Grammar decoding/validation, domain meaning, authorization, execution |
| HeadlessAi | Configured AI endpoint request/response communication | Semantic interpretation or authority |

A valid identity, grammar, graph, or command proposal is not proof that its content is true, safe, policy-compliant, authorized, or executable.

## Resource boundaries

The production grammar applies explicit limits before retaining untrusted collections: at most 4,096 rules, 1,024 symbols per production, and 65,536 right-hand-side symbols across a grammar. Grammar names are limited to 256 UTF-16 characters.

The typed structural model separately bounds node/relationship collections and serialized proposal size through `GrammarLimits`. The codec rejects malformed or unsupported wire versions, unexpected/duplicate properties, invalid tuple shapes, stale definition identity/version, oversized payloads, and structurally invalid graphs. The exact limits and wire rules are maintained in [Typed Structural Schema](STRUCTURAL_SCHEMA.md); do not infer that the production-grammar limits are the graph limits.

These are representation and resource bounds, not semantic correctness guarantees or authorization controls.

## Acceptance criteria

A production grammar is structurally acceptable when:
1. Grammar identity, name, and start symbol are explicit and non-zero.
2. Rule identities are unique.
3. The start symbol is defined by a production.
4. Every referenced nonterminal has a production.
5. External protocol references preserve qualified identity and external ownership.
6. Description output is deterministic for the same grammar definition.

A typed graph is structurally acceptable when:
1. The selected definition identity and version match the proposal.
2. Node and slot identities are unique and slot owners are declared.
3. Every slot declares exactly one valid target kind.
4. Every relationship targets the declared node kind or external category.
5. Required/minimum and maximum cardinality constraints are satisfied.
6. Relationship order is preserved.
7. External values resolve as exposed in the exact host-selected snapshot.
8. Wire version, tuple shape, uniqueness, and payload resource limits are satisfied.

Consumer acceptance adds separate domain-specific checks. A structurally valid graph does not establish that a BotDefinition is legal, a C# program compiles, CI evidence is authentic, or an action may execute.

## Deliberate non-goals

GrammarAi is not a natural-language parser, compiler, Roslyn replacement, tokenizer, provider-specific constrained-decoding exporter, model client, protocol transport, global vocabulary registry, database, query engine, workflow engine, evidence verifier, policy engine, authorization layer, or executor.

Bounded C# expression, type, and declaration grammar resources are structural examples and reusable maps; they are not a complete C# grammar or proof of compiler conformance. The host/compiler retains lexical recognition, binding, semantic diagnostics, language-version enforcement, and compilation.

## Evidence and maturity

The production grammar is exercised by the repository's grammar tests and examples. The typed structural model is exercised by definition/instance/codec tests, including cardinality, target-kind, resolver, ordering, malformed payload, version mismatch, and resource-limit cases. The repository QuickStart exercises both production grammars and a conditional-control-flow graph, including a snapshot-bound external reference.

This is evidence for the library's own structural contract, not proof of every downstream consumer integration. For practical API use, see [Consuming GrammarAi](CONSUMING.md), [Examples](EXAMPLES.md), and [Knowledge Mapping Model](KNOWLEDGE_MAPPING_MODEL.md). For consumer adoption status, see [Ecosystem Adoption Audit](ECOSYSTEM_ADOPTION_AUDIT.md) and [GrammarAi and SingularityBot](INTEGRATION_SINGULARITYBOT.md).

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
