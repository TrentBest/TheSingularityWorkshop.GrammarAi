# GrammarAi proposal for FSM_Foundry

**Consumer coordination:** [FSM_Foundry issue #37](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/37)  
**Disposition:** conditional  
**Status:** proposal for consumer-owner review; not an approved integration

## Proposed use

Consider GrammarAi only for a shared declarative plan format authored outside Foundry: typed step nodes, inputs/outputs, dependency edges, and bounded nested groups. Foundry continues to own scheduling, state progression, retries, lifecycle, cancellation, and execution. CiAi owns CI evidence and gates; CommandAi owns atomic command contracts. Avoid recreating a workflow engine or moving runtime state into grammar definitions. Prototype an adapter over an existing Foundry plan, then test malformed edges, missing ports, cycles, unsupported step kinds, and structurally valid plans that Foundry must still reject on runtime policy or resource constraints.

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
