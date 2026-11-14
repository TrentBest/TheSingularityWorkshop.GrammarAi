# GrammarAi proposal for CiAi

**Consumer coordination:** [CiAi issue #59](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/59)  
**Disposition:** adopt  
**Status:** proposal for consumer-owner review; not an approved integration

## Proposed use

Map the real CiWorkflowPlan into GrammarAi productions for ordered steps, dependency edges, typed inputs/outputs, capability declarations, side-effect classes, triggers, and expected evidence roles. Keep workflow scheduling, provider adapters, evidence completeness, evidence verification, gate mathematics, and fail-closed decisions in CiAi. GrammarAi only answers whether the submitted plan has a well-formed structure. Pin the GrammarAi revision and test actual CiAi plan instances, including missing dependencies, cycles where prohibited, unknown symbols, stale schema identity, malformed types, and over-limit graphs. Do not let structural validity imply that a workflow ran or passed. The existing CiAi #59 implementation remains the implementation tracker; GrammarAi #14 remains a distinct observer-command consumer proof.

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
