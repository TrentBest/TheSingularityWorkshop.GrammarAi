# GrammarAi assessment for CommandAi

**Consumer coordination:** [CommandAi issue #10](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/issues/10)  
**Implementation candidate:** [CommandAi draft PR #8](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/pull/8)  
**Disposition:** optional structural boundary; no mandatory GrammarAi runtime dependency  
**Status:** transport seam implemented in the draft candidate; end-to-end GrammarAi consumer proof remains outstanding

## Current integration shape

CommandAi owns atomic command and proposal contracts, codecs, compatibility, limits, and command semantics. Its enriched atomic-command candidate now supports an optional opaque GrammarAi body:

- `body.grammarWireVersion` carries a positive wire-version value;
- `body.payloadBase64` carries the canonical Base64 representation of the exact GrammarAi body wire bytes;
- the decoded payload is bounded at 32 KiB per command and 128 KiB per proposal, independently of existing envelope/literal limits;
- body-absent enriched commands and the compact legacy `Command=` representation remain unchanged;
- unknown positive GrammarAi wire versions are preserved opaquely for GrammarAi to accept or reject;
- CommandAi validates framing, canonical Base64, non-empty bytes, and resource bounds, but does not parse grammar nodes, resolve grammar definitions, or validate node/slot/protocol references.

This is the correct ownership split: CommandAi transports a bounded optional grammar payload; GrammarAi owns grammar decoding and structural validation; the trusted host owns active-definition resolution, request context, live-state policy, authorization, approval, and execution.

## Evidence and remaining proof

The latest reported CommandAi candidate head is `3a1fc4ddc15d6543ea8922f5327e5535a61d6919`. Exact-head [CommandAi CI run #43573497740](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/actions/runs/43573497740) succeeded for that SHA, including build, tests/coverage, package creation, and packed-package consumer audit. The NuGet publication job was skipped. PR #8 remains draft; this is not a merge or release.

The remaining integration proof is not merely another opaque-byte round trip. A consumer-side test must:

1. Construct a real GrammarAi definition and proposal using an immutable pinned GrammarAi revision.
2. Bind the intended grammar definition ID and exact schema version to the trusted action-definition mapping / CommandAi precondition.
3. Build an enriched CommandAi command with the exact GrammarAi wire payload and round-trip it through the CommandAi codec.
4. Invoke GrammarAi validation against the intended definition.
5. Reject mismatched or stale definition/schema identity and unresolved, unknown, disabled, or wrong-category external references as applicable.
6. Demonstrate that a structurally valid proposal still cannot authorize or execute a command without the host's independent decision.

GrammarAi issue [#14](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/issues/14) tracks the broader consumer-contract gate. Do not treat the transport seam or CommandAi's codec tests as proof that the end-to-end GrammarAi validator has been exercised by a real consumer. Do not begin that cross-repository test without the required consumer-repository authorization.

## Minimal adoption rule

- Keep GrammarAi optional. A command without a grammar body remains a valid existing command shape.
- Keep the exact grammar bytes opaque inside CommandAi.
- Do not duplicate grammar definition IDs in every body when the trusted action-definition mapping already binds the exact definition/schema identity.
- Do not silently substitute schema versions or aliases.
- Keep CommandAi's local codec/resource checks separate from GrammarAi's structural checks.
- Keep host authorization, policy, live-state checks, approval, and execution outside both grammar validation and the CommandAi wire codec.

## Non-goals and safety boundary

GrammarAi does not own atomic command semantics, target/scope meaning, command codec compatibility, idempotency enforcement, live-state evaluation, evidence verification, authorization, approval, or execution. CommandAi does not parse GrammarAi structure. A structurally valid grammar payload is not thereby a valid domain command, safe operation, authorized action, or successful outcome.

## Decision requested from CommandAi

The current candidate demonstrates a useful optional transport seam. Please confirm that this remains the intended boundary and keep the final consumer validation proof tied to an exact immutable GrammarAi revision. No merge, package publication, release, deployment, master write, or branch deletion is authorized by this proposal.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
