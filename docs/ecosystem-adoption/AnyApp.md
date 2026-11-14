# GrammarAi assessment for AnyApp

**Consumer coordination:** [AnyApp issue #59](https://github.com/TrentBest/AnyApp/issues/59)  
**Disposition:** existing host-side integration  
**Source reviewed:** `AnyApp` `master` at `6de7c1d6af3f57330bb1ecdc68bfd7f27b58338f`  
**Status:** implemented integration seam; consumer contract hardening remains

## What exists

AnyApp already references `TheSingularityWorkshop.GrammarAi` (`0.1.0-alpha.2`) alongside ProtocolAi and FSM_UserIO. `WorkshopSemanticExchange.cs` defines a host-owned `GrammarDefinition` for Experience intents. Its terminals refer to ProtocolAi symbols using qualified `(ProtocolId, SymbolId)` identities. The host resolves an `FSM_UserIO.SemanticIntent` through ProtocolAi and checks whether that reference appears in the GrammarAi definition.

`ExperienceManifest` can carry a serialized semantic intent. The AnyApp test suite currently verifies intent resolution, grammar admission, protocol decoding, and manifest round-tripping. This is a concrete consumer integration—not merely a proposed future use.

## Ownership boundary

- ProtocolAi owns canonical protocol and symbol identities.
- GrammarAi owns the admitted structural relationship between the host's intent start symbol and those qualified protocol references.
- FSM_UserIO carries the semantic intent.
- AnyApp owns Experience meaning, manifest handling, host policy, user/session context, catalog resolution, and execution.
- FSM_COS remains the runtime composition boundary.

Keep the grammar host-owned. Do not move Experience meaning, trust, policy, authorization, or execution into GrammarAi, and do not add a dependency between the foundation packages.

## Remaining proof / hardening

The current tests prove a narrow positive path. They do not yet establish a complete fail-closed consumer contract. Before calling the integration robust, add consumer-owned negative tests for at least:

1. Unknown protocol and unknown symbol identities.
2. Known-but-not-admitted symbols and wrong protocol IDs.
3. Missing, malformed, and unsupported intent payloads in serialized manifests.
4. Structurally admitted intents that host policy or authorization still rejects.
5. Exact GrammarAi schema identity/version compatibility, with no silent fallback.

Also verify the actual startup/launch path invokes the structural check before any intent-driven action; the reviewed tests establish the helper behavior and manifest round-trip, not by themselves the complete host enforcement path.

## Recommendation

Keep the existing optional host-side integration. Do not broaden GrammarAi into a general AnyApp runtime dependency or replace Experiences/FSM_COS manifest contracts with it. Treat the remaining work as a focused consumer contract-hardening task, coordinated in AnyApp issue #59. No AnyApp source changes are included here.


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
