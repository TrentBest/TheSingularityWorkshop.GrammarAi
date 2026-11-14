# Consumer-specific GrammarAi adoption guides

These proposals turn the ecosystem triage into actionable, repository-specific starting points. They are recommendations for owner review, not approvals to add dependencies or change consumer code.

## Guides

| Consumer | Disposition | Proposal |
|---|---|---|
| CiAi | Adopt; implementation underway | [CiAi](CiAi.md) |
| StoryBuilder | Document-only; reconsider if a reusable external grammar emerges | [StoryBuilder](StoryBuilder.md) |
| CommandAi | Optional opaque GrammarAi body seam in draft PR #8; end-to-end consumer proof pending | [CommandAi](CommandAi.md) |
| FSM_UserIO | No core dependency; host-side adapter only if a real proposal language emerges | [FSM_UserIO](FSM_UserIO.md) |
| Experiences | No direct dependency; existing JSON Schema and Contracts validator own manifest structure | [Experiences](Experiences.md) |
| FSM_COS | No kernel dependency; host-side authoring adapter only if a richer proposal language emerges | [FSM_COS](FSM_COS.md) |
| FSM_Foundry | No current use for version-1 workflow contract; revisit only for an external plan language | [FSM_Foundry](FSM_Foundry.md) |
| MicroBundleDomain | Conditional optional authoring adapter; no runtime dependency | [MicroBundleDomain](MicroBundleDomain.md) |
| MicroBundleIngestor | No current use for OpenAPI ingestion; revisit for a distinct shared envelope | [MicroBundleIngestor](MicroBundleIngestor.md) |
| Gestures | No current use; future external activity-authoring boundary only | [Gestures](Gestures.md) |
| Learning | No current use; future machine-readable lesson artifact only | [Learning](Learning.md) |
| Physics | No current use; future symbolic authoring only | [Physics](Physics.md) |
| Economy | No current use; future external proposal boundary only | [Economy](Economy.md) |
| Visualize | No current use; future authored-scene boundary only | [Visualize](Visualize.md) |
| GUI | No current use; future serialized-view boundary only | [GUI](GUI.md) |
| AnyApp | Existing host-side GrammarAi integration; contract hardening remains | [AnyApp](AnyApp.md) |
| WebApp | No current use; repository is still scaffolding | [WebApp](WebApp.md) |
| ServerApp | No current use; repository is still scaffolding | [ServerApp](ServerApp.md) |
| MyVR | No current use; repository is still scaffolding | [MyVR](MyVR.md) |
| SingularityBot | No current use yet; strategy format remains illustrative and unimplemented | [SingularityBot](SingularityBot.md) |

## Integration rule

GrammarAi owns structural shape and bounded structural validation only. Each consumer retains its domain meaning, truth, invariants, semantic validation, quality, security, policy, authorization, and runtime behavior. ProtocolAi remains the owner of canonical vocabulary identities. A well-formed structure is not thereby true, safe, trusted, authorized, or executable.

For every candidate, start from an existing consumer model, pin an immutable GrammarAi revision, write consumer-owned mapping and negative tests, and keep the integration optional until a real cross-boundary use proves value. If no real shared structure exists, choose **no current use** rather than adding a speculative dependency.

The [ecosystem adoption audit](../ECOSYSTEM_ADOPTION_AUDIT.md) records the wider triage, existing coordination threads, and repositories not yet source-audited.
