# GrammarAi assessment for FSM_UserIO

**Consumer coordination:** [FSM_UserIO issue #12](https://github.com/TrentBest/FSM_UserIO/issues/12)  
**Current recommendation:** no direct GrammarAi dependency; host-side adapter only if a real structured proposal language emerges  
**Status:** source-based assessment; awaiting consumer-owner review

## What FSM_UserIO owns today

FSM_UserIO is a platform-neutral carrier for semantic interaction, not a command language or policy engine. Its current `development` contract includes:

- `SemanticIntent`: application-defined name plus optional numeric protocol association;
- `SemanticInteraction`: one interpreted intent, required correlation context, and optional observation, expression, and opaque payload;
- `InteractionCapability` / `InteractionMode`: descriptive capability values;
- `SemanticObservation` and `SemanticExpression`: source-neutral evidence/proposal carriers;
- `InteractionContext`: correlation and optional participant/session labels;
- `SemanticFeedback`: producer-reported, non-authoritative feedback;
- `SemanticPayload`: opaque bytes with caller-owned media/schema labels.

These are already explicit typed value contracts with constructor invariants. The published NuGet `0.1.0-alpha.1` remains the earlier `SemanticIntent`-only baseline; the expanded contracts described here are in `development` and are not yet part of that package release.

## Why GrammarAi should not be added to the core

The current API is a set of fixed carrier contracts, not an open-ended nested language. Adding GrammarAi to UserIO would risk turning a deliberately dependency-free semantic boundary into a grammar consumer without a demonstrated need.

In particular:

- GrammarAi does not interpret native input, infer intent, parse arbitrary text, or authenticate source evidence.
- GrammarAi would not add value by restating `SemanticInteraction` as a grammar for the same fixed record shape.
- Protocol identity resolution and namespace scope remain application/ProtocolAi concerns; UserIO's optional numeric `ProtocolId` is only an association value.
- Host/application code still owns domain meaning, trust, validation, authorization, policy, execution, and the truth of feedback.
- UserIO explicitly has no runtime dependency on GrammarAi or ProtocolAi, and that independence is architecturally intentional.

**Recommendation today: no direct GrammarAi dependency in FSM_UserIO.** Keep GrammarAi optional and outside the semantic core.

## A legitimate future adapter boundary

A host or adapter may benefit from GrammarAi if a real AI-/author-authored proposal language appears that composes multiple interactions or capability references into a bounded structure—for example, a typed interaction sequence with ordered steps, scoped protocol-symbol references, and explicit relationships.

If that use case emerges:

1. Define the proposal language in the host/adapter that owns the request and its trust boundary; do not add grammar parsing or authorization to FSM_UserIO.
2. Use ProtocolAi for canonical vocabulary identities where required, with qualified identities and exact catalog/exposure context resolved by the host.
3. Use GrammarAi only to validate the proposal's structural composition: node kinds, slots, productions, ordering, cardinality, and bounded references.
4. Map accepted structural output into existing `SemanticIntent` / `SemanticInteraction` values; do not replace those contracts or introduce a parallel UserIO DTO model.
5. Test real producer/consumer fixtures, including unknown/wrong-category/stale references, malformed or over-limit structures, and structurally valid proposals that host policy still rejects.
6. Keep source provenance, trust, domain meaning, capability availability, policy, authorization, and execution outside GrammarAi.
7. Pin an immutable GrammarAi revision and record exact host commit and CI evidence before claiming adoption.

This would be a host-side or dedicated adapter integration, not a reason for FSM_UserIO itself to depend on GrammarAi.

## Decision requested from FSM_UserIO

Please review this assessment on [FSM_UserIO issue #12](https://github.com/TrentBest/FSM_UserIO/issues/12) and confirm one disposition:

- **No direct use / document-only** — recommended for the current UserIO core;
- **Adapter-only** — if a specific host already has a real structured interaction proposal language;
- **Revisit later** — if multiple consumers demonstrate the same reusable composition contract.

No FSM_UserIO source changes, dependency changes, package changes, or release actions are included or requested.

## Ownership boundary

FSM_UserIO carries semantic interaction. ProtocolAi owns canonical vocabulary identity. GrammarAi owns bounded structural composition. The source adapter owns native conversion. The host owns request context, trust, domain meaning, policy, authorization, and execution. A well-formed grammar does not establish intent, provenance, permission, or successful execution.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
