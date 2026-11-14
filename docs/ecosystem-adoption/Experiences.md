# GrammarAi assessment for Experiences

**Consumer coordination:** [Experiences issue #60](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/60)  
**Current recommendation:** no direct GrammarAi dependency; keep manifest structure in the existing JSON Schema and Experiences.Contracts layer  
**Status:** source-based assessment; awaiting consumer-owner review

## What Experiences already owns

Experiences defines host-independent Experience identity, authored artifact roots, semantic requirements, and publishable MicroBundle artifacts. Its current generic manifest contract is already explicit:

- `docs/contracts/experience-manifest.schema.json` defines schema version 1, required Experience identity/version/title, immutable artifact references, and capability requirements with contract-version ranges.
- The schema rejects additional properties and defines required fields, collection shape, string constraints, SHA-256 formatting, and nested contract-version objects.
- `TheSingularityWorkshop.Experiences.Contracts` implements pure validation, deterministic provider matching, immutable artifact identity checks, and dependency-closure validation, with executable tests in `tests/Contracts.Tests`.
- The semantic interaction requirements contract distinguishes Experience-authored requirements, host-discovered providers, immutable artifact identity, authorization, and composition.

This is a real typed manifest boundary, but it already has a dedicated schema and domain-owned validator.

## Why GrammarAi is not justified today

Adding GrammarAi to the Experience manifest path would currently duplicate structural validation already expressed by JSON Schema and the Experiences.Contracts assembly. It would also risk confusing several distinct owners:

- Experiences owns the authored manifest and requirement semantics.
- MicroBundleDomain owns the neutral bundle/provider contract.
- MicroBundleRepository owns immutable artifact storage and delivery.
- FSM_COS owns host-boundary composition and arbitration.
- The host owns trust, authorization, and activation.
- Physics and other domain repositories own their own model/run semantics.

GrammarAi must not verify artifact hashes, select providers, decide capability compatibility, authorize requirements, or execute composition. A manifest that passes structural validation is not proof that an artifact is trusted, a provider is available, or a composition is authorized.

**Recommendation today: no direct GrammarAi dependency.** Keep the JSON Schema and Experiences.Contracts validator authoritative.

## A future trigger for reconsideration

Revisit GrammarAi only if Experiences introduces a genuinely reusable, externally authored composition language that goes beyond the fixed manifest shape—for example, nested, typed composition expressions or authored dependency graphs shared across multiple producers/consumers and requiring bounded structural validation independent of the manifest schema.

If that need emerges:

1. Name the exact language and producer/consumer boundary.
2. Keep the current manifest schema as the canonical Experience identity/artifact/requirement contract unless a deliberate versioned replacement is approved.
3. Use GrammarAi only for the additional structural language, not to restate the JSON Schema.
4. Keep capability matching, immutable artifact verification, dependency closure, arbitration, policy, authorization, and runtime composition in their current owners.
5. Test real Experience manifests and composition fixtures, including unknown symbols, wrong categories, stale versions, unresolved dependencies, malformed nesting, and valid structures that still fail domain/provider checks.
6. Compare the new checks with existing schema and Contracts tests to prove that GrammarAi adds non-duplicative value.

Any such integration should begin in a host-side or authoring adapter and remain optional until consumer tests prove reuse. It should not make the Experiences repository depend on a runtime grammar engine merely to validate its existing fixed manifest.

## Decision requested from Experiences

Please review this assessment on [Experiences issue #60](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/60) and confirm one disposition:

- **No current use / document-only** — recommended for the existing manifest and validator;
- **Adapter-only later** — if a separate reusable composition language emerges;
- **Adopt** — only with a concrete language and tests proving value beyond the existing JSON Schema and Contracts validator.

No Experiences source, dependency, package, publication, or deployment changes are included or requested.

## Ownership and safety boundary

GrammarAi owns grammar identity, node kinds, slots, productions, typed composition, and bounded structural validation. Experiences owns Experience identity, authored manifest meaning, artifact requirements, and Experience lifecycle. Other owners retain bundle contracts, repository integrity, provider matching, host composition, trust, authorization, and execution. Structural validity does not prove availability, trust, compatibility, or permission.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
