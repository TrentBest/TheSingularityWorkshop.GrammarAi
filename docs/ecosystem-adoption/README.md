# Consumer-specific GrammarAi adoption guides

These proposals turn the ecosystem triage into actionable, repository-specific starting points. They are recommendations for owner review, not approvals to add dependencies or change consumer code.

## Guides

| Consumer | Disposition | Proposal |
|---|---|---|
| CiAi | Adopt; implementation underway | [CiAi](CiAi.md) |
| StoryBuilder | Next independent pilot | [StoryBuilder](StoryBuilder.md) |
| CommandAi | Optional structural composition | [CommandAi](CommandAi.md) |
| FSM_UserIO | Strong candidate | [FSM_UserIO](FSM_UserIO.md) |
| Experiences | Conditional authoring/adapter | [Experiences](Experiences.md) |
| FSM_COS | Conditional narrow adapter | [FSM_COS](FSM_COS.md) |
| FSM_Foundry | Conditional declarative-plan adapter | [FSM_Foundry](FSM_Foundry.md) |
| MicroBundleDomain | Conditional schema adapter | [MicroBundleDomain](MicroBundleDomain.md) |
| MicroBundleIngestor | Conditional envelope adapter | [MicroBundleIngestor](MicroBundleIngestor.md) |
| Gestures | Conditional authoring draft | [Gestures](Gestures.md) |
| Learning | Conditional single-artifact pilot | [Learning](Learning.md) |
| Physics | Conditional symbolic authoring | [Physics](Physics.md) |
| Economy | Conditional proposal shape | [Economy](Economy.md) |
| Visualize | Conditional shared scene contract | [Visualize](Visualize.md) |
| GUI | Conditional authoring/interchange | [GUI](GUI.md) |
| AnyApp | Shared host adapter only | [AnyApp](AnyApp.md) |
| WebApp | Shared host adapter only | [WebApp](WebApp.md) |
| ServerApp | Conditional protocol/manifest adapter | [ServerApp](ServerApp.md) |
| MyVR | Shared experience/interaction adapter | [MyVR](MyVR.md) |
| SingularityBot | Conditional capability/plan declaration | [SingularityBot](SingularityBot.md) |

## Integration rule

GrammarAi owns structural shape and bounded structural validation only. Each consumer retains its domain meaning, truth, invariants, semantic validation, quality, security, policy, authorization, and runtime behavior. ProtocolAi remains the owner of canonical vocabulary identities. A well-formed structure is not thereby true, safe, trusted, authorized, or executable.

For every candidate, start from an existing consumer model, pin an immutable GrammarAi revision, write consumer-owned mapping and negative tests, and keep the integration optional until a real cross-boundary use proves value. If no real shared structure exists, choose **no current use** rather than adding a speculative dependency.

The [ecosystem adoption audit](../ECOSYSTEM_ADOPTION_AUDIT.md) records the wider triage, existing coordination threads, and repositories not yet source-audited.
