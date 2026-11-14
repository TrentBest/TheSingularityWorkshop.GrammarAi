# GrammarAi assessment for AnyApp

**Consumer coordination:** [AnyApp issue #59](https://github.com/TrentBest/AnyApp/issues/59)  
**Disposition:** existing host-side integration  
**Source reviewed:** `AnyApp` `development` at `f256e9286ff766b6de72d1f4bcd13d5e85a76451`  
**Status:** implemented and exercised; keep host-owned and bounded

## What exists

AnyApp references GrammarAi in both dependency modes: a released `PackageReference` and a source `ProjectReference` for repository-source builds. `WorkshopSemanticExchange.cs` defines a host-owned GrammarAi `GrammarDefinition` whose productions admit selected, qualified ProtocolAi symbols. The host resolves `FSM_UserIO.SemanticIntent` values against its ProtocolAi vocabulary and checks whether the resolved symbol is admitted by the grammar.

This is used in the actual launch path: `MainWindow.LaunchManifestAsync` calls `WorkshopSemanticExchange.ValidateManifestIntent(manifest)` before configuring/starting host services or composing the destination Experience. The manifest's optional intent is therefore checked before launch, not merely represented in a test helper.

The current test suite covers:
- admitted intent resolution and protocol decoding;
- a ProtocolAi-known symbol that the GrammarAi grammar deliberately does not admit;
- unknown and foreign protocol intents;
- rejection of a manifest carrying a known-but-not-admitted intent;
- acceptance of a manifest with no intent or an admitted intent.

This is a concrete, real consumer integration—not a speculative proposal.

## Ownership boundary

- ProtocolAi owns canonical protocol and symbol identities.
- GrammarAi owns the admitted structural relationship between the host's intent start symbol and those qualified protocol references.
- FSM_UserIO carries the semantic intent.
- AnyApp owns Experience meaning, manifest handling, host policy, user/session context, catalog resolution, and execution.
- FSM_COS remains the runtime composition boundary.

Keep the grammar host-owned. Do not move Experience meaning, trust, policy, authorization, or execution into GrammarAi, and do not add dependencies between the foundation packages.

## Recommendation

Retain the existing integration as-is. Its value is narrow and concrete: a host can distinguish a protocol-known symbol from one admitted by this host's structural grammar before acting on the manifest. Do not broaden this into a general Experience-manifest validator or make GrammarAi responsible for artifact identity, content hashes, capability resolution, authorization, or composition.

No further AnyApp implementation is proposed by this adoption assessment.


---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
