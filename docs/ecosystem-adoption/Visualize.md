# GrammarAi proposal for Visualize

**Consumer coordination:** [Visualize issue #47](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/issues/47)  
**Disposition:** conditional  
**Status:** proposal for consumer-owner review; not an approved integration

## Proposed use

Use GrammarAi only if a genuinely shared authored scene/representation contract emerges across tools: typed scene nodes, child/relationship slots, and references to domain objects. Visualize owns visualization and world representation behavior; Physics owns physical models; Ontology owns semantic identity. Do not move rendering, simulation, spatial truth, or scene execution into GrammarAi. First identify an existing scene model and demonstrate lossless mapping. If no common authored contract exists, retain local types and close the proposal as no current use.

## Minimal adoption path

1. Identify the existing consumer model and one real producer/consumer boundary; do not invent a parallel domain model.
2. Define only the reusable structural contract: grammar identity/version, node kinds, typed slots, productions, cardinality, ordering, and qualified external references as needed.
3. Pin an immutable GrammarAi revision and, where applicable, ProtocolAi identities. Map the grammar to real consumer types with explicit conversion code.
4. Add consumer-owned positive and negative tests: valid instances, missing/extra fields, wrong categories, unknown or stale references, cardinality/depth limits, and domain-invalid-but-structurally-valid cases.
5. Keep domain truth, semantic validation, security, policy, authorization, evidence, and execution in their existing owners. Record exact commit and CI evidence before calling adoption complete.

## Non-goals and safety boundary

GrammarAi validates structure only. It does not parse arbitrary generated text, infer meaning, establish truth, verify evidence, authenticate content, authorize capabilities, execute actions, or replace the consumer's domain model. A structurally valid proposal can still be semantically invalid, unsafe, untrusted, unauthorized, or impossible to execute.

## Decision requested from the consumer owner

Please review the proposal against current source and choose one explicit outcome: **adopt** (with a real-model test), **adapter-only**, **document-only**, or **no current use**. If the proposed structure does not correspond to a real shared contract, prefer no current use over a speculative dependency. No consumer source changes are included in this proposal.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
