# GrammarAi ecosystem adoption audit

**Audit date:** 2026-11-14  
**Working branch:** `development`  
**Coordination register:** [GrammarAi #20](https://github.com/TrentBest/TheSingularityWorkshop.GrammarAi/issues/20)  
**Purpose:** decide where GrammarAi adds real structural value without creating unnecessary dependencies or moving domain authority.

## Executive decision

GrammarAi should not become a mandatory package across the Workshop. It is useful when a real producer/consumer boundary needs a versioned, typed structure with meaningful roles, slots, ordering, cardinality, or nested composition—especially when an AI or authoring surface proposes that structure and a host must validate it independently.

The current repository coordination already contains 27 consumer-specific issues. They are not 27 approved integrations. They are requests for each owner to confirm **adopt**, **adapter-only**, **document-only**, or **no current use**. The audit found all 27 linked issues still open on 2026-11-14. No consumer issue should be duplicated.

## Adoption rule

Recommend GrammarAi only when all of the following are true:

1. There is a real shared structure or an AI-/author-authored proposal—not merely an ordinary local configuration object.
2. Its relationships, roles, ordering, cardinality, or compatibility need independent structural validation.
3. A consumer can test the mapping against its actual domain model and a pinned GrammarAi revision.
4. The domain repository can retain its own meaning, truth, semantic invariants, quality rules, security, policy, and runtime behavior.

If a local C# model or an existing domain validator already expresses the contract more clearly, keep it. GrammarAi must not become a parser, database, query engine, workflow engine, physics solver, renderer, policy engine, evidence verifier, authorization layer, or executor.

## Confirmed architectural ownership

- **ProtocolAi:** canonical vocabulary and protocol/category/member identities.
- **GrammarAi:** grammar identity, node kinds, slots, productions, typed composition, bounded structural validation, and structural proposal representation.
- **Consumer domain:** meaning, truth, domain invariants, semantic validation, quality, and runtime behavior.
- **Host / FSM_COS / Foundry:** request context, trust, policy, authorization, orchestration, and execution according to their respective boundaries.
- **CiAi:** CI provider evidence, completeness, evidence verification, and gate decisions.
- **CommandAi:** atomic command and proposal contracts; host remains responsible for authorization and execution.

A structurally valid proposal is not thereby true, safe, physically valid, policy-compliant, authorized, or executable.

## Repository-by-repository disposition

This table is an initial triage, not a claim that every source tree has been exhaustively inspected. The linked consumer issue is the coordination point; the consumer owner must confirm the disposition and identify the real test.

| Repository | Initial disposition | Concrete structural opportunity | Boundary / next proof |
|---|---|---|---|
| [CiAi #59](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/59) | **Adopt; implementation underway** | Versioned CI workflow plan: ordered steps, dependencies, triggers, typed inputs/outputs, capabilities, side effects, expected evidence roles | CiAi retains workflow semantics, live evidence verification, gate mathematics, and fail-closed authority. Test real CiWorkflowPlan data against the pinned GrammarAi schema. GrammarAi #14 remains a separate consumer-proof/coordination issue. |
| [StoryBuilder #8](https://github.com/TrentBest/StoryBuilder/issues/8) | **Document-only; reconsider later if a reusable external grammar emerges** | Current source-driven JSON CompositionManifestImporter maps into StoryWorkspace and already enforces its fixed page-order contract; StoryWorkspaceValidator owns cross-record integrity | No direct GrammarAi dependency is justified now: it would duplicate the fixed importer/domain validators. Reconsider only for a reusable externally authored structure, with real-model tests proving non-duplicative bounded validation. Earlier draft type names StoryProject, TimelineEvent, and PassageAnchor do not match the current documented contract. |
| [CommandAi #10](https://github.com/TrentBest/TheSingularityWorkshop.CommandAi/issues/10) | **Optional structural boundary; no mandatory runtime dependency** | Draft PR #8 carries an optional opaque GrammarAi body with wire version and bounded canonical Base64 payload; CommandAi validates framing/limits but not grammar structure | Exact-head CommandAi CI passed on `3a1fc4d`; PR #8 remains draft. End-to-end real GrammarAi definition/proposal validation against an exact pin remains outstanding; codec transport tests alone do not prove it. |
| [ProtocolAi #15](https://github.com/TrentBest/TheSingularityWorkshop.ProtocolAi/issues/15) | **Foundational counterpart; not a GrammarAi consumer** | Stable vocabulary/category/member identities referenced by GrammarAi | Keep ProtocolAi as identity owner. Confirm qualified references and compatibility rules; do not create a competing vocabulary registry. |
| [FSM_UserIO #12](https://github.com/TrentBest/FSM_UserIO/issues/12) | **No direct core dependency; host-side adapter only if a real proposal language emerges** | Current source already has fixed typed carriers: SemanticIntent, SemanticInteraction, capability, observation/expression, context, feedback, and opaque payload | GrammarAi would duplicate the fixed envelope if used inside UserIO. Keep UserIO dependency-free; revisit only for a host-owned, AI-/author-authored composition language with real consumer tests. |
| [Experiences #60](https://github.com/TrentBest/TheSingularityWorkshop.Experiences/issues/60) | **No direct dependency; revisit only for a separate reusable composition language** | Generic Experience manifest already has JSON Schema v1 plus Experiences.Contracts validation, provider matching, immutable artifact checks, and dependency-closure validation | GrammarAi would duplicate the fixed manifest shape today. Keep capability matching, artifact integrity, composition, trust, and authorization in existing owners; require a new non-duplicative structural language before reconsidering. |
| [FSM_COS #55](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS/issues/55) | **No kernel dependency; host-side authoring adapter only if a richer proposal language emerges** | Current RuntimeManifest and RuntimeManifestSchedule are typed contracts; FSM_COS already validates dependency closure, identity/version matching, cycles, schedule consistency, and bounded arbitration | GrammarAi inside the kernel would duplicate composition validation. Keep manifest mapping and runtime resolution in FSM_COS; reconsider only for a distinct host-owned authoring language with non-duplicative tests. |
| [FSM_Foundry #37](https://github.com/TrentBest/TheSingularityWorkshop.FSM_Foundry/issues/37) | **No current use for the existing version-1 workflow contract** | Only reconsider if an independently authored/interchanged plan language appears beyond the current ordered steps and handler keys | Foundry retains validation, scheduling, lifecycle, state progression, retries, cancellation, and execution. CiAi owns evidence gates; CommandAi owns atomic commands. |
| [MicroBundleDomain #27](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain/issues/27) | **Conditional; optional authoring adapter only** | External/AI-authored nested capability or field declarations mapped to existing MicroBundleDefinition | MicroBundleDomain retains canonical identity, version, dependency/provider semantics, field meaning, and compatibility. No GrammarAi runtime dependency. |
| [MicroBundleRepository #36](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/36) | **No direct runtime dependency indicated** | Possibly an interchange-only artifact-reference/dependency declaration | Repository owns storage, retrieval, versioning, indexing, and integrity. GrammarAi must not access storage or verify hashes. |
| [MicroBundleIngestor #5](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleIngestor/issues/5) | **No current use for the existing OpenAPI path** | Reconsider only for a distinct versioned handoff envelope shared by independent producers/consumers | OpenAPI already supplies the formal source contract; Ingestor owns acquisition, parsing, canonicalization, compilation, inspection, and artifact lifecycle. GrammarAi must not ingest or trust content. |
| [Ontology #8](https://github.com/TrentBest/TheSingularityWorkshop.Ontology/issues/8) | **Complementary; document/adapter only** | Grammar structures may reference ontology identities | Ontology retains semantic identity and relationship truth. Do not collapse ontology predicates into grammar slots or add a dependency in the wrong direction. |
| [Gestures #35](https://github.com/TrentBest/TheSingularityWorkshop.Gestures/issues/35) | **No current use; future external authoring boundary only** | Reconsider if a serialized ActivityDefinition graph is independently authored and consumed by multiple tools | Current builder is local typed C#; serialization, live coordination, and general graph composition remain future work. Gesture playback is fixed ordered sequencing, not a shared grammar. |
| [Learning #3](https://github.com/TrentBest/TheSingularityWorkshop.Learning/issues/3) | **No current use; future machine-readable lesson artifact only** | Reconsider if one versioned lesson graph is shared by independent authoring/consuming tools | Current learning paths are authored Markdown trails and source-linked lessons, not a shared runtime curriculum schema. Learning retains pedagogy and mastery. |
| [Physics #51](https://github.com/TrentBest/TheSingularityWorkshop.Physics/issues/51) | **No current use; future symbolic authoring only** | Reconsider if an external producer submits a versioned symbolic model tree consumed independently | Existing run-record envelope is metadata/interoperability, not a symbolic expression language. Physics retains units, dimensional analysis, model validity, solvers, and simulation. |
| [Economy #28](https://github.com/TrentBest/TheSingularityWorkshop.Economy/issues/28) | **No current use; future external proposal boundary only** | Reconsider if an external authoring producer submits a versioned economic proposal consumed by independent tools | Current offer/transfer/execution types are local typed domain contracts with existing validation; no shared serialized transaction grammar is demonstrated. Economy retains accounting and transaction meaning. |
| [Renderer #36](https://github.com/TrentBest/TheSingularityWorkshop.Renderer/issues/36) | **No direct runtime dependency indicated** | Possible interchange grammar for a renderer document/request | Renderer remains a MicroBundle and owns rendering semantics. Keep its consumption model; GrammarAi must not become a renderer or NuGet consumption dependency. |
| [Visualize #47](https://github.com/TrentBest/TheSingularityWorkshop.Visualize/issues/47) | **No current use; future authored-scene boundary only** | Reconsider if an external producer creates a shared serialized scene graph mapped to VisualizerFrame | Current contracts describe frames, geometry/resources, and draw work, not a general authored scene graph. Visualize owns presentation execution; Physics and Ontology retain their domains. |
| [GUI #79](https://github.com/TrentBest/TheSingularityWorkshop.GUI/issues/79) | **No current use; future serialized-view boundary only** | Reconsider when an external editor/AI authoring surface exchanges versioned GUI trees | GUI already owns a recursive typed GuiNode/GuiBuilder model; serialization is explicitly future work and no independent producer/consumer boundary is shown. GUI retains semantic view and adapter contracts. |
| [AnyApp #59](https://github.com/TrentBest/AnyApp/issues/59) | **Existing host-side GrammarAi integration** | Host-owned Experience intent grammar links GrammarAi productions to qualified ProtocolAi symbols; FSM_UserIO SemanticIntent is resolved and checked before launch | AnyApp development references GrammarAi in source/package modes. MainWindow calls ValidateManifestIntent before starting host services/composition. Tests cover admitted, known-but-not-admitted, unknown, and foreign intents plus manifest rejection. AnyApp retains meaning, trust, policy, authorization, and execution. |
| [WebApp #16](https://github.com/TrentBest/TheSingularityWorkshop.WebApp/issues/16) | **No current use; existing host/bridge contracts are sufficient** | Reassess only if a reusable authored cross-host interaction/composition graph emerges | Development contains an active FSM_COS host and documented bounded WebApp–AnyApp bridge, but no GrammarAi dependency or shared grammar-backed structural language. Existing fixed host and security contracts remain authoritative. |
| [ServerApp #12](https://github.com/TrentBest/TheSingularityWorkshop.ServerApp/issues/12) | **No current use; JSON Schema and domain validators cover v1** | Reassess only for a distinct reusable nested provisioning/placement authoring language | Development has a versioned ServerRoleManifest, JSON Schema v1, strict JSON parsing, domain validator, topological provisioning-plan validation, and host allowlist. GrammarAi would duplicate the current fixed contract. |
| [MyVR #32](https://github.com/TrentBest/TheSingularityWorkshop.MyVR/issues/32) | **No current use yet; architecture documented, no source contract** | Reassess when device/session and bounded interaction contracts exist in source | Development documents capability arrangement and companion security, but has no C# source or project files and no concrete structural artifact to validate. |
| [Profiles #7](https://github.com/TrentBest/TheSingularityWorkshop.Profiles/issues/7) | **No direct dependency indicated; privacy review required** | Only a privacy-safe, reusable preference/profile declaration | Profiles retains consent, privacy, access, and lifecycle. Do not expose sensitive profile semantics through generic grammar. |
| [DataWarehouse #8](https://github.com/TrentBest/TheSingularityWorkshop.DataWarehouse/issues/8) | **No direct runtime dependency indicated** | Possibly a typed record/query envelope at an external interchange boundary | DataWarehouse retains storage, indexing, retrieval, and persistence; Ontology retains semantic identity. GrammarAi is not a query engine. |
| [Hermit #4](https://github.com/TrentBest/TheSingularityWorkshop.Hermit/issues/4) | **No current use until a concrete task/object contract exists** | Possible task-plan or object/constraint declaration | Hermit retains orchestration and manipulation; Physics owns physical validity; host owns authorization. Avoid speculative dependency. |
| [FSM_NeuralNetworks #3](https://github.com/TrentBest/TheSingularityWorkshop.FSM_NeuralNetworks/issues/3) | **No direct runtime dependency indicated** | Possible model/training configuration envelope | NeuralNetworks retains architecture, tensor semantics, training/inference, GPU execution, and performance. GrammarAi is not a tensor/model validator. |
| [SingularityBot #12](https://github.com/TrentBest/TheSingularityWorkshop.SingularityBot/issues/12) | **Conditional; optional authoring/knowledge-graph adapter only** | Typed graph of capabilities, goals, evidence, strategy relationships, prerequisites, and constraints shared by Forge/LLM authoring tools | Development has a real BotDefinition and domain validators plus CommandAi/HeadlessAi integration, but no GrammarAi dependency. Consider GrammarAi only for a distinct multi-tool graph contract; do not duplicate BotDefinition schema or add GrammarAi to the runtime. |

## Existing coordination state

The 27 consumer issues above were created before this audit and were all verified open on 2026-11-14. They are the correct places for repository owners to respond; do not create duplicate issues.

The coordination register also records existing work that should be connected rather than reimplemented:

- CiAi #59 is the active implementation tracker for the typed workflow-plan integration.
- GrammarAi #14 tracks the observer-command/gate-predicate consumer contract proof; the remaining test work is in CiAi, not GrammarAi.
- CommandAi #10 coordinates the optional composition boundary; CommandAi's existing proposal envelope remains its own contract.
- StoryBuilder #8 was inspected as the next independent candidate. The current source-backed result is document-only: the importer and domain validator already cover the fixed contract, so there is no demonstrated non-duplicative GrammarAi role yet.
- The other consumer issues should be triaged into adopt, adapter-only, document-only, or no current use based on actual source-backed examples and tests.

## Repositories not yet individually source-audited

The connected 27-repository register is the current Workshop adoption scope. The authenticated account also exposes historical, external, and utility repositories (including older FSM, BIM/Revit, Unity, bot, and demo projects). Their repository names were enumerated, but they have not all been inspected at source level in this pass. Do not infer that they need GrammarAi. Review those repositories only when they contain an active AI-facing or cross-repository structural contract.

## Recommended sequence

1. Complete the CiAi proof already underway without moving evidence or authorization ownership into GrammarAi.
2. Use StoryBuilder #8 as the next independent, real-domain pilot.
3. Resolve CommandAi #10 against its existing atomic proposal and codec contracts.
4. Keep FSM_UserIO #12 dependency-free; revisit only if a host demonstrates a reusable AI-/author-authored interaction composition language.
5. Keep Experiences and FSM_COS dependency-free at their current manifest boundaries; Foundry has no demonstrated GrammarAi use in its version-1 workflow contract; consider MicroBundleDomain only for a real external authoring adapter. MicroBundleIngestor should not wrap its existing OpenAPI path in GrammarAi. Current Gestures, Learning, Physics, Economy, Visualize, and GUI source also shows no non-duplicative GrammarAi need; revisit only if their specified external authoring/interchange boundaries actually emerge.
6. Retain AnyApp's proven host-side GrammarAi intent gate. Keep WebApp and ServerApp on their existing host/bridge and JSON Schema/domain-validation contracts; MyVR needs source-level device/session contracts before reassessment. Keep SingularityBot's BotDefinition and validators authoritative; consider only an optional shared knowledge-graph authoring adapter if a real multi-tool contract emerges. Close out remaining consumer issues with an explicit disposition and prefer **no current use** over speculative package references.

## Verification checklist for every adoption

- Pin the exact GrammarAi revision or package version.
- Map the consumer's real domain object—not a test-only lookalike.
- Identify every ProtocolAi category/member identity and qualify external references.
- Bind resolution to the exact request/catalog/capability snapshot.
- Test valid structures and malformed, unknown, disabled, stale, wrong-category, and over-limit cases as applicable.
- Keep semantic/domain validation in the consumer.
- Prove structural validity cannot authorize, execute, grade, verify evidence, or assert domain truth.
- Record the exact consumer commit and test/CI run.
- Do not merge, publish, release, deploy, or delete branches without the required separate authorization.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/TheSingularityWorkshop.png" alt="The Singularity Workshop" width="200" /></p>
